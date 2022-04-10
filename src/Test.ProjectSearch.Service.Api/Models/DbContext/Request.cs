using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Модель запроса.
/// </summary>
public class Request
{
    /// <summary>
    /// Идентификатор запроса.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Текст запроса.
    /// </summary>
    public string RequestText { get; set; }

    /// <summary>
    /// Результат запроса.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public SearchResult Result { get; set; }

    /// <summary>
    /// Время запроса.
    /// </summary>
    public Instant ReauestInstant { get; set; } = new();
}