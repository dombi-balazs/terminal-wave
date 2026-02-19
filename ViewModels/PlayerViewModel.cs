using TerminalWave.Entities;
using TerminalWave.Services;
using NAudio.Wave;
using System.Linq;

namespace TerminalWave.ViewModel;

class PlayerViewModel : IDisposable
{
    private readonly IMusicService _musicService;
    private readonly IPlayerService _playerService;
    private bool _disposed = false;
    
    public IReadOnlyList<MusicEntity> Songs { get; private set; } = new List<MusicEntity>();
    public MusicEntity? CurrentTrack => _playerService.CurrentTrack;
    public TimeSpan CurrentTime => _playerService.CurrentTime;
    public TimeSpan TotalTime => _playerService.TotalTime;
    public int CurrentIndex => _playerService.CurrentIndex;

    public PlayerViewModel(IMusicService musicService, IPlayerService playerService)
    {
        _musicService = musicService;
        _playerService = playerService;
        _playerService.TrackFinished += OnTrackFinished;
        LoadSongs();
    }

    private void LoadSongs()
    {
        Songs = _musicService.GetMusicFiles().ToList();
        _playerService.LoadPlaylist(Songs);
    }

    private void OnTrackFinished() { }

    public void TogglePause()
    {
        var track = CurrentTrack;
        if (track == null) return;

        if (_playerService.State == PlaybackState.Playing)
            _playerService.PauseMusic();
        else if (_playerService.State == PlaybackState.Paused)
            _playerService.ResumeMusic();
        else
            _playerService.PlayMusic(track);
    }

    public void Next() => _playerService.NextTrack();
    public void Previous() => _playerService.PreviousTrack();
    public void FastForward() => _playerService.FastForward();
    public void Rewind() => _playerService.Rewind();

    public void Dispose()
    {
        if (_disposed) return;
        _playerService.TrackFinished -= OnTrackFinished;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}