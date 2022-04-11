/// <summary>
/// Модель представления владельца, полученного из <see cref="IGitHubApiHttpService.SearchProject"/>.
/// </summary>
public class OwnerRequestViewModel
{
    /// <summary>
    /// Логин.
    /// </summary>
    public string Login { get; set; }
}