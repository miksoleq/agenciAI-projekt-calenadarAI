using CalendarAI.Core.Model;

namespace CalendarAI.Core.Abstractions;

/// <summary>
/// Logika kalendarza – jedyne miejsce, z którego korzystają tryb app i tryb LLM.
/// </summary>
public interface ICalendarService
{
    /// <summary>Dodaje wydarzenie i zapisuje stan. Rzuca ArgumentException, gdy end &lt;= start.</summary>
    Event AddEvent(string title, DateTimeOffset start, DateTimeOffset end);

    /// <summary>Zwraca wydarzenia nachodzące na przedział [from, to), posortowane po Start.</summary>
    IReadOnlyList<Event> ListEvents(DateTimeOffset from, DateTimeOffset to);
}
