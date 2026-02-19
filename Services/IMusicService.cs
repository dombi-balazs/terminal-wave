using TerminalWave.Entities;

namespace TerminalWave.Services;

public interface IMusicService
{
    IEnumerable<MusicEntity> GetMusicFiles();
}