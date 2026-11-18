using CalendarAI.Core.Abstractions;

// AppMode is the CLI's normal application loop.
// It handles text commands only and delegates calendar operations to ICalendarService.
public sealed class AppMode : IMode
{
    private readonly ICalendarService _calendar;
    private readonly AgentMode _agentMode;
    private readonly IClock _clock;

    public AppMode(
        ICalendarService calendar,
        AgentMode agentMode,
        IClock clock)
    {
        _calendar = calendar;
        _agentMode = agentMode;
        _clock = clock;
    }

    // RunAsync keeps control in app mode until the user exits the application.
    // Entering chat mode temporarily transfers control to AgentMode; /back returns here.
    public async Task RunAsync()
    {
        while (true)
        {
            Console.Write("app> ");
            var input = Console.ReadLine();

            if (input is null)
                return;

            switch (input.Trim().ToLowerInvariant())
            {
                case "exit":
                    return;

                case "chat":
                    await _agentMode.RunAsync();
                    break;

                case "list":
                    PrintEvents();
                    break;

                case "help":
                    PrintHelp();
                    break;

                case "":
                    // Empty input is ignored so the prompt remains easy to read.
                    break;

                default:
                    Console.WriteLine("Unknown command. Type 'help' to see available commands.");
                    break;
            }
        }
    }

    // This method is a presentation adapter: it reads through ICalendarService,
    // but it does not implement add/list logic or save state itself.
    private void PrintEvents()
    {
        var events = _calendar.ListEvents(
            _clock.Now,
            _clock.Now.AddDays(7));

        if (events.Count == 0)
        {
            Console.WriteLine("No events in the next 7 days.");
            return;
        }

        foreach (var calendarEvent in events)
        {
            Console.WriteLine(
                $"- {calendarEvent.Title} | " +
                $"{calendarEvent.Start:yyyy-MM-dd HH:mm zzz} - " +
                $"{calendarEvent.End:yyyy-MM-dd HH:mm zzz}");
        }
    }

    // Help text describes the CLI interface, not the underlying calendar domain.
    private static void PrintHelp()
    {
        Console.WriteLine("chat - enter agent mode");
        Console.WriteLine("list - list upcoming events");
        Console.WriteLine("exit - quit");
    }
}