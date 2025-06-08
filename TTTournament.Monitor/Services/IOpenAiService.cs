using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public interface IOpenAiService
{
    Task<TournamentAnnouncement?> ParseAnnouncementAsync(string text, CancellationToken token);
}
