using NAudio.Wave;
using TerminalWave.Entities;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerminalWave.Services;

public class PlayerService : IPlayerService, IDisposable
{
    private WaveOutEvent? _outputDevice;
    private AudioFileReader? _audioFile;
    private List<MusicEntity> _playlist = new();
    private int _currentIndex = -1;
    private bool _isInternalChange = false;

    public PlaybackState State => _outputDevice?.PlaybackState ?? PlaybackState.Stopped;
    public MusicEntity? CurrentTrack => (_currentIndex >= 0 && _currentIndex < _playlist.Count) ? _playlist[_currentIndex] : null;
    public int CurrentIndex => _currentIndex;
    public TimeSpan CurrentTime => _audioFile?.CurrentTime ?? TimeSpan.Zero;
    public TimeSpan TotalTime => _audioFile?.TotalTime ?? TimeSpan.Zero;
    public event Action? TrackFinished;

    public void LoadPlaylist(IEnumerable<MusicEntity> playlist)
    {
        _playlist = playlist.ToList();
        if (_playlist.Any()) _currentIndex = 0;
    }

    public void PlayMusic(MusicEntity music)
    {
        _isInternalChange = true;
        StopMusic();

        try
        {
            if (!File.Exists(music.MusicPath)) return;

            _audioFile = new AudioFileReader(music.MusicPath);
            _outputDevice = new WaveOutEvent();
            _outputDevice.PlaybackStopped += OnPlaybackStopped;
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();

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

    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        if (_isInternalChange) return;

        if (_audioFile != null && _audioFile.Position >= _audioFile.Length)
        {
            TrackFinished?.Invoke();
            Task.Run(() => NextTrack());
        }
    }

    public void PauseMusic() => _outputDevice?.Pause();
    public void ResumeMusic() => _outputDevice?.Play();

    public void StopMusic()
    {
        if (_outputDevice != null)
        {
            _outputDevice.PlaybackStopped -= OnPlaybackStopped;
            _outputDevice.Stop();
            _outputDevice.Dispose();
            _outputDevice = null;
        }
        if (_audioFile != null)
        {
            _audioFile.Dispose();
            _audioFile = null;
        }
    }

    public void FastForward() => Seek(TimeSpan.FromSeconds(5));
    public void Rewind() => Seek(TimeSpan.FromSeconds(-5));

    private void Seek(TimeSpan offset)
    {
        if (_audioFile == null) return;
        var newTime = _audioFile.CurrentTime + offset;
        if (newTime < TimeSpan.Zero) newTime = TimeSpan.Zero;
        if (newTime > _audioFile.TotalTime) newTime = _audioFile.TotalTime;
        _audioFile.CurrentTime = newTime;
    }

    public void Dispose() => StopMusic();
}