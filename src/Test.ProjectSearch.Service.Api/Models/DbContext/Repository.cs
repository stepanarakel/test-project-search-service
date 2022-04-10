/// <summary>
/// Модель репозитория.
/// </summary>
public class Repository
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Владелец.
    /// </summary>
    public Owner Owner { get; set; }

    /// <summary>
    /// Количество звёзд.
    /// </summary>
    public int StargazersCount { get; set; }

    /// <summary>
    /// Количество наблюдателей.
    /// </summary>
    public int WatchersCount { get; set; }

    /// <summary>
    /// Ссылка на проект.
    /// </summary>
    public string Url { get; set; }
}