using System.ServiceModel.Syndication;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace Discord_Bot;

public class ArticleSender
{
    public event Func<LogMessage, Task>? Log;

    private const ulong ServerId = 261125968628809729;

    private readonly DiscordSocketClient _client;
    private readonly RssComparer _rssComparer;
    private readonly RssParser _rssParser;

    private bool _isFirstSend = true;

    public ArticleSender(DiscordSocketClient client)
    {
        _client = client;
        _rssComparer = new RssComparer();
        _rssParser = new RssParser();
    }

    public void SendArticles(SyndicationFeed rssFeed)
    {
        LogActionInfo("Begin rss feed comparison");

        var lastIdOfPreviousFeed = _rssComparer.LastKnownId;
        if (lastIdOfPreviousFeed is null)
            _isFirstSend = true;

        if (!_rssComparer.IsFeedNew(rssFeed))
        {
            LogActionInfo("Rss feed is not new -> returning");
            return;
        }
        LogActionInfo("Feed is new -> parsing feed");

        var messages = _rssParser.ParseRssFeed(rssFeed, lastIdOfPreviousFeed, _isFirstSend);
        LogActionInfo($"Parsed {messages.Count} articles");
        
        LogActionInfo("Sending parsed articles");
        messages.ForEach(m => _client.GetGuild(ServerId).GetTextChannel(ServerId).SendMessageAsync(m));

        _isFirstSend = false;
    }

    private void LogActionInfo(string message)
    {
        Log(new LogMessage(LogSeverity.Info, "Article", message));
    }
}