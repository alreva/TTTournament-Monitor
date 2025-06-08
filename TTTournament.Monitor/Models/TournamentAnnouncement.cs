namespace TTTournament.Monitor.Models;

/// <summary>
/// Represents key details extracted from a tournament announcement.
/// </summary>
public class TournamentAnnouncement
{
    public string? TournamentName { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Venue { get; set; }
    public string? OrganizerEmail { get; set; }
    public string? RegistrationDeadline { get; set; }
    public string? Categories { get; set; }
    public string? SourceUrl { get; set; }
}
