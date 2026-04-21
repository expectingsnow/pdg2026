public static class Helper
{
    public static TimeSpan? TimeAtCheckPoint(CheckPoint[] checkPoints, string checkPointKey)
    {
        var checkPoint = checkPoints.FirstOrDefault(c => c.key == checkPointKey);
        
        return TryParseIsoDuration(checkPoint?.time, out var timeSpan) ? timeSpan : null;
    }
    
    public static TimeSpan? TimeSpanFromCheckPointToCheckPoint(CheckPoint[] checkPoints, string checkPointKeyFrom, string checkPointKeyTo)
    {
        var timeFrom = TimeAtCheckPoint(checkPoints, checkPointKeyFrom); if (timeFrom is null) { return null; }
        var timeTo   = TimeAtCheckPoint(checkPoints, checkPointKeyTo  ); if (timeTo   is null) { return null; }

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
}