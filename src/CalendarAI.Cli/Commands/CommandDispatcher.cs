namespace CalendarAI.Cli.Commands;

public sealed class CommandDispatcher(IEnumerable<ICommand> commands, TextWriter output)
{
    private readonly Dictionary<string, ICommand> _commands =
        commands.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

    public CommandResult Dispatch(string line)
    {
        var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0) return CommandResult.Continue;

        if (!_commands.TryGetValue(tokens[0], out var command))
        {
            output.WriteLine($"Nieznana komenda: {tokens[0]}. Wpisz 'help'.");
            return CommandResult.Continue;
        }

        try
        {
            return command.Execute(tokens[1..]);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            output.WriteLine($"Błąd: {ex.Message}");
            return CommandResult.Continue;
        }
    }
}