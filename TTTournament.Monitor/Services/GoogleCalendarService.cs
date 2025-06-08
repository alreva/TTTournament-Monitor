using Google.Apis.Calendar.v3;
using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public class GoogleCalendarService : ICalendarService
{
    private readonly CalendarService _calendar;
    private readonly ILogger<GoogleCalendarService> _logger;

    public GoogleCalendarService(CalendarService calendar, ILogger<GoogleCalendarService> logger)
    {
        _calendar = calendar;
        _logger = logger;
    }

    public async Task CreateRemindersAsync(TournamentAnnouncement announcement, CancellationToken token)
    {
        _logger.LogInformation("Would create calendar events for {Name}", announcement.TournamentName);
        await Task.CompletedTask;
    }
}
