/// <summary>
/// Модель представления ошибки.
/// </summary>
public class ErrorResponseViewModel
{
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; }

    private ErrorResponseViewModel() { }

    /// <summary>
    /// Инициализировать новый жкземпляр <see cref="ErrorResponseViewModel"/>.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public ErrorResponseViewModel(string message)
        => Message = message;
}