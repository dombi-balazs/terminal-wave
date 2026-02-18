namespace TerminalWave.Entities;

class MusicEntity
{
    public string MusicName { get; set; } = string.Empty;
    public TimeSpan MusicLength { get; set; }
    public string MusicPath { get; set; } = string.Empty;
}