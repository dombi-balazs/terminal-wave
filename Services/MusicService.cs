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
            .Select(file => 
            {
                string title = Path.GetFileNameWithoutExtension(file);
                TimeSpan duration = TimeSpan.Zero;

                try
                {
                    using var tagFile = TagLib.File.Create(file);
                    duration = tagFile.Properties.Duration;
                    
                    if (!string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                    {
                        string artist = tagFile.Tag.FirstPerformer;
                        title = string.IsNullOrEmpty(artist) ? tagFile.Tag.Title : $"{artist} - {tagFile.Tag.Title}";
                    }
                }
                catch
                {
                    
                }

                return new MusicEntity
                {
                    MusicName = title,
                    MusicPath = file,
                    MusicLength = duration
                };
            }).ToList();
    }
}