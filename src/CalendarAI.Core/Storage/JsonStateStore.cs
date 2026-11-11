using System.Text.Json;
using CalendarAI.Core.Abstractions;
using CalendarAI.Core.Model;

namespace CalendarAI.Core.Storage;

public sealed class JsonStateStore(string path) : IStateStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public AppState Load()
    {
        if (!File.Exists(path)) return AppState.Empty;
        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<AppState>(stream, Options) ?? AppState.Empty;
    }

    public void Save(AppState state)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        Directory.CreateDirectory(dir);

        var tmp = path + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(state, Options));
        File.Move(tmp, path, overwrite: true);
    }
}