using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Bot;

public class Program
{
    private const string Url = "https://habr.com/ru/rss/all/all/?fl=ru";
    private const long PollingPeriod = TimeSpan.TicksPerMinute * 1;

    private DiscordSocketClient _client;
    private CommandHandler _commandHandler;

    private RssReader _rssReader;
    private RssDelegator _rssDelegator;

    public static Task Main(string[] args) => new Program().MainAsync();

    private async Task MainAsync()
    {
        //Register client
        _client = new DiscordSocketClient();

        //Register RSS
        _rssReader = new(Url);
        _rssDelegator = new(_client);

        _rssDelegator.Log += Log;

        _client.Log += Log;
        _client.Ready += WaitForNewPosts;

        _commandHandler = new CommandHandler(_client, new CommandService());
        await _commandHandler.InstallCommandsAsync();

        var token = await File.ReadAllTextAsync("./token.txt");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private Task WaitForNewPosts()
    {
        ScheduleActionIndefinitely(TimeSpan.FromTicks(PollingPeriod), CheckFeedAndNotify);
        return Task.CompletedTask;
    }

    private async Task ScheduleActionIndefinitely(TimeSpan period, Action action)
    {
        var timer = new PeriodicTimer(period);
        while (await timer.WaitForNextTickAsync())
        {
            action();
        }
    }

    private void CheckFeedAndNotify()
    {
        var feed = _rssReader.ReadRssFeed();
        _rssDelegator.DelegateRss(feed);
    }
}