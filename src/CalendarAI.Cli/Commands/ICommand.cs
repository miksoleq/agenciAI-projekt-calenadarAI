namespace CalendarAI.Cli.Commands;

public enum CommandResult { Continue, Exit }

public interface ICommand
{
    string Name { get; }
    string Help { get; }
    CommandResult Execute(string[] args);
}