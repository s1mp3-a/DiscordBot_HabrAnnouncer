using System.ServiceModel.Syndication;
using System.Text;

namespace Discord_Bot;

public class RssParser
{
    public List<string> ParseRssFeed(SyndicationFeed rssFeed, string? lastIdOfPreviousFeed, bool isFirstSend)
    {
        List<string> articlesText = new List<string>();
            
        var builder = new StringBuilder();
        foreach (var item in rssFeed.Items)
        {
            if (!isFirstSend)
            {
                if (item.Id == lastIdOfPreviousFeed)
                {
                    break;
                }
            }

            var tags = item.Categories.Select(sc => sc.Name).Take(3).Select(t => $"[{t}]");

            builder.AppendLine("**Новая статья на хабре!**");
            builder.AppendLine("```");
            builder.AppendLine($"Тэги: {tags.Aggregate((a, b) => $"{a}  {b}")}");
            builder.Append("```");
            builder.Append(item.Id);
            
            articlesText.Add(builder.ToString());
            
            builder.Clear();
        }

        return articlesText;
    }
}