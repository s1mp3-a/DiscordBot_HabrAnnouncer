using System.ServiceModel.Syndication;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace Discord_Bot;

public class RssDelegator
{
    public event Func<LogMessage, Task> Log;

    private const ulong Id = 261125968628809729;

    private DiscordSocketClient _client;
    private RssComparer _rssComparer;

    private bool _firstSend = true;

    public RssDelegator(DiscordSocketClient client)
    {
        _client = client;
        _rssComparer = new RssComparer();
    }

    public void DelegateRss(SyndicationFeed rssFeed)
    {
        Log(new LogMessage(LogSeverity.Info, "RssDelegator", "Begin rss delegation"));

        var lastIdOfPreviousFeed = _rssComparer.PrevFeed?.Items.FirstOrDefault()?.Id;
        if (lastIdOfPreviousFeed is null)
            _firstSend = true;

        if (!_rssComparer.IsFeedNew(rssFeed))
        {
            Log(new LogMessage(LogSeverity.Info, "RssDelegator", "Rss feed is not new -> returning"));
            return;
        }

        var builder = new StringBuilder();
        Log(new LogMessage(LogSeverity.Info, "RssDelegator", "Begin output construction"));
        foreach (var item in rssFeed.Items)
        {
            Log(new LogMessage(LogSeverity.Info, "RssDelegator", $"Current id: {item.Id}    prev id: {lastIdOfPreviousFeed}"));
            if (!_firstSend)
            {
                if (item.Id == lastIdOfPreviousFeed)
                {
                    Log(new LogMessage(LogSeverity.Info, "RssDelegator", "Same Id reached -> returning"));
                    break;
                }
            }

            var tags = item.Categories.Select(sc => sc.Name).Take(3).Select(t => $"[{t}]");

            builder.AppendLine("**Новая статья на хабре!**");
            builder.AppendLine("```");
            builder.AppendLine($"Тэги: {tags.Aggregate((a, b) => $"{a}  {b}")}");
            builder.Append("```");
            builder.Append(item.Id);

            _client.GetGuild(Id).GetTextChannel(Id).SendMessageAsync(builder.ToString());

            builder.Clear();
        }

        _firstSend = false;
    }
}