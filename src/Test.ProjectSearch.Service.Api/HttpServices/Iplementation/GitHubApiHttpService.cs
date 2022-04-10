using Microsoft.Extensions.Options;
using static AppConstants;

/// <summary>
/// Реализация <see cref="IGitHubApiHttpService"/>.
/// </summary>
public class GitHubApiHttpService : SimpleHttpService, IGitHubApiHttpService
{
    readonly IOptions<GitHubHttpHeaderOptions> _options;

    /// <summary>
    /// Инициализировать новый экземпляр класса <see cref="GitHubApiHttpService"/>.
    /// </summary>
    /// <param name="httpClient">Http клиент.</param>
    /// <param name="loggerFactory">Фабрка логгеров.</param>
    public GitHubApiHttpService(
            IOptions<GitHubHttpHeaderOptions> options,
            HttpClient httpClient,
            ILoggerFactory loggerFactory)
        : base(
            httpClient,
            loggerFactory,
            new Uri(GitHubApiUrl).ToString())
    {
        _options = options;
    }

    ///<inheritdoc/>
    public async Task<SearchResultRequestViewModel> SearchProject(string name)
    {
        var uri = $"/search/repositories?q={name}";

        var result = await Get<SearchResultRequestViewModel>(uri, headers: _options.Value.ForwardedHeaders);
        return result.ResultObject;
    }
}