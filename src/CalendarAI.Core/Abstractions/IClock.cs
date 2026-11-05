namespace CalendarAI.Core.Abstractions;

/// <summary>
/// Źródło aktualnego czasu (w testach podmieniane na stały czas).
/// </summary>
public interface IClock
{
    DateTimeOffset Now { get; }
}
