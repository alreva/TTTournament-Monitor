using Tesseract;

namespace TTTournament.Monitor.Services;

public class TesseractOcrService : IOcrService
{
    private readonly ILogger<TesseractOcrService> _logger;

    public TesseractOcrService(ILogger<TesseractOcrService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExtractTextAsync(Stream imageStream, CancellationToken token)
    {
        try
        {
            using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromMemory(await ReadAllBytesAsync(imageStream, token));
            using var page = engine.Process(img);
            return page.GetText();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tesseract OCR failed");
            return string.Empty;
        }
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream stream, CancellationToken token)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, token);
        return ms.ToArray();
    }
}
