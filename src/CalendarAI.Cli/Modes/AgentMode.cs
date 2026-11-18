using Microsoft.Extensions.AI;

public sealed class AgentMode : IMode
{
    private readonly IChatClient _chatClient;
    private readonly ChatOptions _chatOptions;
    private readonly List<ChatMessage> _history;

    public AgentMode(
        IChatClient chatClient,
        ChatOptions ChatOptions,
        List<ChatMessage> history
    )
    {
        _chatClient = chatClient;
        _chatOptions = ChatOptions;
        _history = history;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Write("chat> ");
            var input = Console.ReadLine();

            if (input is null || input.Trim() == "/back")
                return;

            if (string.IsNullOrWhiteSpace(input))
                continue;

            _history.Add(new ChatMessage(ChatRole.User, input));

            var response = await _chatClient.GetResponseAsync(_history, _chatOptions);

            _history.AddMessages(response);

            Console.WriteLine($"agent> {response.Text}");
        }
    }
}