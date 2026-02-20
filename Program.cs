using TerminalWave.Services;
using TerminalWave.ViewModel;
using TerminalWave.Entities;
using System.Text;

namespace TerminalWave;

internal class Program
{
    private static int _lastTimeSecond = -1;
    private static int _lastSelectionIndex = -1;
    private static PlaybackState _lastState = PlaybackState.Stopped;
    private static int _lastSongCount = -1;
    private static int _lastWindowWidth = -1;
    private static int _lastWindowHeight = -1;

    static void Main(string[] args)
    {
        var musicService = new MusicService();
        var artistService = new ArtistService(); 
        using var playerService = new PlayerService();
        using var viewmodel = new PlayerViewModel(musicService, playerService, artistService);
        
        int selectionIndex = 0;
        bool isRunning = true;
        Console.CursorVisible = false;
        Console.Clear();

        _lastWindowWidth = Console.WindowWidth;
        _lastWindowHeight = Console.WindowHeight;

        DrawUI(viewmodel, playerService.State, selectionIndex);

        while (isRunning)
        {
            bool needsRedraw = false;

            if (Console.WindowWidth != _lastWindowWidth || Console.WindowHeight != _lastWindowHeight)
            {
                Console.Clear();
                _lastWindowWidth = Console.WindowWidth;
                _lastWindowHeight = Console.WindowHeight;
                needsRedraw = true;
            }

            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                needsRedraw = true;
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow: if (selectionIndex > 0) selectionIndex--; break;
                    case ConsoleKey.DownArrow: if (selectionIndex < viewmodel.Songs.Count - 1) selectionIndex++; break;
                    case ConsoleKey.Enter:
                        if (viewmodel.Songs.Count > 0)
                        {
                            var track = viewmodel.Songs[selectionIndex];
                            playerService.PlayMusic(track);
                            viewmodel.ClearSearch();
                            selectionIndex = viewmodel.Songs.ToList().IndexOf(track);
                            Console.Clear();
                        }
                        break;
                    case ConsoleKey.Spacebar: viewmodel.TogglePause(); break;
                    case ConsoleKey.N: viewmodel.Next(); break;
                    case ConsoleKey.P: viewmodel.Previous(); break;
                    case ConsoleKey.F: viewmodel.FastForward(); break;
                    case ConsoleKey.B: viewmodel.Rewind(); break;
                    case ConsoleKey.R: viewmodel.RefreshSongs(); selectionIndex = 0; Console.Clear(); break;
                    case ConsoleKey.Q: isRunning = false; break;
                    case ConsoleKey.T: string tQ = GetSearchQuery(" SEARCH TITLE: "); viewmodel.SearchTitle(tQ); selectionIndex = 0; Console.Clear(); break;
                    case ConsoleKey.A: string aQ = GetSearchQuery(" SEARCH ARTIST: "); viewmodel.SearchArtist(aQ); selectionIndex = 0; Console.Clear(); break;
                    case ConsoleKey.M: viewmodel.ClearSearch(); selectionIndex = 0; Console.Clear(); break;
                }
                while (Console.KeyAvailable) Console.ReadKey(intercept: true);
            }

            int curSec = (int)viewmodel.CurrentTime.TotalSeconds;
            if (curSec != _lastTimeSecond || playerService.State != _lastState || selectionIndex != _lastSelectionIndex || viewmodel.Songs.Count != _lastSongCount) needsRedraw = true;

            if (needsRedraw)
            {
                DrawUI(viewmodel, playerService.State, selectionIndex);
                _lastTimeSecond = curSec; _lastState = playerService.State; _lastSelectionIndex = selectionIndex; _lastSongCount = viewmodel.Songs.Count;
            }
            Thread.Sleep(50); 
        }
        Console.CursorVisible = true; Console.ResetColor(); Console.Clear();
    }

    static void DrawUI(PlayerViewModel viewmodel, PlaybackState state, int selectionIndex)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        
        int artBoxWidth = width > 90 ? (int)(width * 0.35) : 0;
        int listWidth = width - (artBoxWidth > 0 ? artBoxWidth + 2 : 1);
        int currentLine = 0;

        Console.SetCursorPosition(0, 0);

        void WriteListLine(string text, ConsoleColor fg = ConsoleColor.Gray, ConsoleColor bg = ConsoleColor.Black)
        {
            if (currentLine >= height - 1) return;
            Console.SetCursorPosition(0, currentLine);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            string display = text.Length > listWidth ? text.Substring(0, listWidth) : text.PadRight(listWidth);
            Console.Write(display);
            Console.ResetColor();
            currentLine++;
        }

        WriteListLine("--- TERMINAL WAVE | DASHBOARD ---", ConsoleColor.DarkYellow);
        WriteListLine("[ARROWS] NAV [ENTER] PLAY [SPACE] PAUSE", ConsoleColor.DarkYellow);
        WriteListLine("[N/P] NEXT/PREV [F/B] FF/REW [Q] EXIT", ConsoleColor.DarkYellow);
        WriteListLine("[T/A] TITLE/ARTIST SEARCH [M] MENU", ConsoleColor.DarkYellow);
        
        Console.ForegroundColor = ConsoleColor.White;
        WriteListLine(new string('=', listWidth), ConsoleColor.White);

        if (currentLine < height - 1)
        {
            Console.SetCursorPosition(0, currentLine);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(" STATUS: ");
            Console.ForegroundColor = (state == PlaybackState.Playing) ? ConsoleColor.Cyan : ConsoleColor.Red;
            Console.Write($"{state.ToString().ToUpper(),-10}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($" | {viewmodel.CurrentTime:mm\\:ss}/{viewmodel.TotalTime:mm\\:ss}".PadRight(Math.Max(0, listWidth - 18)));
            currentLine++;
        }

        WriteListLine(new string('-', listWidth), ConsoleColor.White);
        WriteListLine(string.Format("   #   {0,-25} | {1}", "ARTIST", "TITLE"), ConsoleColor.DarkYellow);
        WriteListLine(new string('-', listWidth), ConsoleColor.White);

        string baseMusicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        string[]? currentDirLevels = null;

        for (int i = 0; i < viewmodel.Songs.Count; i++)
        {
            if (currentLine >= height - 1) break;
            var track = viewmodel.Songs[i];
            string trackDir = Path.GetDirectoryName(track.MusicPath) ?? baseMusicFolder;
            string relPath = Path.GetRelativePath(baseMusicFolder, trackDir);
            string[] trackDirLevels = relPath == "." ? Array.Empty<string>() : relPath.Split(Path.DirectorySeparatorChar);

            if (currentDirLevels == null || !currentDirLevels.SequenceEqual(trackDirLevels))
            {
                if (currentDirLevels == null) { WriteListLine("Music", ConsoleColor.Blue); }
                int div = 0;
                if (currentDirLevels != null) while (div < currentDirLevels.Length && div < trackDirLevels.Length && currentDirLevels[div] == trackDirLevels[div]) div++;
                for (int d = div; d < trackDirLevels.Length; d++) { WriteListLine($"{new string('-', d + 1)} {trackDirLevels[d].ToUpper()}", ConsoleColor.Blue); }
                currentDirLevels = trackDirLevels;
            }

            if (currentLine >= height - 1) break;

            bool isSelected = (i == selectionIndex);
            bool isPlaying = (track.MusicPath == viewmodel.CurrentTrack?.MusicPath);

            ConsoleColor fg = ConsoleColor.Gray;
            ConsoleColor bg = ConsoleColor.Black;

            if (isPlaying) 
            { 
                bg = ConsoleColor.Cyan; 
                fg = isSelected ? ConsoleColor.White : ConsoleColor.Black; 
            }
            else if (isSelected) 
            {
                fg = ConsoleColor.Cyan; 
            }

            string artist = track.Artist.Length > 25 ? track.Artist.Substring(0, 22) + "..." : track.Artist;
            string trackRow = string.Format("{0}{1,3}. {2,-25} | {3}", isPlaying ? " > " : "   ", i + 1, artist, track.Title);
            
            WriteListLine(trackRow, fg, bg);

            if (isPlaying && currentLine < height - 1)
            {
                DrawDashboardProgress(viewmodel.CurrentTime, viewmodel.TotalTime, listWidth, currentLine);
                currentLine++;
            }
        }

        while (currentLine < height - 1) WriteListLine("");

        if (artBoxWidth > 5)
        {
            var artData = viewmodel.GetCurrentArtistArt();
            string[] scaledArt = ScaleArt(artData.Lines, artBoxWidth, height - 7);
            int artStartCol = listWidth + 2;
            
            for (int i = 0; i < height - 2; i++)
            {
                Console.SetCursorPosition(artStartCol, i + 1);
                if (i < scaledArt.Length)
                {
                    Console.ForegroundColor = artData.Color;
                    Console.Write(scaledArt[i].PadRight(artBoxWidth));
                }
                else
                {
                    Console.Write(new string(' ', artBoxWidth));
                }
            }
        }
        Console.ResetColor();
    }

    static string[] ScaleArt(string[] lines, int maxWidth, int maxHeight)
    {
        if (lines == null || lines.Length == 0 || maxWidth <= 0 || maxHeight <= 0) return Array.Empty<string>();
        int sH = lines.Length; int sW = lines.Max(l => l.Length);
        double sc = Math.Min((double)maxWidth / sW, (double)maxHeight / sH);
        if (sc >= 1.0 && sH <= maxHeight) return lines;
        int tH = Math.Max(1, (int)(sH * sc)); int tW = Math.Max(1, (int)(sW * sc));
        var res = new List<string>();
        for (int y = 0; y < tH; y++) {
            int sY = (int)(y / sc); if (sY >= lines.Length) sY = lines.Length - 1;
            string sL = lines[sY]; var sb = new StringBuilder();
            for (int x = 0; x < tW; x++) { int sX = (int)(x / sc); sb.Append(sX < sL.Length ? sL[sX] : ' '); }
            res.Add(sb.ToString());
        }
        return res.ToArray();
    }

    static void DrawDashboardProgress(TimeSpan current, TimeSpan total, int width, int line)
    {
        if (total.TotalSeconds <= 0) return;
        Console.SetCursorPosition(0, line);
        double progress = Math.Clamp(current.TotalSeconds / total.TotalSeconds, 0, 1);
        int barWidth = Math.Max(5, width - 15);
        int filled = (int)(barWidth * progress);
        Console.ForegroundColor = ConsoleColor.Green;
        string prefix = "    [";
        Console.Write(prefix);
        Console.Write(new string('█', filled)); 
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', barWidth - filled)); 
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("]");
        int currentPos = prefix.Length + barWidth + 1;
        if (currentPos < width) Console.Write(new string(' ', width - currentPos));
        Console.ResetColor();
    }

    static string GetSearchQuery(string prompt)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.BackgroundColor = ConsoleColor.DarkYellow; Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(prompt.PadRight(Console.WindowWidth));
        Console.SetCursorPosition(prompt.Length, Console.WindowHeight - 1);
        Console.CursorVisible = true;
        string? q = Console.ReadLine();
        Console.CursorVisible = false;
        Console.ResetColor();
        return q ?? string.Empty;
    }
}