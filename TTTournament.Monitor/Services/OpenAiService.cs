using OpenAI_API;
using OpenAI_API.Chat;
using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public class OpenAiService : IOpenAiService
{
    private readonly OpenAIAPI _api;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(ILogger<OpenAiService> logger)
    {
        _logger = logger;
        var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
        _api = new OpenAIAPI(key);
    }

    public async Task<TournamentAnnouncement?> ParseAnnouncementAsync(string text, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        var chat = _api.Chat.CreateConversation();
        chat.AppendSystemMessage("Extract the tournament name, date, time, venue, organizer email, registration deadline, and categories from this text. Respond in JSON.");
        chat.AppendUserInput(text);

        try
        {
            var response = await chat.GetResponseFromChatbotAsync();
            if (string.IsNullOrWhiteSpace(response)) return null;
            return System.Text.Json.JsonSerializer.Deserialize<TournamentAnnouncement>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI parsing failed");
            return null;
        }
    }
}
