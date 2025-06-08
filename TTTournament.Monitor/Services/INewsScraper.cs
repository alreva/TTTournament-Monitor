using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public interface INewsScraper
{
    Task<IEnumerable<TournamentAnnouncement>> GetNewAnnouncementsAsync(CancellationToken token);
}
