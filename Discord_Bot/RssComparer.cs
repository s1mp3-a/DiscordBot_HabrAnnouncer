using System.ServiceModel.Syndication;

namespace Discord_Bot;

public class RssComparer
{
    private SyndicationFeed? _previousFeed;
    private SyndicationFeed? _prePreviousFeed;
    public SyndicationFeed? PrevFeed => _previousFeed;
    public SyndicationFeed? PrePrevFeed => _prePreviousFeed;

    public bool IsFeedNew(SyndicationFeed currentFeed)
    {
        if (_previousFeed is null)
        {
            _prePreviousFeed = _previousFeed;
            _previousFeed = currentFeed;
            return true;
        }

        if (_previousFeed.Items.FirstOrDefault()?.Id
            != currentFeed.Items.FirstOrDefault()?.Id)
        {
            _prePreviousFeed = _previousFeed;
            _previousFeed = currentFeed;
            return true;
        }

        _prePreviousFeed = _previousFeed;
        _previousFeed = currentFeed;
        return false;
    }
}