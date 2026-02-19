using TerminalWave.Entities;
using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace TerminalWave.Services;

public class MusicService: IMusicService
{
    public IEnumerable<MusicEntity> GetMusicFiles()
    {
        string MusicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        var MusicList = new List<MusicEntity>();
        var Files = Directory.EnumerateFiles(MusicFolder, "*mp3", SearchOption.AllDirectories);
        foreach (var File in Files)
        {
            MusicList.Add(new MusicEntity
            {
                MusicName = Path.GetFileNameWithoutExtension(File),
                MusicPath = File,
                MusicLength = TimeSpan.Zero

            }
            );
        };
        return MusicList;
    }
};