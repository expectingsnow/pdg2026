public record RootObject( string id, int timestamp, Dictionary<string, KeyFrame> keyframe );

public record KeyFrame(
    string bib,
    string name,
    Category category,
    Start_block start_block,
    Athletes[] athletes,
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

public record Category( string name, string parent_key);

public record Start_block(string key);

public record Athletes( string name, int birth_year, string nationality );

public record CheckPoint(
    string key,
    string time
);

public record Datum(
    string bib,
    string name,
    
    string categoryParentKey,
    string categoryName,
    
    string startBlockKey,
    string status, 
    string? time,
    int? rank,
    int crank,
    
    string? athlete1Name,
    int? athlete1BirthYear,
    string? athlete1Nationality,
    string? athlete2Name,
    int? athlete2BirthYear,
    string? athlete2Nationality,
    string? athlete3Name,
    int? athlete3BirthYear,
    string? athlete3Nationality,
    
    string checkPoint00,
    string checkPoint01,
    string time_01_02,
    string checkPoint02,
    string time_02_03,
    string checkPoint03,
    string time_03_04,
    string checkPoint04,
    string time_04_05,
    string checkPoint05,
    string time_05_06,
    string checkPoint06,
    string time_06_07,
    string checkPoint07,
    string time_07_08,
    string checkPoint08,
    string time_08_09,
    string checkPoint09,
    string time_09_10,
    string checkPoint10,
    string time_10_11,
    string checkPoint11,
    string time_11_12,
    string checkPoint12
);