using System.Reflection;

namespace TerminalWave.Services;

public class ArtistService : IArtistService
{
    private readonly Dictionary<string, (string ResourceName, ConsoleColor Color)> _artMap = new(StringComparer.OrdinalIgnoreCase);

    public ArtistService()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();

        foreach (var resource in resources)
        {
            if (resource.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = resource.Split('.').Reverse().Skip(1).First();
                string artistName;
                ConsoleColor artColor = ConsoleColor.DarkYellow;

                if (fileName.Contains('-'))
                {
                    var parts = fileName.Split('-');
                    artistName = parts[0].Replace("_", " ");
                    if (Enum.TryParse<ConsoleColor>(parts[1], true, out var parsedColor)) artColor = parsedColor;
                }
                else artistName = fileName.Replace("_", " ");

                _artMap[artistName] = (resource, artColor);
            }
        }
    }

    public bool HasArt(string artistName) => !string.IsNullOrEmpty(artistName) && _artMap.ContainsKey(artistName);

    public ArtistArtResult GetArtistArt(string artistName)
    {
        if (string.IsNullOrEmpty(artistName) || !_artMap.TryGetValue(artistName, out var info))
            return new ArtistArtResult(Array.Empty<string>(), ConsoleColor.DarkYellow);

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(info.ResourceName);
            if (stream == null) return new ArtistArtResult(Array.Empty<string>(), info.Color);

            using var reader = new StreamReader(stream);
            var lines = new List<string>();
            while (!reader.EndOfStream) lines.Add(reader.ReadLine() ?? "");
            return new ArtistArtResult(lines.ToArray(), info.Color);
        }
        catch { return new ArtistArtResult(Array.Empty<string>(), info.Color); }
    }
}