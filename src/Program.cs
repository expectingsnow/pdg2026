Console.WriteLine("Hello, World!");

var directoryData = Path.Combine(Directory.GetCurrentDirectory(), "data");

List<string> links =
[
    "https://api-live.pdg.ch/replay/races/Z1/keyframe?timestamp_lte=1776350873",
    "https://api-live.pdg.ch/replay/races/A1/keyframe?timestamp_lte=1776350049",
    "https://api-live.pdg.ch/replay/races/Z2/keyframe?timestamp_lte=1776519533",
    "https://api-live.pdg.ch/replay/races/A2/keyframe?timestamp_lte=1776522750"
];

Directory.CreateDirectory(directoryData);

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
