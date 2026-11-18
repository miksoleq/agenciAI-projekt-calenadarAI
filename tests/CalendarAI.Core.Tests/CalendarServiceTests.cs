using CalendarAI.Core.Model;
using CalendarAI.Core.Services;
using CalendarAI.Core.Storage;

namespace CalendarAI.Core.Tests;

public sealed class CalendarServiceTests : IDisposable
{
    private static readonly DateTimeOffset T0 = new(2026, 11, 6, 18, 0, 0, TimeSpan.FromHours(1));

    private readonly string _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private string FilePath => Path.Combine(_dir, "calendar.json");

    private CalendarService NewService() => new(new JsonStateStore(FilePath));

    public void Dispose()
    {
        if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
    }

    [Fact]
    public void AddedEvent_SurvivesRestart()
    {
        var added = NewService().AddEvent("Trening", T0, T0.AddHours(1));

        var afterRestart = NewService();   // nowy store i nowy serwis = nowe uruchomienie aplikacji
        var events = afterRestart.ListEvents(T0.AddDays(-1), T0.AddDays(1));

        var ev = Assert.Single(events);
        Assert.Equal(added, ev);
    }

    [Fact]
    public void Load_MissingFile_ReturnsEmptyState()
    {
        var state = new JsonStateStore(FilePath).Load();

        Assert.Equal(AppState.CurrentVersion, state.Version);
        Assert.Empty(state.Events);
    }

    [Fact]
    public void AddEvent_EndNotAfterStart_Throws()
    {
        var service = NewService();

        Assert.Throws<ArgumentException>(() => service.AddEvent("X", T0, T0));
        Assert.Throws<ArgumentException>(() => service.AddEvent("X", T0, T0.AddMinutes(-5)));
    }

    [Fact]
    public void ListEvents_FiltersByRange_AndSortsByStart()
    {
        var service = NewService();
        service.AddEvent("Późno", T0.AddHours(3), T0.AddHours(4));
        service.AddEvent("Wcześnie", T0, T0.AddHours(1));
        service.AddEvent("Poza zakresem", T0.AddDays(5), T0.AddDays(5).AddHours(1));

        var result = service.ListEvents(T0, T0.AddHours(5));

        Assert.Equal(["Wcześnie", "Późno"], result.Select(e => e.Title));
    }
}