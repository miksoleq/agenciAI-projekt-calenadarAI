using System.Globalization;
using CalendarAI.Core.Model;

namespace CalendarAI.Cli.Commands;

internal static class EventFormatter
{
    private const string Format = "yyyy-MM-dd HH:mm";

    public static string Line(Event e)
    {
        var inv = CultureInfo.InvariantCulture;
        var start = e.Start.ToLocalTime();
        var end = e.End.ToLocalTime();
        var endText = end.Date == start.Date ? end.ToString("HH:mm", inv) : end.ToString(Format, inv);
        return $"{start.ToString(Format, inv)}–{endText}  {e.Title}";
    }
}