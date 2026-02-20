using TerminalWave.Services;
using TerminalWave.ViewModel;
using TerminalWave.Entities;

namespace TerminalWave;

internal class Program
{
    static void Main(string[] args)
    {
        var musicService = new MusicService();
        using var playerService = new PlayerService();
        using var viewmodel = new PlayerViewModel(musicService, playerService);

        bool isRunning = true;
        Console.CursorVisible = false;
        Console.Clear();

        while (isRunning)
        {
            DrawUI(viewmodel, playerService.State);

            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Spacebar: viewmodel.TogglePause(); break;
                    case ConsoleKey.N: viewmodel.Next(); break;
                    case ConsoleKey.P: viewmodel.Previous(); break;
                    case ConsoleKey.F: viewmodel.FastForward(); break;
                    case ConsoleKey.B: viewmodel.Rewind(); break;
                    case ConsoleKey.Q: isRunning = false; break;
                }
                while (Console.KeyAvailable) Console.ReadKey(intercept: true);
            }
            Thread.Sleep(50);
        }

        Console.CursorVisible = true;
        Console.ResetColor();
        Console.Clear();
    }

    static void DrawUI(PlayerViewModel viewmodel, PlaybackState state)
    {
        Console.SetCursorPosition(0, 0);
        int width = Console.WindowWidth;
        int maxHeight = Console.WindowHeight - 1;
        int currentLine = 0;

        void WriteLineSafe(string text)
        {
            if (currentLine < maxHeight)
            {
                Console.WriteLine(text.Length > width ? text.Substring(0, width) : text.PadRight(width));
                currentLine++;
            }
        }

        WriteLineSafe("=== TerminalWave ===");
        WriteLineSafe("[Space] Play/Pause | [N] Next | [P] Prev | [F] +5s | [B] -5s | [Q] Exit");
        WriteLineSafe(new string('-', width));
        WriteLineSafe($"Status: {state} | Time: {viewmodel.CurrentTime:mm\\:ss} / {viewmodel.TotalTime:mm\\:ss}");
        WriteLineSafe("");

        for (int i = 0; i < viewmodel.Songs.Count; i++)
        {
            if (currentLine >= maxHeight) break;

            if (i == viewmodel.CurrentIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string trackInfo = $"> {i + 1}. {viewmodel.Songs[i].MusicName}";
                if (trackInfo.Length > 40) trackInfo = trackInfo.Substring(0, 37) + "...";
                Console.Write(trackInfo.PadRight(40));
                DrawProgressBar(viewmodel.CurrentTime, viewmodel.TotalTime);
                Console.WriteLine("".PadRight(Math.Max(0, width - Console.CursorLeft)));
                Console.ResetColor();
                currentLine++;
            }
            else
            {
                WriteLineSafe($"  {i + 1}. {viewmodel.Songs[i].MusicName}");
            }
        }
        while (currentLine < maxHeight) WriteLineSafe("");
    }

    static void DrawProgressBar(TimeSpan current, TimeSpan total)
    {
        if (total.TotalSeconds <= 0) return;
        int barWidth = 20;
        double progress = Math.Clamp(current.TotalSeconds / total.TotalSeconds, 0, 1);
        int filledLength = (int)(barWidth * progress);
        Console.Write($" [{new string('#', filledLength)}{new string('-', barWidth - filledLength)}] {(int)(progress * 100)}%");
    }
}