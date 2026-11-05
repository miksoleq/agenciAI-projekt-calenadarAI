namespace CalendarAI.Core.Model;

/// <summary>
/// Cały stan aplikacji zapisywany do calendar.json.
/// </summary>
public sealed record AppState(
    int Version,
    IReadOnlyList<Event> Events)
{
    public const int CurrentVersion = 1;

    public static AppState Empty { get; } = new(CurrentVersion, []);
}
