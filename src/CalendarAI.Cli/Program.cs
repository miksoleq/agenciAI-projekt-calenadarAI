using CalendarAI.Agent.Llm;
using CalendarAI.Agent.Prompts;
using CalendarAI.Agent.Tools;
using CalendarAI.Cli;
using CalendarAI.Cli.Fakes;
using CalendarAI.Core.Abstractions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

// ===== TEMPORARY COMPOSITION ROOT =====
// This file connects the existing layers for an end-to-end demo.
// The CLI mode will eventually replace this demo; agent and core code stay unchanged.

// Load non-sensitive LLM settings from appsettings.json.
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var llmOptions = (config.GetSection("Llm").Get<LlmOptions>() ?? new LlmOptions()) with
{
    // The API key must stay outside appsettings.json.
    ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
};

// This clock supplies the current date to the system prompt and app mode.
IClock clock = new SystemClock();

// TEMPORARY: one in-memory calendar instance is shared by app and agent modes.
// When a JSON/other CalendarService is added, replace only this object.
ICalendarService calendar = new InMemoryCalendarService();

// Create the selected provider: OpenAI or Ollama.
IChatClient chatClient = ChatClientFactory.Create(llmOptions);

// Adapt the shared calendar service to the LLM tools add_event and list_events.
var calendarTools = new CalendarTools(calendar);

// Build the system prompt with the current date and time.
var promptBuilder = new SystemPromptBuilder(clock);
var chatOptions = new ChatOptions
{
    Tools = [.. calendarTools.AsTools()]
};

// Conversation history belongs to AgentMode and is kept between agent messages.
var history = new List<ChatMessage>
{
    new(ChatRole.System, promptBuilder.Build())
};

// Composition of the two modes.
// AgentMode receives the chat client and tools; AppMode receives the calendar service.
var agentMode = new AgentMode(chatClient, chatOptions, history);
var appMode = new AppMode(calendar, agentMode, clock);

Console.WriteLine(
    $"CalendarAI demo ({llmOptions.Provider}/{llmOptions.Model}). " +
    "Type 'help' for available commands.");

// AppMode owns the top-level loop. /back inside AgentMode returns control here.
await appMode.RunAsync();
