using System.ServiceModel.Syndication;

namespace Discord_Bot;

public class RssComparer
{
    private const string PersistencyFileLocation = "./lastId";
    private string? _lastKnownId;
    public string? LastKnownId => _lastKnownId;

    public RssComparer()
    {
        try
        {
            _lastKnownId = File.ReadAllText(PersistencyFileLocation);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
            File.Create(PersistencyFileLocation).Close();
            _lastKnownId = null;
        }
    }
    
    public bool IsFeedNew(SyndicationFeed currentFeed)
    {
        if (_lastKnownId is null)
        {
            _lastKnownId = currentFeed.Items.First().Id;
            File.WriteAllText(PersistencyFileLocation, _lastKnownId);
            return true;
        }

        if (_lastKnownId != currentFeed.Items.FirstOrDefault()?.Id)
        {
            _lastKnownId = currentFeed.Items.FirstOrDefault()?.Id;
            File.WriteAllText(PersistencyFileLocation, _lastKnownId);
            return true;
        }
        
        _lastKnownId = currentFeed.Items.FirstOrDefault()?.Id;
        File.WriteAllText(PersistencyFileLocation, _lastKnownId);
        return false;
    }
}