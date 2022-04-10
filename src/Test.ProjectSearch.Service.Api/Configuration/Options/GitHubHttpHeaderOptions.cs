/// <summary>
/// Опции для работы с GitHub Api.
/// </summary>
public class GitHubHttpHeaderOptions
{
    /// <summary>
    /// Расположения в фале конфигурации.
    /// </summary>
    public const string Position = "GitHubHeaders";

    /// <summary>
    /// Список имен заголовков, которые будут перенаправлены в нижестоящую службу.
    /// </summary>
    public IHeaderDictionary ForwardedHeaders { get; } = new HeaderDictionary();

    /// <summary>
    /// Добавить заголовок.
    /// </summary>
    /// <param name="name">Имя заголовка.</param>
    /// <param name="value">Значения заголовка.</param>
    public void AddForwardHeader(string name, string value)
     => ForwardedHeaders.Add(name, value);
}