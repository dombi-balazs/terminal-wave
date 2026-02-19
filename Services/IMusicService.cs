using TerminalWave.Entities;
using NAudio.Wave;

namespace TerminalWave.Services;

public interface IMusicService
{
    IEnumerable<MusicEntity> GetMusicFiles();
}