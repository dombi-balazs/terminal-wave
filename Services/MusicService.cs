using TerminalWave.Entities;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TerminalWave.Services;

public class MusicService : IMusicService
{
    public IEnumerable<MusicEntity> GetMusicFiles()
    {
        string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        if (!Directory.Exists(musicFolder)) return Enumerable.Empty<MusicEntity>();

        return Directory.EnumerateFiles(musicFolder, "*.mp3", SearchOption.AllDirectories)
            .Select(file => new MusicEntity
            {
                MusicName = Path.GetFileNameWithoutExtension(file),
                MusicPath = file,
                MusicLength = TimeSpan.Zero
            }).ToList();
    }
}