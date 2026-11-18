using System.Globalization;
using CalendarAI.Core.Abstractions;

namespace CalendarAI.Cli.Commands;

public sealed class ListCommand(ICalendarService calendar, IClock clock, TextWriter output) : ICommand
{
    public string Name => "list";
    public string Help => "list [od] [do] – pokazuje wydarzenia (daty rrrr-mm-dd; domyślnie najbliższe 7 dni)";

    public CommandResult Execute(string[] args)
    {
        if (args.Length > 2) throw new FormatException("Użycie: " + Help);

        var from = args.Length >= 1 ? ParseDay(args[0]) : new DateTimeOffset(clock.Now.DateTime.Date);
        var to = args.Length == 2 ? ParseDay(args[1]).AddDays(1) : from.AddDays(7);
        if (to <= from) throw new FormatException("Data końcowa nie może być wcześniejsza niż początkowa.");

        var events = calendar.ListEvents(from, to);
        if (events.Count == 0)
        {
            output.WriteLine("Brak wydarzeń.");
            return CommandResult.Continue;
        }

        foreach (var e in events) output.WriteLine(EventFormatter.Line(e));
        return CommandResult.Continue;
    }

    private static DateTimeOffset ParseDay(string text)
    {
        if (!DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day))
            throw new FormatException($"Niepoprawna data: '{text}' (oczekiwano rrrr-mm-dd).");
        return new DateTimeOffset(day);
    }
}