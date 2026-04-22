public record RootObject(
    string id,
    int timestamp,
    Dictionary<string, KeyFrame> keyframe
);

public record KeyFrame(
    int id,
    string timing_id,
    string bib,
    string name,
    bool is_private,
    Category category,
    Start_block start_block,
    Athletes[] athletes,
    string status,
    string start,
    string time,
    int rank,
    int crank,
    int trend,
    double lat,
    double lon,
    object loc_time,
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

public record Category(
    int id,
    int parent,
    string key,
    string name,
    string parent_key
);

public record Start_block(
    int id,
    int race,
    string key,
    string start_date
);

public record Athletes(
    int id,
    int team,
    string timing_id,
    string name,
    int birth_year,
    string nationality,
    int order,
    bool is_lead,
    bool is_standby
);

public record CheckPoint(
    int id,
    string key,
    string time,
    string status,
    int rank,
    int crank
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