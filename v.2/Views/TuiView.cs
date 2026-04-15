using System.Text;
using TerminalWave.Entities;
using TerminalWave.Services;
using TerminalWave.ViewModel;

namespace TerminalWave.Views;

public class TuiView
{
    private readonly PlayerService player;
    private readonly PlayerViewModel vm;

    private bool running = true;
    private int selectionIndex = 0;
    private bool focusBrowser = true;
    private bool showSettings = false;

    // --- ÚJ: Változók a remegés (flickering) megelőzésére ---
    private int _lastSecond = -1;
    private int _lastSelection = -1;
    private bool _needsFullRedraw = true;

    public TuiView(PlayerViewModel vm, PlayerService player)
    {
        this.vm = vm;
        this.player = player;
    }

    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Clear();

        while (running)
        {
            // --- ÚJ: Csak akkor rajzolunk, ha valami tényleg megváltozott ---
            int currentSec = (int)vm.CurrentTime.TotalSeconds;
            if (_needsFullRedraw || currentSec != _lastSecond || selectionIndex != _lastSelection)
            {
                Draw();
                _lastSecond = currentSec;
                _lastSelection = selectionIndex;
                _needsFullRedraw = false;
            }

            HandleInput();
            Thread.Sleep(30); // 30ms-ra csökkentve a gyorsabb gombérzékelésért
        }
    }

    private void HandleInput()
    {
        if (!Console.KeyAvailable) return;

        var key = Console.ReadKey(true).Key;
        int songCount = vm.Songs.Count;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                if (selectionIndex > 0) selectionIndex--;
                break;

            case ConsoleKey.DownArrow:
                if (selectionIndex < songCount - 1) selectionIndex++;
                break;

            case ConsoleKey.Enter:
                if (songCount > 0)
                {
                    var selectedTrack = vm.Songs[selectionIndex];
                    player.PlayMusic(selectedTrack);
                    _needsFullRedraw = true; // Frissítés kérése
                }
                break;

            case ConsoleKey.N:
                vm.Next();
                UpdateSelectionToCurrent();
                _needsFullRedraw = true;
                break;

            case ConsoleKey.P:
                vm.Previous();
                UpdateSelectionToCurrent();
                _needsFullRedraw = true;
                break;

            case ConsoleKey.Spacebar:
                vm.TogglePause();
                _needsFullRedraw = true;
                break;

            case ConsoleKey.Tab:
                focusBrowser = !focusBrowser;
                _needsFullRedraw = true;
                break;

            case ConsoleKey.R: // --- ÚJ: Manuális képernyő frissítés ---
                Console.Clear();
                _needsFullRedraw = true;
                break;

            case ConsoleKey.S:
                showSettings = !showSettings;
                Console.Clear(); // Ha menüt váltunk, érdemes törölni a szemetet
                _needsFullRedraw = true;
                break;

            case ConsoleKey.Q:
                running = false;
                break;
        }
    }

    private void UpdateSelectionToCurrent()
    {
        if (vm.CurrentTrack != null)
        {
            var index = vm.Songs.ToList().FindIndex(s => s.MusicPath == vm.CurrentTrack.MusicPath);
            if (index != -1) selectionIndex = index;
        }
    }

    private void Draw()
    {
        Console.SetCursorPosition(0, 0);
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;

        int browserWidth = (int)(width * 0.4);
        int contentHeight = height - 5;

        DrawSeparators(browserWidth, contentHeight, width);
        DrawMusicList(browserWidth, contentHeight);
        DrawContent(browserWidth, contentHeight, width);
        DrawPlayerBar(width, height);
    }

    private void DrawSeparators(int browserWidth, int contentHeight, int width)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int y = 0; y < contentHeight; y++)
        {
            Console.SetCursorPosition(browserWidth, y);
            Console.Write("│");
        }
        Console.SetCursorPosition(0, contentHeight);
        Console.Write(new string('─', width));
        Console.ResetColor();
    }

    private void DrawMusicList(int width, int maxHeight)
    {
        Console.SetCursorPosition(1, 0);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("▬▬ MUSIC LIBRARY ▬▬".PadRight(width - 2));
        Console.ResetColor();

        int displayOffset = Math.Max(0, selectionIndex - (maxHeight - 5));

        for (int i = 0; i < maxHeight - 2; i++)
        {
            int songIdx = i + displayOffset;
            Console.SetCursorPosition(1, i + 2);

            if (songIdx < vm.Songs.Count)
            {
                var track = vm.Songs[songIdx];
                bool isSelected = (songIdx == selectionIndex);
                bool isPlaying = (vm.CurrentTrack?.MusicPath == track.MusicPath);

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else if (isPlaying)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                string prefix = isPlaying ? "► " : "  ";
                string line = $"{prefix}{track.Artist} - {track.Title}";

                if (line.Length > width - 3) line = line.Substring(0, width - 6) + "...";
                Console.Write(line.PadRight(width - 2));

                Console.ResetColor();
            }
            else
            {
                Console.Write(new string(' ', width - 1));
            }
        }
    }

    private void DrawContent(int offsetX, int contentHeight, int totalWidth)
    {
        int contentWidth = totalWidth - offsetX - 2;
        Console.SetCursorPosition(offsetX + 2, 0);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(showSettings ? "SETTINGS".PadRight(15) : "VISUALIZER".PadRight(15));
        Console.ResetColor();

        if (showSettings)
        {
            DrawSettings(offsetX + 2);
        }
        else
        {
            Random r = new Random();
            for (int i = 0; i < contentHeight - 2; i++)
            {
                Console.SetCursorPosition(offsetX + 4, i + 2);
                int barSize = (player.State == PlaybackState.Playing) ? r.Next(1, contentWidth - 5) : 2;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(new string('█', barSize).PadRight(contentWidth));
            }
        }
    }

    private void DrawSettings(int offsetX)
    {
        Console.SetCursorPosition(offsetX + 2, 4);
        Console.Write("Audio Engine: NAudio".PadRight(30));
        Console.SetCursorPosition(offsetX + 2, 6);
        Console.Write("Theme: Terminal Dark".PadRight(30));
    }

    private void DrawPlayerBar(int width, int height)
    {
        int y = height - 4;
        Console.SetCursorPosition(2, y);

        if (vm.CurrentTrack != null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"NOW PLAYING: {vm.CurrentTrack.Artist} - {vm.CurrentTrack.Title}".PadRight(width - 4));
        }
        else
        {
            Console.Write("Stopped".PadRight(width - 4));
        }

        DrawProgressBar(width, y + 1);

        Console.SetCursorPosition(2, y + 3);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        // --- ÚJ: Hozzáadtam a menühöz az R betűt ---
        Console.Write("ENTER Play | SPACE Pause | N Next | P Prev | R Refresh | S Settings | Q Quit".PadRight(width - 4));
        Console.ResetColor();
    }

    private void DrawProgressBar(int width, int y)
    {
        int barWidth = width - 20;
        Console.SetCursorPosition(2, y);

        if (vm.TotalTime.TotalSeconds <= 0)
        {
            Console.Write("[" + new string('░', barWidth) + "] 00:00 / 00:00");
            return;
        }

        double percent = Math.Clamp(vm.CurrentTime.TotalSeconds / vm.TotalTime.TotalSeconds, 0, 1);
        int filled = (int)(percent * barWidth);

        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('█', filled));
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', barWidth - filled));
        Console.ResetColor();
        Console.Write($"] {vm.CurrentTime:mm\\:ss} / {vm.TotalTime:mm\\:ss}");
    }
}