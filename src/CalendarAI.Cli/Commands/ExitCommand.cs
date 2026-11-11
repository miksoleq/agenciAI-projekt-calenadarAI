namespace CalendarAI.Cli.Commands;

public sealed class ExitCommand : ICommand
{
    public string Name => "exit";
    public string Help => "exit – kończy program";
    public CommandResult Execute(string[] args) => CommandResult.Exit;
}