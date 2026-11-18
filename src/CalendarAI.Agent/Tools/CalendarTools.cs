using System.ComponentModel;
using CalendarAI.Core.Abstractions;
using CalendarAI.Core.Model;
using Microsoft.Extensions.AI;

namespace CalendarAI.Agent.Tools;

/// <summary>
/// Adapter: wystawia ICalendarService jako narzędzia (tools) dla LLM.
/// </summary>
public sealed class CalendarTools(ICalendarService calendar)
{
    // Zwraca listę narzędzi, żeby AgentMode mógł je przekazać w ChatOptions.Tools.
    // AIFunctionFactory czyta metodę przez refleksję (parametry + [Description]) i robi z niej AIFunction.
    public IReadOnlyList<AITool> AsTools() =>
    [
        AIFunctionFactory.Create(AddEvent, name: "add_event"),
        AIFunctionFactory.Create(ListEvents, name: "list_events")
    ];

    [Description("Adds a new event to the user's calendar. Returns the created event or an error message.")]
    private string AddEvent(
        [Description("Short title of the event, e.g. 'Workout'.")] string title,
        [Description("Start time in ISO 8601 with timezone offset, e.g. 2026-11-16T18:00:00+01:00.")] DateTimeOffset start,
        [Description("End time in ISO 8601 with timezone offset. Must be after start.")] DateTimeOffset end)
    {
        // Walidacja jest w CalendarService (S) – tu tylko tłumaczymy wyjątek na tekst dla modelu.
        try
        {
            Event added = calendar.AddEvent(title, start, end);
            return $"Added: {FormatEvent(added)}";
        }
        catch (ArgumentException ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    [Description("Lists calendar events that overlap the given time range [from, to).")]
    private string ListEvents(
        [Description("Range start in ISO 8601 with timezone offset.")] DateTimeOffset from,
        [Description("Range end in ISO 8601 with timezone offset.")] DateTimeOffset to)
    {
        IReadOnlyList<Event> events = calendar.ListEvents(from, to);

        if (events.Count == 0)
            return "No events in this range.";

        return string.Join("\n", events.Select(e => $"- {FormatEvent(e)}"));
    }

    private static string FormatEvent(Event e) =>
        $"{e.Title} | {e.Start:yyyy-MM-dd HH:mm zzz} - {e.End:yyyy-MM-dd HH:mm zzz} | id: {e.Id}";
}
