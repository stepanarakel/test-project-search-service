/// <summary>
/// 
/// </summary>
public class SearchResult
{
    /// <summary>
    /// Общее количество репозиториев.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Список репозиториев.
    /// </summary>
    public IEnumerable<Repository> Items { get; set; } = Enumerable.Empty<Repository>();
}