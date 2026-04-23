using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;

Console.WriteLine("start");

var directoryData = Path.Combine(Directory.GetCurrentDirectory(), "data");

Directory.CreateDirectory(directoryData);

var pathOut = Path.Combine(directoryData, "pdg2026.csv");

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

List<Datum> records = [];

foreach (var file in files)
{
    var content = File.ReadAllText(file, Encoding.UTF8);
    var rootObject = JsonSerializer.Deserialize<RootObject>(content) ?? throw new Exception("Deserialize failed");

    records.AddRange(
        rootObject.keyframe.Select(p => p.Value).Select(frame => new Datum(
                bib                : frame.bib,
                name               : frame.name,
                categoryName        : frame.category.name,
                categoryParentKey  : frame.category.parent_key,
                startBlockKey      : frame.start_block.key,
                
                status             : frame.status, 
                time               : KeyFrame.TryParseIsoDuration(frame.time, out var time) ? time.ToString() : null,
                rank               : frame.rank == 0 ? null : frame.rank,
                crank              : frame.crank,
                
                athlete1Name       : frame.athletes.Length > 0 ? frame.athletes[0].name        : null,
                athlete1BirthYear  : frame.athletes.Length > 0 ? frame.athletes[0].birth_year  : null,
                athlete1Nationality: frame.athletes.Length > 0 ? frame.athletes[0].nationality : null,
                athlete2Name       : frame.athletes.Length > 1 ? frame.athletes[1].name        : null,
                athlete2BirthYear  : frame.athletes.Length > 1 ? frame.athletes[1].birth_year  : null,
                athlete2Nationality: frame.athletes.Length > 1 ? frame.athletes[1].nationality : null,
                athlete3Name       : frame.athletes.Length > 2 ? frame.athletes[2].name        : null,
                athlete3BirthYear  : frame.athletes.Length > 2 ? frame.athletes[2].birth_year  : null,
                athlete3Nationality: frame.athletes.Length > 2 ? frame.athletes[2].nationality : null,
              
                checkPoint00       : frame.TimeAtCheckPoint("S0" )?.ToString() ?? string.Empty,
                checkPoint01       : frame.TimeAtCheckPoint("S1" )?.ToString() ?? string.Empty,
                checkPoint02       : frame.TimeAtCheckPoint("S2" )?.ToString() ?? string.Empty,
                checkPoint03       : frame.TimeAtCheckPoint("S3" )?.ToString() ?? string.Empty,
                checkPoint04       : frame.TimeAtCheckPoint("S4" )?.ToString() ?? string.Empty,
                checkPoint05       : frame.TimeAtCheckPoint("S5" )?.ToString() ?? string.Empty,
                checkPoint06       : frame.TimeAtCheckPoint("S6" )?.ToString() ?? string.Empty,
                checkPoint07       : frame.TimeAtCheckPoint("S7" )?.ToString() ?? string.Empty,
                checkPoint08       : frame.TimeAtCheckPoint("S8" )?.ToString() ?? string.Empty,
                checkPoint09       : frame.TimeAtCheckPoint("S9" )?.ToString() ?? string.Empty,
                checkPoint10       : frame.TimeAtCheckPoint("S10")?.ToString() ?? string.Empty,
                checkPoint11       : frame.TimeAtCheckPoint("S11")?.ToString() ?? string.Empty,
                checkPoint12       : frame.TimeAtCheckPoint("S12")?.ToString() ?? string.Empty,

                time_01_02         : frame.TimeFromCheckPointToCheckPoint( "S1" , "S2" )?.ToString() ?? string.Empty,
                time_02_03         : frame.TimeFromCheckPointToCheckPoint( "S2" , "S3" )?.ToString() ?? string.Empty,
                time_03_04         : frame.TimeFromCheckPointToCheckPoint( "S3" , "S4" )?.ToString() ?? string.Empty,
                time_04_05         : frame.TimeFromCheckPointToCheckPoint( "S4" , "S5" )?.ToString() ?? string.Empty,
                time_05_06         : frame.TimeFromCheckPointToCheckPoint( "S5" , "S6" )?.ToString() ?? string.Empty,
                time_06_07         : frame.TimeFromCheckPointToCheckPoint( "S6" , "S7" )?.ToString() ?? string.Empty,
                time_07_08         : frame.TimeFromCheckPointToCheckPoint( "S7" , "S8" )?.ToString() ?? string.Empty,
                time_08_09         : frame.TimeFromCheckPointToCheckPoint( "S8" , "S9" )?.ToString() ?? string.Empty,
                time_09_10         : frame.TimeFromCheckPointToCheckPoint( "S9" , "S10")?.ToString() ?? string.Empty,
                time_10_11         : frame.TimeFromCheckPointToCheckPoint( "S10", "S11")?.ToString() ?? string.Empty,
                time_11_12         : frame.TimeFromCheckPointToCheckPoint( "S11", "S12")?.ToString() ?? string.Empty
            )
        )
    );
}
#endregion

#region dump
await using var writer = new StreamWriter(pathOut, append: false, Encoding.UTF8);
await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
await csv.WriteRecordsAsync(records.OrderBy(p => int.Parse(p.bib)));
#endregion
Console.WriteLine("--------------------");
Console.WriteLine("save data");
Console.WriteLine("--------------------");
Console.WriteLine("done");

