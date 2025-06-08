namespace TTTournament.Monitor.Services;

public interface IOcrService
{
    Task<string> ExtractTextAsync(Stream imageStream, CancellationToken token);
}
