using CalendarAI.Core.Abstractions;
using CalendarAI.Core.Model;

namespace CalendarAI.Cli.Fakes;

// MOCK: tymczasowy kalendarz w pamięci (bez zapisu do pliku).
// DO USUNIĘCIA po wprowadzeniu prawdziwego CalendarService w CalendarAI.Core.
public class InMemoryCalendarService : ICalendarService
{
    private readonly List<Event> _events = [];

    public Event AddEvent(string title, DateTimeOffset start, DateTimeOffset end)
    {
        if (end <= start) throw new ArgumentException("End must be after start.");

        var newEvent = new Event(Guid.NewGuid(), title, start, end);
        _events.Add(newEvent);
        Console.WriteLine($"[mock] added {newEvent}");
        return newEvent;
    }

    public IReadOnlyList<Event> ListEvents(DateTimeOffset from, DateTimeOffset to)
    {
        return _events
            .Where(e => e.Start < to && e.End > from)
            .OrderBy(e => e.Start)
            .ToList(); // List<Event> implementuje IReadOnlyList<Event>
    }
}
