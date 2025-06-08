using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public interface IGmailService
{
    Task CreateDraftAsync(TournamentAnnouncement announcement, CancellationToken token);
}
