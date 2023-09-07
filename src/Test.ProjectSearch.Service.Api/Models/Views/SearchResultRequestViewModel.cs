/// <summary>
/// Модель представления ответа, полученного из <see cref="IGitHubApiHttpService.SearchProject"/>.
/// </summary>
public class SearchResultRequestViewModel
{
    /// <summary>
    /// Общее количество репозиториев.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Список репозиториев.
    /// </summary>
    public IEnumerable<RepositoryRequestViewModel> Items { get; set; }
}