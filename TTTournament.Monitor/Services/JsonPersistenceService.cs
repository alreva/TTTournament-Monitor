using System.Text.Json;

namespace TTTournament.Monitor.Services;

public class JsonPersistenceService : IPersistenceService
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "processed.json");
    private readonly SemaphoreSlim _lock = new(1,1);
    private HashSet<string> _ids = new();

    public JsonPersistenceService()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _ids = JsonSerializer.Deserialize<HashSet<string>>(json) ?? new();
        }
    }

    public async Task<bool> IsProcessedAsync(string postId, CancellationToken token)
    {
        await _lock.WaitAsync(token);
        try
        {
            return _ids.Contains(postId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task MarkProcessedAsync(string postId, CancellationToken token)
    {
        await _lock.WaitAsync(token);
        try
        {
            if (_ids.Add(postId))
            {
                var json = JsonSerializer.Serialize(_ids, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json, token);
            }
        }
        finally
        {
            _lock.Release();
        }
    }
}
