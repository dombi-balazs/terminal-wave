using TerminalWave.Entities;
using TerminalWave.Services;

namespace TerminalWave.ViewModel;

class PlayerViewModel : IDisposable
{
    private readonly IMusicService _musicService;
    private readonly IPlayerService _playerService;
    private readonly IArtistService _artistService;
    private bool _disposed = false;
    private List<MusicEntity> _allSongs = new();
    public IReadOnlyList<MusicEntity> Songs { get; private set; } = new List<MusicEntity>();
    public MusicEntity? CurrentTrack => _playerService.CurrentTrack;
    public TimeSpan CurrentTime => _playerService.CurrentTime;
    public TimeSpan TotalTime => _playerService.TotalTime;
    public int CurrentIndex => _playerService.CurrentIndex;
    public bool IsRefreshing { get; private set; } = false;

    public PlayerViewModel(IMusicService musicService, IPlayerService playerService, IArtistService artistService)
    {
        _musicService = musicService;
        _playerService = playerService;
        _artistService = artistService;
        _playerService.TrackFinished += OnTrackFinished;
        LoadSongs();
    }

    private void LoadSongs()
    {
        _allSongs = _musicService.GetMusicFiles().ToList();
        Songs = _allSongs;
        _playerService.LoadPlaylist(Songs);
    }

    public ArtistArtResult GetCurrentArtistArt()
    {
        if (CurrentTrack == null) return new ArtistArtResult(Array.Empty<string>(), ConsoleColor.DarkYellow);
        return _artistService.GetArtistArt(CurrentTrack.Artist);
    }

    public void SearchTitle(string query)
    {
        Songs = string.IsNullOrWhiteSpace(query) ? _allSongs : _allSongs.Where(s => s.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        _playerService.UpdatePlaylist(Songs);
    }

    public void SearchArtist(string query)
    {
        Songs = string.IsNullOrWhiteSpace(query) ? _allSongs : _allSongs.Where(s => s.Artist.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        _playerService.UpdatePlaylist(Songs);
    }

    public void TogglePause()
    {
        if (CurrentTrack == null) return;
        if (_playerService.State == PlaybackState.Playing) _playerService.PauseMusic();
        else if (_playerService.State == PlaybackState.Paused) _playerService.ResumeMusic();
        else _playerService.PlayMusic(CurrentTrack);
    }

    public void Next() => _playerService.NextTrack();
    public void Previous() => _playerService.PreviousTrack();
    public void FastForward() => _playerService.FastForward();
    public void Rewind() => _playerService.Rewind();
    public void ClearSearch() { Songs = _allSongs; _playerService.UpdatePlaylist(Songs); }

    public async void RefreshSongs()
    {
        if (IsRefreshing) return;
        IsRefreshing = true;
        _allSongs = await Task.Run(() => _musicService.GetMusicFiles().ToList());
        Songs = _allSongs;
        _playerService.UpdatePlaylist(Songs);
        IsRefreshing = false;
    }

    private void OnTrackFinished() { }

    public void Dispose()
    {
        if (_disposed) return;
        _playerService.TrackFinished -= OnTrackFinished;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}