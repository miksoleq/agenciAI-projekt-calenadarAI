using CalendarAI.Core.Abstractions;

namespace CalendarAI.Agent.Prompts;

public sealed class SystemPromptBuilder(IClock clock)
{
    public string Build()
    {
        var now = clock.Now;
        // TODO
        // https://github.com/miksoleq/agenciAI-projekt-calenadarAI/issues/29
        return
        $"""
        You are a calendar assistant.
        Current date and time: {now:yyyy-MM-dd HH:mm zzz} ({now.DayOfWeek}).
        Use the add_event and list_events tools to manage the user's calendar.
        Always pass dates in ISO 8601 format with offset {now:zzz}.
        If the user gives no end time, assume the event lasts 1 hour.
        If the request is not about the calendar, politely refuse in one sentence.
        """;
    }
}

