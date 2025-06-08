namespace TTTournament.Monitor.Services;

public interface IPersistenceService
{
    Task<bool> IsProcessedAsync(string postId, CancellationToken token);
    Task MarkProcessedAsync(string postId, CancellationToken token);
}
