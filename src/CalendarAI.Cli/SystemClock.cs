using CalendarAI.Core.Abstractions;

namespace CalendarAI.Cli;

/// <summary>
/// Prawdziwy zegar: zwraca aktualny czas systemowy (z lokalną strefą czasową).
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
