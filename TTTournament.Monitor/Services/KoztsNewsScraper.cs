using System.Text.RegularExpressions;
using HtmlAgilityPack;
using TTTournament.Monitor.Models;

namespace TTTournament.Monitor.Services;

public class KoztsNewsScraper : INewsScraper
{
    private static readonly string NewsUrl = "https://kozts.pl/index.php?option=com_content&task=category&sectionid=1&id=1&Itemid=1";
    private readonly HttpClient _httpClient = new();
    private readonly IPersistenceService _persistence;
    private readonly ILogger<KoztsNewsScraper> _logger;

    public KoztsNewsScraper(IPersistenceService persistence, ILogger<KoztsNewsScraper> logger)
    {
        _persistence = persistence;
        _logger = logger;
    }

    public async Task<IEnumerable<TournamentAnnouncement>> GetNewAnnouncementsAsync(CancellationToken token)
    {
        var list = new List<TournamentAnnouncement>();
        string html;
        try
        {
            html = await _httpClient.GetStringAsync(NewsUrl, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download news page");
            return list;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var links = doc.DocumentNode.SelectNodes("//a[contains(@href,'option=com_content')]") ?? Enumerable.Empty<HtmlNode>();
        foreach (var link in links)
        {
            var href = link.GetAttributeValue("href", string.Empty);
            var title = link.InnerText.Trim();
            if (string.IsNullOrEmpty(href) || string.IsNullOrEmpty(title))
                continue;
            if (!ContainsKeywords(title))
                continue;

            var postId = ExtractId(href);
            if (postId is null)
                continue;
            if (await _persistence.IsProcessedAsync(postId, token))
                continue;

            list.Add(new TournamentAnnouncement
            {
                TournamentName = title,
                SourceUrl = href.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? href : $"https://kozts.pl/{href.TrimStart('/')}"
            });
        }
        return list;
    }

    private static bool ContainsKeywords(string text)
    {
        var lower = text.ToLowerInvariant();
        return lower.Contains("turniej") || lower.Contains("festiwal") || lower.Contains("tenisa");
    }

    private static string? ExtractId(string url)
    {
        var match = Regex.Match(url, "[?&]id=(\\d+)");
        return match.Success ? match.Groups[1].Value : null;
    }
}
