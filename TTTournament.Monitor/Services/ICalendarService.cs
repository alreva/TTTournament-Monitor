using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public interface ICalendarService
{
    Task CreateRemindersAsync(TournamentAnnouncement announcement, CancellationToken token);
}
