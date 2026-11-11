using System.Text;
using CalendarAI.Cli;
using CalendarAI.Cli.Commands;
using CalendarAI.Core.Abstractions;
using CalendarAI.Core.Services;
using CalendarAI.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;   // polskie znaki i "–" w konsoli

// Composition root: jedyne miejsce, gdzie powstają konkretne obiekty.
IClock clock = new SystemClock();
ICalendarService calendar = new CalendarService(new JsonStateStore("calendar.json"));  // po A: new CalendarService(new JsonStateStore("calendar.json"))

var input = Console.In;
var output = Console.Out;

var commands = new List<ICommand>
{
    new AddCommand(calendar, output),
    new ListCommand(calendar, clock, output),
    new ExitCommand(),
};
commands.Add(new HelpCommand(commands, output));   // help dostaje tę samą listę, więc widzi też siebie

var dispatcher = new CommandDispatcher(commands, output);
new Repl(dispatcher, input, output).Run();