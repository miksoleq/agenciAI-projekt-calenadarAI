namespace CalendarAI.Core.Model;

/// <summary>
/// Pojedyncze wydarzenie w kalendarzu (zajęcia, trening itp.).
/// </summary>
public sealed record Event(
    Guid Id,
    string Title,
    DateTimeOffset Start,
    DateTimeOffset End);
