public record RootObject(Dictionary<string, KeyFrame> keyframe);

public record Category(string name, string parent_key);

public record StartBlock(string key);

public record Athlete(string name, int birth_year, string nationality);

public record CheckPoint(string key, string time);

public record KeyFrame(
    string bib,
    string name,
    Category category,
    StartBlock start_block,
    Athlete[] athletes,
    string status, 
    string time,
    int rank,
    int crank,
    CheckPoint[] checkpoints
)
{
    public TimeSpan? TimeAtCheckPoint(string checkPointKey)
    {
        var checkPoint = checkpoints.FirstOrDefault(c => c.key == checkPointKey);
        
        return TryParseIsoDuration(checkPoint?.time, out var timeSpan) ? timeSpan : null;
    }
    
    public TimeSpan? TimeFromCheckPointToCheckPoint(string checkPointKeyFrom, string checkPointKeyTo)
    {
        var timeFrom = TimeAtCheckPoint(checkPointKeyFrom); if (timeFrom is null) { return null; }
        var timeTo   = TimeAtCheckPoint(checkPointKeyTo  ); if (timeTo   is null) { return null; }

        return timeTo - timeFrom;
    }
    
    public static bool TryParseIsoDuration(string? value, out TimeSpan? timeSpan)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            timeSpan = null;
            return false;
        }

        try
        {
            timeSpan = System.Xml.XmlConvert.ToTimeSpan(value);
            return true;
        }
        catch (FormatException)
        {
            timeSpan = null;
            return false;
        }
    }
};
