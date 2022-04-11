using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

/// <summary>
/// Класс представляет собой простой Http-сервис.
/// </summary>
public class SimpleHttpService
{
    readonly ILogger<SimpleHttpService> _log;
    readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Базовый Uri-адресс Http-ресурса.
    /// </summary>
    protected string BaseUri { get; } = string.Empty;

    /// <summary>
    /// Тайм-аут по умолчанию для запроса будет установлен, если пользователь не установил его.
    /// </summary>
    public TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// HttpClient из фабрики Http-клиентов.
    /// </summary>
    public HttpClient HttpClient { get; }

    /// <summary>
    /// Фабрка логгеров.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Инициализировать новый экземпляр класса <see cref="SimpleHttpService"/>.
    /// </summary>
    /// <param name="httpClient">HttpClient из http-клиентской фабрики.</param>
    /// <param name="loggerFactory">Фабрка логгеров.</param>
    /// <param name="baseUri">Базовый URL-адрес всех запросов. Например: http://site.ru</param>
    public SimpleHttpService(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        string? baseUri = null)
    {
        HttpClient = httpClient;
        HttpClient.Timeout = Timeout.InfiniteTimeSpan;
        LoggerFactory = loggerFactory;

        if (!string.IsNullOrEmpty(baseUri))
        {
            BaseUri = AddTrailingSlash(baseUri);
            HttpClient.BaseAddress = new Uri(BaseUri);
        }

        _log = loggerFactory.CreateLogger<SimpleHttpService>();
    }

    /// <summary>
    /// Выполнить HTTP-запрос GET и вернуть результат, десериализованный в тип <typeparamref name = "TResult" />.
    /// </summary>
    /// <typeparam name="TResult">Тип результата запроса.</typeparam>
    /// <param name="uri">Uri вызываемой службы.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="headers">Заголовки Http-запроса, которые будут установлены на HttpRequestMessage.</param>
    /// <exception cref="ResponseException"></exception>
    public Task<RestHttpResponseMessage<TResult?>> Get<TResult>(string uri, CancellationToken cancellationToken = default,
        IHeaderDictionary? headers = default) =>
        MakeRequestWithoutBody<TResult>("GET", uri, cancellationToken, headers);

    [return: NotNull]
    protected virtual async Task<RestHttpResponseMessage<TResult?>> MakeRequestWithoutBody<TResult>(
        string requestType,
        string uri,
        CancellationToken? cancellationToken = default,
        IHeaderDictionary? headers = default)
    {
        var cancellationTokenSource = cancellationToken ?? new CancellationTokenSource(DefaultTimeout).Token;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var fullUri = GetAbsoluteUri(uri);
        LogStartEvent(requestType, fullUri);
        HttpResponseMessage response;

        var result = string.Empty;
        try
        {
            var method = new HttpMethod(requestType);
            response = await DoHttpRequest(uri, headers, method, cancellationTokenSource);

            var content = response.Content;
            result = await content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            LogRequestException(requestType, fullUri, null, result, e, stopwatch);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }

        CheckStatusCode(requestType, fullUri, response, null, result, stopwatch);
        LogEndEvent(requestType, fullUri, response.StatusCode, stopwatch);

        return new RestHttpResponseMessage<TResult?>(response) { ResultObject = JsonConvert.DeserializeObject<TResult>(result, _jsonSettings) };
    }

    void LogStartEvent(string method, Uri uri)
    {
        _log.LogInformation(
            new EventId(TraceConstants.DownServiceEventId),
            "Start downstream request {Method} {Path}.",
            method,
            uri.ToString());
    }

    void LogRequestException(string method, Uri uri, string? requestData, string responseData, Exception e, Stopwatch stopWatch)
    {
        _log.LogError(
            new EventId(TraceConstants.DownServiceEventId),
            e,
            "Downstream request {Method} {Path} failed with " +
            "Exception at {elapsedMilliseconds} ms. " +
            "Request body: {@ServiceRequestData}. Response body: {@ServiceResponseData}. Exception message: " + e.Message,
            method,
            uri.ToString(),
            stopWatch.ElapsedMilliseconds,
            requestData,
            responseData);
    }

    void LogEndEvent(string method, Uri uri, HttpStatusCode statusCode, Stopwatch stopWatch)
    {

        _log.LogInformation(
            new EventId(TraceConstants.DownServiceEventId),
            "Downstream request {Method} {Path} finished with " +
            "StatusCode {StatusCode} at {elapsedMilliseconds} ms.",
            method,
            uri.ToString(),
            (int)statusCode,
            stopWatch.ElapsedMilliseconds);
    }

    Uri GetAbsoluteUri(string uri)
    {
        return !Uri.IsWellFormedUriString(uri, UriKind.Absolute)
            ? new Uri(HttpClient.BaseAddress, uri)
            : new Uri(uri);
    }

    void SetHttpRequestHeaders(HttpRequestMessage request, IHeaderDictionary? headers)
    {
        if (headers is null || !headers.Any()) return;
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value.ToArray());
        }
    }

    async Task<HttpResponseMessage> DoHttpRequest(string uri,
        IHeaderDictionary? headers,
        HttpMethod method,
        CancellationToken cancellationTokenSource,
        HttpContent? content = default)
    {
        var request = new HttpRequestMessage(method, uri);
        if (content is not null)
            request.Content = content;
        SetHttpRequestHeaders(request, headers);

        var response = await HttpClient.SendAsync(request, cancellationTokenSource);
        response.RequestMessage = request;
        return response;
    }

    void CheckStatusCode(string method,
        Uri uri,
        HttpResponseMessage response,
        string? requestData,
        string? responseData,
        Stopwatch stopWatch)
    {
        if (response.IsSuccessStatusCode)
            return;

        _log.LogError(
            new EventId(TraceConstants.DownServiceEventId),
            "Downstream request {Method} {Path} failed with " +
            "StatusCode {StatusCode} at {elapsedMilliseconds} ms. Request body: {@ServiceRequestData}. " +
            "Response body: {@ServiceResponseData}.",
            method,
            uri.ToString(),
            (int)response.StatusCode,
            stopWatch.ElapsedMilliseconds,
            requestData,
            responseData);
        throw new ResponseException(
            $"Downstream request failed with status code {(int)response.StatusCode} at {stopWatch.ElapsedMilliseconds} ms. " +
            $"Request body: {requestData}. Response body: {responseData}.",
            response.StatusCode,
            responseData);
    }

    string AddTrailingSlash(string baseUri)
    {
        if (baseUri.Last() != '/')
            return baseUri + "/";

        return baseUri;
    }
}