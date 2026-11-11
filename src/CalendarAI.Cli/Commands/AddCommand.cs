using System.Globalization;
using System.Text.RegularExpressions;
using CalendarAI.Core.Abstractions;

namespace CalendarAI.Cli.Commands;

public sealed class AddCommand(ICalendarService calendar, TextWriter output) : ICommand
{
    private static readonly Regex DurationPattern =
        new(@"^(?:(?<h>\d{1,3})h)?(?:(?<m>\d{1,4})m)?$", RegexOptions.IgnoreCase);

    public string Name => "add";
    public string Help => "add <tytuł> <rrrr-mm-dd> <gg:mm> <czas, np. 1h30m> – dodaje wydarzenie";

    public CommandResult Execute(string[] args)
    {
        if (args.Length < 4) throw new FormatException("Użycie: " + Help);

        var title = string.Join(' ', args[..^3]);
        var start = ParseStart(args[^3], args[^2]);
        var duration = ParseDuration(args[^1]);

        var added = calendar.AddEvent(title, start, start + duration);
        output.WriteLine($"Dodano: {EventFormatter.Line(added)}");
        return CommandResult.Continue;
    }

    private static DateTimeOffset ParseStart(string date, string time)
    {
        if (!DateTime.TryParseExact($"{date} {time}", "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var local))
            throw new FormatException($"Niepoprawna data lub godzina: '{date} {time}' (oczekiwano rrrr-mm-dd gg:mm).");

        return new DateTimeOffset(local);   // czas lokalny użytkownika
    }

    private static TimeSpan ParseDuration(string text)
    {
        var match = DurationPattern.Match(text);
        if (!match.Success)
            throw new FormatException($"Niepoprawny czas trwania: '{text}' (np. 1h, 45m, 1h30m).");

        var hours = match.Groups["h"].Success ? int.Parse(match.Groups["h"].Value) : 0;
        var minutes = match.Groups["m"].Success ? int.Parse(match.Groups["m"].Value) : 0;
        var duration = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);

        if (duration <= TimeSpan.Zero)
            throw new FormatException("Czas trwania musi być dłuższy niż zero.");
        return duration;
    }
}