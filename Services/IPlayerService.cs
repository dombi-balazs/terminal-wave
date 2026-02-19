using TerminalWave.Entities;

namespace TerminalWave.Services;

public interface IPlayerService
{
    void PlayMusic(MusicEntity music);
    void PauseMusic();
    void NextTrack();
    void PreviousTrack();
}