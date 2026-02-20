using TerminalWave.Entities;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace TerminalWave.Services;

public class PlayerService : IPlayerService, IDisposable
{
    private MediaPlayer? _player;
    private List<MusicEntity> _playlist = new();
    private int _currentIndex = -1;
    private bool _isInternalChange = false;

    public PlaybackState State
    {
        get
        {
            if (_player == null) return PlaybackState.Stopped;
            return _player.PlaybackSession.PlaybackState switch
            {
                MediaPlaybackState.Playing => PlaybackState.Playing,
                MediaPlaybackState.Paused => PlaybackState.Paused,
                _ => PlaybackState.Stopped
            };
        }
    }

    public MusicEntity? CurrentTrack => (_currentIndex >= 0 && _currentIndex < _playlist.Count) ? _playlist[_currentIndex] : null;
    public int CurrentIndex => _currentIndex;
    
    public TimeSpan CurrentTime => _player?.PlaybackSession.Position ?? TimeSpan.Zero;
    public TimeSpan TotalTime => CurrentTrack?.MusicLength ?? _player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;

    public event Action? TrackFinished;

    public void LoadPlaylist(IEnumerable<MusicEntity> playlist)
    {
        _playlist = playlist.ToList();
        if (_playlist.Any()) _currentIndex = 0;
    }

    public void PlayMusic(MusicEntity music)
    {
        _isInternalChange = true;
        
        if (_player == null)
        {
            _player = new MediaPlayer();
            _player.MediaEnded += OnPlaybackStopped;
        }

        try
        {
            _player.Source = MediaSource.CreateFromUri(new Uri(music.MusicPath));
            _player.Play();
            _currentIndex = _playlist.IndexOf(music);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
        finally
        {
            _isInternalChange = false;
        }
    }

    public void NextTrack()
    {
        if (!_playlist.Any()) return;
        _currentIndex = (_currentIndex + 1) % _playlist.Count;
        PlayMusic(_playlist[_currentIndex]);
    }

    public void PreviousTrack()
    {
        if (!_playlist.Any()) return;
        _currentIndex = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
        PlayMusic(_playlist[_currentIndex]);
    }

    private void OnPlaybackStopped(MediaPlayer sender, object args)
    {
        if (_isInternalChange) return;
        
        TrackFinished?.Invoke();
        NextTrack();
    }

    public void PauseMusic() => _player?.Pause();
    public void ResumeMusic() => _player?.Play();

    public void StopMusic()
    {
        if (_player != null)
        {
            _player.MediaEnded -= OnPlaybackStopped;
            _player.Pause();
            _player.Source = null;
            _player.Dispose();
            _player = null;
        }
    }

    public void FastForward() => Seek(TimeSpan.FromSeconds(5));
    public void Rewind() => Seek(TimeSpan.FromSeconds(-5));

    private void Seek(TimeSpan offset)
    {
        if (_player == null) return;
        var session = _player.PlaybackSession;
        
        var newTime = session.Position + offset;
        if (newTime < TimeSpan.Zero) newTime = TimeSpan.Zero;
        
        var total = session.NaturalDuration;
        if (total != TimeSpan.Zero && newTime > total) newTime = total;
        
        session.Position = newTime;
    }

    public void Dispose() => StopMusic();
}