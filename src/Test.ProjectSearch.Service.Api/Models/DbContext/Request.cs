using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Primitives;
using NpgsqlTypes;

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
    public Instant RequestInstant { get; set; } = SystemClock.Instance.GetCurrentInstant();
}