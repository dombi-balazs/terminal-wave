using TerminalWave.Entities;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TerminalWave.Services;

public class MusicService : IMusicService
{
    private static readonly string[] AllowedExtensions = { ".mp3", ".wav" };
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
            .Select(file => new MusicEntity
            {
                MusicName = Path.GetFileNameWithoutExtension(file),
                MusicPath = file,
                MusicLength = TimeSpan.Zero
            }).ToList();
    }
}