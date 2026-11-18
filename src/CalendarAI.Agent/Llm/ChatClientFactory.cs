using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;

namespace CalendarAI.Agent.Llm;

public static class ChatClientFactory
{
    public static IChatClient Create(LlmOptions options)
    {
        IChatClient inner = options.Provider switch
        {
            LlmProvider.Ollama => CreateOllama(options),
            LlmProvider.OpenAI => CreateOpenAI(options),
            _ => throw new NotSupportedException($"Unknown LLM provider: {options.Provider}") 
        };

        return new ChatClientBuilder(inner)
            .UseFunctionInvocation()
            .Build();
    }

    private static IChatClient CreateOllama(LlmOptions o) =>
        new OllamaApiClient(new Uri(o.Endpoint ?? "http://localhost:11434"), o.Model);

    private static IChatClient CreateOpenAI(LlmOptions o)
    {
        if (string.IsNullOrWhiteSpace(o.ApiKey))
            throw new InvalidOperationException("OPENAI_API_KEY is not set.");

        return new OpenAIClient(o.ApiKey)
            .GetChatClient(o.Model)
            .AsIChatClient();
    }
}