using TerminalWave.Services;
using TerminalWave.ViewModel;
using TerminalWave.Views;
using System.Text;

namespace TerminalWave;

internal class Program
{
    static void Main(string[] args)
    {
        
        Console.OutputEncoding = Encoding.UTF8;

        
        var musicService = new MusicService();
        var artistService = new ArtistService();
        using var playerService = new PlayerService();

        
        using var viewmodel = new PlayerViewModel(
            musicService,
            playerService,
            artistService
        );

        
        var ui = new TuiView(viewmodel, playerService);

        
        Console.Clear();

       
        ui.Run();

        
        Console.ResetColor();
        Console.Clear();
        Console.WriteLine("Köszönöm, hogy a TerminalWave-et használtad!");
        Console.CursorVisible = true;
    }
}