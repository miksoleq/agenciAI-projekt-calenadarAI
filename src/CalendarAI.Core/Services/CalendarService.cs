using CalendarAI.Core.Abstractions;
using CalendarAI.Core.Model;

namespace CalendarAI.Core.Services;

public sealed class CalendarService(IStateStore store) : ICalendarService
{
    private AppState _state = store.Load();

    public Event AddEvent(string title, DateTimeOffset start, DateTimeOffset end)
    {
        if (end <= start)
            throw new ArgumentException("Koniec musi być późniejszy niż początek.", nameof(end));

        var ev = new Event(Guid.NewGuid(), title, start, end);
        _state = _state with { Events = [.. _state.Events, ev] };
        store.Save(_state);
        return ev;
    }

    public IReadOnlyList<Event> ListEvents(DateTimeOffset from, DateTimeOffset to) =>
        _state.Events
            .Where(e => e.Start < to && e.End > from)
            .OrderBy(e => e.Start)
            .ToList();
}