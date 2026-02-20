using TerminalWave.Entities;

namespace TerminalWave.Services;

public class MusicService : IMusicService
{
    private static readonly string[] AllowedExtensions = { ".mp3", ".wav", ".flac" };

    public IEnumerable<MusicEntity> GetMusicFiles()
    {
        string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        if (!Directory.Exists(musicFolder)) return Enumerable.Empty<MusicEntity>();

        return Directory.EnumerateFiles(musicFolder, "*.*", SearchOption.AllDirectories)
            .Where(file =>
            {
                string extension = Path.GetExtension(file).ToLowerInvariant();
                return AllowedExtensions.Contains(extension);
            })
            .OrderBy(file => file)
            .Select(file =>
            {
                string fileName = Path.GetFileName(file);
                string title = Path.GetFileNameWithoutExtension(file);
                string artist = "Unknown Artist";
                TimeSpan duration = TimeSpan.Zero;

                try
                {
                    using var tagFile = TagLib.File.Create(file);
                    duration = tagFile.Properties.Duration;

                    if (!string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                    {
                        title = tagFile.Tag.Title;
                    }

                    if (!string.IsNullOrWhiteSpace(tagFile.Tag.FirstPerformer))
                    {
                        artist = tagFile.Tag.FirstPerformer;
                    }
                }
                catch
                {
                }

                return new MusicEntity
                {
                    FileName = fileName,
                    Artist = artist,
                    Title = title,
                    MusicPath = file,
                    MusicLength = duration
                };
            }).ToList();
    }
}