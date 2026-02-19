using NAudio.Wave;
using TerminalWave.Entities;

namespace TerminalWave.Services;

public class PlayerService : IPlayerService, IDisposable
{
    private WaveOutEvent? _outputDevice;
    private AudioFileReader? _audioFile;
    
    public PlaybackState State => _outputDevice?.PlaybackState ?? PlaybackState.Stopped;

    public void PlayMusic(MusicEntity music)
    {
        StopMusic();

        try
        {
            _audioFile = new AudioFileReader(music.MusicPath);
            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Music player error: {ex.Message}");
        }
    }

    public void PauseMusic() => _outputDevice?.Pause();

    public void ResumeMusic() => _outputDevice?.Play();

    public void StopMusic()
    {
        _outputDevice?.Stop();
        _outputDevice?.Dispose();
        _outputDevice = null;

        _audioFile?.Dispose();
        _audioFile = null;
    }

    public void NextTrack() { }
    public void PreviousTrack() { }

    public void Dispose()
    {
        StopMusic();
    }
}