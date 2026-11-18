namespace CalendarAI.Agent.Llm;

public enum LlmProvider { Ollama, OpenAI }

public sealed record LlmOptions
{
    public LlmProvider Provider
    {
        get;
        init;
    } = LlmProvider.Ollama;

    public string Model { get; init; } = "qwen2.5";

    public string? Endpoint { get; init; }
    public string? ApiKey { get; init; }
}