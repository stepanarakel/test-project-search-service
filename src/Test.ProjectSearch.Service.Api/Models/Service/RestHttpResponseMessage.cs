/// <summary>
/// Расширенная версия класса <см. cref = "HttpResponseMessage" />,
/// которая предоставляет готовые методы для получения десериализованных объектов для базовых запросов.
/// </summary>
/// <typeparam name="TResult">The Response result type.</typeparam>
public class RestHttpResponseMessage<TResult>
{
    /// <summary>
    /// Объект, десериализованный из JSON в тип <typeparamref name = "TResult" />.
    /// </summary>
    public TResult? ResultObject { get; set; }

    /// <summary>
    /// Оригинальный ответ от HttpClient.
    /// </summary>
    public HttpResponseMessage OriginalResponse { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref = "RestHttpResponseMessage {TResult}" />.
    /// </summary>
    /// <param name="responseMessage">Ответ HttpClient.</param>
    public RestHttpResponseMessage(HttpResponseMessage responseMessage)
    {
        OriginalResponse = responseMessage;
    }
}