using HtmlAgilityPack;
using TTTournament.Monitor.Models;
using TTTournament.Monitor.Services;

namespace TTTournament.Monitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly INewsScraper _scraper;
    private readonly IOcrService _ocr;
    private readonly IOpenAiService _openAi;
    private readonly IGmailService _gmail;
    private readonly ICalendarService _calendar;
    private readonly IPersistenceService _persistence;
    private readonly HttpClient _httpClient = new();

    public Worker(ILogger<Worker> logger,
                  INewsScraper scraper,
                  IOcrService ocr,
                  IOpenAiService openAi,
                  IGmailService gmail,
                  ICalendarService calendar,
                  IPersistenceService persistence)
    {
        _logger = logger;
        _scraper = scraper;
        _ocr = ocr;
        _openAi = openAi;
        _gmail = gmail;
        _calendar = calendar;
        _persistence = persistence;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking for new announcements at: {time}", DateTimeOffset.Now);
            var announcements = await _scraper.GetNewAnnouncementsAsync(stoppingToken);
            foreach (var a in announcements)
            {
                await ProcessAnnouncementAsync(a, stoppingToken);
            }
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }

    private async Task ProcessAnnouncementAsync(TournamentAnnouncement announcement, CancellationToken token)
    {
        if (announcement.SourceUrl is null) return;
        var html = await DownloadHtmlAsync(announcement.SourceUrl, token);
        if (string.IsNullOrEmpty(html)) return;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var textParts = new List<string>();
        foreach (var p in doc.DocumentNode.SelectNodes("//p") ?? Enumerable.Empty<HtmlNode>())
        {
            textParts.Add(p.InnerText.Trim());
        }
        var imgUrl = doc.DocumentNode.SelectSingleNode("//img")?.GetAttributeValue("src", null);
        if (imgUrl is not null)
        {
            try
            {
                using var stream = await _httpClient.GetStreamAsync(imgUrl, token);
                var ocrText = await _ocr.ExtractTextAsync(stream, token);
                textParts.Add(ocrText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to OCR image");
            }
        }

        var combined = string.Join("\n", textParts);
        var parsed = await _openAi.ParseAnnouncementAsync(combined, token);
        if (parsed is not null)
        {
            parsed.SourceUrl = announcement.SourceUrl;
            await _gmail.CreateDraftAsync(parsed, token);
            await _calendar.CreateRemindersAsync(parsed, token);
            await _persistence.MarkProcessedAsync(announcement.SourceUrl, token);
        }
    }

    private async Task<string> DownloadHtmlAsync(string url, CancellationToken token)
    {
        try
        {
            return await _httpClient.GetStringAsync(url, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download {Url}", url);
            return string.Empty;
        }
    }
}
