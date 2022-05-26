using System.Xml;
using System.ServiceModel.Syndication;

namespace Discord_Bot;
public class RssReader
{
    private readonly string _url;

    public RssReader(string url)
    {
        _url = url;
    }

    public SyndicationFeed ReadRssFeed()
    {
        using var reader = XmlReader.Create(_url);
        var feed = SyndicationFeed.Load(reader);
        return feed;
    }
}