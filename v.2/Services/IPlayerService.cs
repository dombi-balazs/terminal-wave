using TerminalWave.Entities;

namespace TerminalWave.Services;

public interface IPlayerService
{
    void LoadPlaylist(IEnumerable<MusicEntity> playlist);
    void UpdatePlaylist(IEnumerable<MusicEntity> newPlaylist);
    void PlayMusic(MusicEntity music);
    void PauseMusic();
    void ResumeMusic();
    void StopMusic();
    void NextTrack();
    void PreviousTrack();
    void FastForward();
    void Rewind();

    TimeSpan CurrentTime { get; }
    TimeSpan TotalTime { get; }
    MusicEntity? CurrentTrack { get; }
    int CurrentIndex { get; }
    PlaybackState State { get; }
    event Action TrackFinished;
}