namespace TerminalWave.Entities;

public class MusicEntity
{
    public string FileName { get; set; } = string.Empty;
    public string Artist { get ; set; } = string.Empty;
    public string Title { get ; set; } = string.Empty;
    public TimeSpan MusicLength { get; set; }
    public string MusicPath { get; set; } = string.Empty;
}