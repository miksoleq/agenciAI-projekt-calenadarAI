namespace CalendarAI.Cli.Commands;

public sealed class HelpCommand(IReadOnlyList<ICommand> commands, TextWriter output) : ICommand
{
    public string Name => "help";
    public string Help => "help – wypisuje dostępne komendy";

    public CommandResult Execute(string[] args)
    {
        foreach (var command in commands) output.WriteLine($"  {command.Help}");
        return CommandResult.Continue;
    }
}