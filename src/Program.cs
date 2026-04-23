using System.Text.Json;

Console.WriteLine("start");

#region setup
var directoryData = Path.Combine(Directory.GetCurrentDirectory(), "data");
var pathOut = Path.Combine(directoryData, "pdg2026.csv");

Directory.CreateDirectory(directoryData);
#endregion

#region download
Console.WriteLine("--------------------");
Console.WriteLine("get data");

List<string> links = [
    "https://api-live.pdg.ch/replay/races/Z1/keyframe?timestamp_lte=1776350873",
    "https://api-live.pdg.ch/replay/races/A1/keyframe?timestamp_lte=1776350049",
    "https://api-live.pdg.ch/replay/races/Z2/keyframe?timestamp_lte=1776519533",
    "https://api-live.pdg.ch/replay/races/A2/keyframe?timestamp_lte=1776522750"
];


using var client = new HttpClient();

foreach (var link in links)
{
    Console.Write(link);

    var path = Path.Combine(
        directoryData, 
        link.Replace(":", "_")
            .Replace("/", "_")
            .Replace("?", "_")
            .Replace("=", "_")
            .Replace("-", "_")
            .Replace(".", "_") + ".json");

    if(File.Exists(path))
    {
        Console.WriteLine($"... skipping");
        continue;
    }

    var response = await client.GetAsync(link);
    var content = await response.Content.ReadAsStringAsync();

    var contentIntended = JsonSerializer.Serialize(
        JsonSerializer.Deserialize<object>(content), new  JsonSerializerOptions { WriteIndented = true }
    );
    
    File.WriteAllText(path, contentIntended);

    Console.WriteLine($"... downloaded.");
}
#endregion

#region load and transform
Console.WriteLine("--------------------");
Console.WriteLine("transform data");
var files = Directory
                      .GetFiles(directoryData, "*.json")
                      .OrderBy(p => p)
                      .ToArray();

List<object> records = [];

foreach (var file in files)
{
    var content = File.ReadAllText(file, System.Text.Encoding.UTF8);
    var rootObject = JsonSerializer.Deserialize<RootObject>(content) ?? throw new Exception("Deserialize failed");

    records.AddRange(
        rootObject.keyframe
                    .Select(p => p.Value)
                    .Select(frame => new {
                        bib                = frame.bib,
                        name               = frame.name,
                        categoryName       = frame.category.name,
                        categoryParentKey  = frame.category.parent_key,
                        startBlockKey      = frame.start_block.key,
                        
                        status             = frame.status, 
                        time               = KeyFrame.TryParseIsoDuration(frame.time, out var time) ? time.ToString() : null,
                        rank               = (int?)(frame.rank == 0 ? null : frame.rank),
                        crank              = frame.crank,
                        
                        athlete1Name       = frame.athletes.ElementAtOrDefault(0)?.name       ,
                        athlete1BirthYear  = frame.athletes.ElementAtOrDefault(0)?.birth_year ,
                        athlete1Nationality= frame.athletes.ElementAtOrDefault(0)?.nationality,
                        athlete2Name       = frame.athletes.ElementAtOrDefault(1)?.name       ,
                        athlete2BirthYear  = frame.athletes.ElementAtOrDefault(1)?.birth_year ,
                        athlete2Nationality= frame.athletes.ElementAtOrDefault(1)?.nationality,
                        athlete3Name       = frame.athletes.ElementAtOrDefault(2)?.name       ,
                        athlete3BirthYear  = frame.athletes.ElementAtOrDefault(2)?.birth_year ,
                        athlete3Nationality= frame.athletes.ElementAtOrDefault(2)?.nationality,
                      
                        checkPoint00       = frame.TimeAtCheckPoint(               "S0"        )?.ToString(),
                        checkPoint01       = frame.TimeAtCheckPoint(               "S1"        )?.ToString(),
                        time_01_02         = frame.TimeFromCheckPointToCheckPoint( "S1" , "S2" )?.ToString(),
                        checkPoint02       = frame.TimeAtCheckPoint(               "S2"        )?.ToString(),
                        time_02_03         = frame.TimeFromCheckPointToCheckPoint( "S2" , "S3" )?.ToString(),
                        checkPoint03       = frame.TimeAtCheckPoint(               "S3"        )?.ToString(),
                        time_03_04         = frame.TimeFromCheckPointToCheckPoint( "S3" , "S4" )?.ToString(),
                        checkPoint04       = frame.TimeAtCheckPoint(               "S4"        )?.ToString(),
                        time_04_05         = frame.TimeFromCheckPointToCheckPoint( "S4" , "S5" )?.ToString(),
                        checkPoint05       = frame.TimeAtCheckPoint(               "S5"        )?.ToString(),
                        time_05_06         = frame.TimeFromCheckPointToCheckPoint( "S5" , "S6" )?.ToString(),
                        checkPoint06       = frame.TimeAtCheckPoint(               "S6"        )?.ToString(),
                        time_06_07         = frame.TimeFromCheckPointToCheckPoint( "S6" , "S7" )?.ToString(),
                        checkPoint07       = frame.TimeAtCheckPoint(               "S7"        )?.ToString(),
                        time_07_08         = frame.TimeFromCheckPointToCheckPoint( "S7" , "S8" )?.ToString(),
                        checkPoint08       = frame.TimeAtCheckPoint(               "S8"         )?.ToString(),
                        time_08_09         = frame.TimeFromCheckPointToCheckPoint( "S8" , "S9" )?.ToString(),
                        checkPoint09       = frame.TimeAtCheckPoint(               "S9"        )?.ToString(),
                        time_09_10         = frame.TimeFromCheckPointToCheckPoint( "S9" , "S10")?.ToString(),
                        checkPoint10       = frame.TimeAtCheckPoint(               "S10"       )?.ToString(),
                        time_10_11         = frame.TimeFromCheckPointToCheckPoint( "S10", "S11")?.ToString(),
                        checkPoint11       = frame.TimeAtCheckPoint(               "S11"       )?.ToString(),
                        time_11_12         = frame.TimeFromCheckPointToCheckPoint( "S11", "S12")?.ToString(),
                        checkPoint12       = frame.TimeAtCheckPoint(               "S12"       )?.ToString()
                    }
        )
    );
}
#endregion

#region dump
await using var writer = new StreamWriter(pathOut, append: false, System.Text.Encoding.UTF8);
await using var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
await csv.WriteRecordsAsync(records);

Console.WriteLine("--------------------");
Console.WriteLine("save data");
Console.WriteLine("--------------------");
#endregion

Console.WriteLine("done");

