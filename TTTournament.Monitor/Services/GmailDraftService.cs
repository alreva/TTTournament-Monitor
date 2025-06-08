using Google.Apis.Gmail.v1;
using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public class GmailDraftService : IGmailService
{
    private readonly GmailService _gmail;
    private readonly ILogger<GmailDraftService> _logger;

    public GmailDraftService(GmailService gmail, ILogger<GmailDraftService> logger)
    {
        _gmail = gmail;
        _logger = logger;
    }

    public async Task CreateDraftAsync(TournamentAnnouncement announcement, CancellationToken token)
    {
        _logger.LogInformation("Would create Gmail draft for {Name}", announcement.TournamentName);
        await Task.CompletedTask;
    }
}
