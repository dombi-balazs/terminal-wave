using TerminalWave.Services;
using TerminalWave.Entities;
using System;
using System.Linq;

namespace TerminalWave;

internal class Program
{
    static void Main(string[] args)
    {
        var musicService = new MusicService();
        using var playerService = new PlayerService();
        var songs = musicService.GetMusicFiles().ToList();

        if (songs.Count == 0)
        {
            Console.WriteLine("Couldn't find music");
            return;
        }

        var testSong = songs[0];

        Console.WriteLine("Starting test program");
        Console.WriteLine($"Song: {testSong.MusicName}");
        
        Console.WriteLine("Calling PlayMusic() function, music should be heard");
        playerService.PlayMusic(testSong);

        Console.WriteLine("Press any button to stop");
        Console.ReadKey();

        Console.WriteLine("StopMusic() function is called, music should stop.");
        playerService.StopMusic();

        Console.WriteLine("Press any key to close the test");
        Console.ReadKey();

        Console.WriteLine("End of the test.");
    }
}