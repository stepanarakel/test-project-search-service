/// <summary>
/// Интерфейс взаимодействия с github web API.
/// </summary>
public interface IGitHubApiHttpService
{
    /// <summary>
    /// Получить коллекцию репозиториев.
    /// </summary>
    /// <param name="name">Имя репозитория.</param>
    /// <returns>Модель с репозиториями <see cref="SearchResultRequestViewModel"/>.</returns>
    public Task<SearchResultRequestViewModel> SearchProject(string name);
}