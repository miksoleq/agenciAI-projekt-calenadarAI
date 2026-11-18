using CalendarAI.Cli.Commands;

namespace CalendarAI.Cli;

public sealed class Repl(CommandDispatcher dispatcher, TextReader input, TextWriter output)
{
    public void Run()
    {
        output.WriteLine("calendarAI – wpisz 'help', aby zobaczyć komendy.");
        while (true)
        {
            output.Write("> ");
            var line = input.ReadLine();
            if (line is null) break;   // koniec wejścia (Ctrl+Z / Ctrl+D)
            if (dispatcher.Dispatch(line) == CommandResult.Exit) break;
        }
    }
}