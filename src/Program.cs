using System.Text.Json;

Console.WriteLine("Hello, World!");

var directoryData = Path.Combine(Directory.GetCurrentDirectory(), "data");

Directory.CreateDirectory(directoryData);

#region download

List<string> links =
[
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


    File.WriteAllText(path, content);

       Console.WriteLine($"... downloaded.");
}
#endregion

var files = Directory
                      .GetFiles(directoryData, "*.json")
                      .OrderBy(p => p)
                      .ToArray();

foreach (var file in files)
{
    var content = File.ReadAllText(file);
    var data = JsonSerializer.Deserialize<RootObject>(content);

    if (data == null)
    {
        Console.WriteLine($"Json deserialization failed. [{file}]");
        continue;
    }
    var frames = data.keyframe.Select(p=>p.Value).ToArray();
    
    Console.WriteLine($"{file} - {frames.Length} keyframes");
}