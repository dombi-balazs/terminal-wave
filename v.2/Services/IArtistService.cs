namespace TerminalWave.Services;

public interface IArtistService
{
    ArtistArtResult GetArtistArt(string artistName);
    bool HasArt(string artistName);
}