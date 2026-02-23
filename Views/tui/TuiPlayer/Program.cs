using System;
using System.Collections.Generic;
using System.Threading;

Console.CursorVisible = false;
Console.Clear();

var app = new TuiApp();
app.Run();

enum FocusArea
{
    Browser,
    Content
}

class TuiApp
{
    private UiState state = new UiState();
    private bool running = true;

    public void Run()
    {
        while (running)
        {
            Renderer.Draw(state);
            HandleInput();
            Thread.Sleep(80);
        }
    }

    private void HandleInput()
    {
        if (!Console.KeyAvailable) return;

        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                state.MoveSelection(-1);
                break;

            case ConsoleKey.DownArrow:
                state.MoveSelection(1);
                break;

            case ConsoleKey.Tab:
                state.ToggleFocus();
                break;

            case ConsoleKey.Spacebar:
                state.IsPlaying = !state.IsPlaying;
                break;

            case ConsoleKey.Q:
                running = false;
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = true;
                break;
        }
    }
}

static class Renderer
{
    public static void Draw(UiState state)
    {
        int w = Console.WindowWidth;
        int h = Console.WindowHeight;

        int browserW = w / 4;
        int contentH = h - 4;

        Console.SetCursorPosition(0, 0);
        Console.Clear();

        DrawSeparators(browserW, contentH, w);
        DrawBrowser(state, browserW);
        DrawContent(state, browserW, contentH);
        DrawControls(state, h, w);
    }

    private static void DrawSeparators(int bw, int ch, int w)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;

        for (int y = 0; y < ch; y++)
        {
            Console.SetCursorPosition(bw, y);
            Console.Write("│");
        }

        Console.SetCursorPosition(0, ch);
        Console.Write(new string('─', w));

        Console.ResetColor();
    }

    private static void DrawBrowser(UiState state, int width)
    {
        Console.SetCursorPosition(1, 0);
        Console.ForegroundColor = state.Focus == FocusArea.Browser ? ConsoleColor.Cyan : ConsoleColor.Gray;
        Console.Write("Browser");
        Console.ResetColor();

        for (int i = 0; i < state.BrowserItems.Count; i++)
        {
            Console.SetCursorPosition(1, i + 2);

            if (i == state.SelectedIndex && state.Focus == FocusArea.Browser)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("> " + state.BrowserItems[i]);
                Console.ResetColor();
            }
            else
            {
                Console.Write("  " + state.BrowserItems[i]);
            }
        }
    }

    private static void DrawContent(UiState state, int offsetX, int height)
    {
        Console.SetCursorPosition(offsetX + 2, 0);
        Console.ForegroundColor = state.Focus == FocusArea.Content ? ConsoleColor.Cyan : ConsoleColor.Gray;
        Console.Write("Playlist / Visualization");
        Console.ResetColor();

        Console.SetCursorPosition(offsetX + 2, height / 2);

        if (state.IsPlaying)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("♪ Playing...");
        }
        else
        {
            Console.Write("Paused");
        }

        Console.ResetColor();
    }

    private static void DrawControls(UiState state, int h, int w)
    {
        Console.SetCursorPosition(0, h - 3);

        DrawProgressBar(state.Progress, w - 20);

        Console.SetCursorPosition(0, h - 2);
        Console.Write("[SPACE] Play/Pause   [TAB] Switch   [Q] Quit");
    }

    private static void DrawProgressBar(int progress, int width)
    {
        int filled = (progress * width) / 100;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("[");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('■', filled));

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('□', width - filled));
        Console.Write("]");

        Console.ResetColor();
    }
}

class UiState
{
    public List<string> BrowserItems { get; } = new()
    {
        "Music",
        "Downloads",
        "Playlists"
    };

    public int SelectedIndex { get; private set; }
    public FocusArea Focus { get; private set; } = FocusArea.Browser;
    public bool IsPlaying { get; set; }
    public int Progress { get; private set; }

    public void MoveSelection(int delta)
    {
        if (Focus != FocusArea.Browser) return;

        SelectedIndex = Math.Clamp(SelectedIndex + delta, 0, BrowserItems.Count - 1);
    }

    public void ToggleFocus()
    {
        Focus = Focus == FocusArea.Browser ? FocusArea.Content : FocusArea.Browser;
    }

    public UiState()
    {
        new Thread(() =>
        {
            while (true)
            {
                if (IsPlaying)
                {
                    Progress = (Progress + 1) % 100;
                }
                Thread.Sleep(200);
            }
        })
        { IsBackground = true }.Start();
    }
}