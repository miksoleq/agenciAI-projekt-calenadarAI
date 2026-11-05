using CalendarAI.Core.Model;

namespace CalendarAI.Core.Abstractions;

/// <summary>
/// Odczyt i zapis całego stanu aplikacji.
/// </summary>
public interface IStateStore
{
    /// <summary>Wczytuje stan; zwraca AppState.Empty, jeśli nic nie zapisano.</summary>
    AppState Load();

    /// <summary>Zapisuje cały stan, nadpisując poprzedni.</summary>
    void Save(AppState state);
}
