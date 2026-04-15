using TerminalWave.Entities;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace TerminalWave.Services;

public class PlayerService : IPlayerService, IDisposable
{
    private MediaPlayer? _player;
    private List<MusicEntity> _playlist = new();
    private int _currentIndex = -1;
    private bool _isChangingTrack = false;

    public PlaybackState State
    {
        get
        {
            if (_player == null || _player.Source == null) return PlaybackState.Stopped;

            return _player.PlaybackSession.PlaybackState switch
            {
                MediaPlaybackState.Playing => PlaybackState.Playing,
                MediaPlaybackState.Paused => PlaybackState.Paused,
                MediaPlaybackState.Opening => PlaybackState.Buffering,
                MediaPlaybackState.Buffering => PlaybackState.Buffering,
                _ => PlaybackState.Stopped
            };
        }
    }

    public MusicEntity? CurrentTrack => (_currentIndex >= 0 && _currentIndex < _playlist.Count) ? _playlist[_currentIndex] : null;
    public int CurrentIndex => _currentIndex;
    public TimeSpan CurrentTime => _player?.PlaybackSession.Position ?? TimeSpan.Zero;
    public TimeSpan TotalTime => _player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;

    public event Action? TrackFinished;

    public void LoadPlaylist(IEnumerable<MusicEntity> playlist)
    {
        _playlist = playlist.ToList();
        if (_playlist.Any() && _currentIndex == -1) _currentIndex = 0;
    }

    public void PlayMusic(MusicEntity music)
    {
        if (_isChangingTrack) return;
        _isChangingTrack = true;

        try
        {
            if (_player == null)
            {
                _player = new MediaPlayer();
                _player.MediaEnded += OnPlaybackStopped;
            }

            if (_player.Source is IDisposable oldSource)
            {
                oldSource.Dispose();
            }

            var uri = new Uri(Path.GetFullPath(music.MusicPath));
            _player.Source = MediaSource.CreateFromUri(uri);
            _player.Play();
            
            _currentIndex = _playlist.IndexOf(music);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Playback Error: {ex.Message}");
        }
        finally
        {
            _isChangingTrack = false;
        }
    }

    public void NextTrack()
    {
        if (!_playlist.Any()) return;
        int nextIndex = (_currentIndex + 1) % _playlist.Count;
        PlayMusic(_playlist[nextIndex]);
    }

    public void PreviousTrack()
    {
        if (!_playlist.Any()) return;
        int prevIndex = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
        PlayMusic(_playlist[prevIndex]);
    }

    private void OnPlaybackStopped(MediaPlayer sender, object args)
    {
        TrackFinished?.Invoke();
        
       Task.Run(() => NextTrack());
    }

    public void PauseMusic() => _player?.Pause();
    public void ResumeMusic() => _player?.Play();

    public void StopMusic()
    {
        if (_player != null)
        {
            _player.MediaEnded -= OnPlaybackStopped;
            _player.Pause();
            if (_player.Source is IDisposable ds) ds.Dispose();
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
        
        if (newTime >= session.NaturalDuration)
        {
            session.Position = session.NaturalDuration - TimeSpan.FromMilliseconds(500);
            return;
        }

        if (newTime < TimeSpan.Zero) newTime = TimeSpan.Zero;
        session.Position = newTime;
    }

    public void UpdatePlaylist(IEnumerable<MusicEntity> newPlaylist)
    {
        var currentPath = CurrentTrack?.MusicPath;
        _playlist = newPlaylist.ToList();
        
        if (currentPath != null)
        {
            _currentIndex = _playlist.FindIndex(m => m.MusicPath == currentPath);
        }
    }

    public void Dispose() => StopMusic();
}