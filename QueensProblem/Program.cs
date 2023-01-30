using QueensProblem.Data;

using System.Runtime.Serialization;

namespace QueensProblem;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Select Mode:\n" +
            "[0] Single\n" +
            "[1] Find Max Size");
        var modeRaw = Console.ReadLine();
        if (int.TryParse(modeRaw, out var mode))
        {
            switch(mode)
            {
                case 0:
                    Spacer();
                    Console.WriteLine("Running in Single Mode...");
                    Spacer();
                    _ = RunSingle();
                    break;
                case 1:
                    Spacer();
                    Console.WriteLine("Running in Find Max Mode...");
                    Spacer();
                    RunFindMax();
                    break;
                default:
                    Console.WriteLine("Invalid Case.");
                    Spacer();
                    Main(args);
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid Case.");
            Spacer();
            Main(args);
        }
    }

    static bool RunSingle(int sizeOverride = -1, bool silent = false, bool forcePlacement = true)
    {
        int size;
        bool onlyResults = false;
        if (sizeOverride == -1)
        {
            Console.WriteLine("Input Board Size: ");
            var sizeRaw = Console.ReadLine();

            if (!int.TryParse(sizeRaw, out size))
            {
                Console.WriteLine("Inavlid Input. Enter -1 to exit.");
                return RunSingle();
            }

            if (size == -1)
            {
                Console.WriteLine("Exiting...");
                return false;
            }

            Console.WriteLine("Press 1 to show only iteration results. Press any other key to" +
            "show all placements...");
            var key = Console.ReadKey();

            onlyResults = key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1;
        }
        else
        {
            size = sizeOverride;
        }

        bool success = false;
        for (int i = 0; i < size; i++)
        {
            if (forcePlacement || onlyResults)
            {
                Spacer();
                Console.WriteLine($"Iteration {i}");
                Spacer();
            }

            var board = new Board(size, silent);

            if (!onlyResults)
                board.Print();

            while (!board.IsAtEndState() && board.TryAddQueen(0, i, out var coords))
            {
                if (forcePlacement && !onlyResults)
                {
                    SucessMessage($"Added new Queen at: {coords.Value}");
                    board.Print(true);
                }
            }

            if (board.IsAtEndState())
            {
                Spacer();
                SucessMessage($"End State for size {size}x{size} at iteraation {i}");
                board.Print(true);
                Spacer();

                success = true;
                break;
            }
            else if (onlyResults)
            {
                Spacer();
                Console.WriteLine($"End State for Iteration {i}");
                board.Print(true);
                Spacer();
            }
        }

        return success;
    }

    static void RunFindMax()
    {
        Console.WriteLine("Press 1 to show all successful placements. Press any other key to" +
            "only show completed boards...");
        var key = Console.ReadKey();

        bool placementUI = key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1;
        
        int size = 4;
        var cancelSource = new CancellationTokenSource();
        int lastSuccess = 0;

        try
        {
            var runner = Task.Run(() =>
            {
                while (true)
                {
                    cancelSource.Token.ThrowIfCancellationRequested();

                    var res = RunSingle(size, true, placementUI);

                    if (res)
                    {
                        Spacer();
                        SucessMessage($"Completed Size {size} Board");
                        Spacer();

                        lastSuccess = size;
                        size *= 2;
                    }
                    else
                    {
                        Spacer();
                        ErrorMessage($"Failed Size {size} Board");
                        Spacer();
                        return;
                    }
                }
            }, cancelSource.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Runner Aborted...");
        }

        Console.ReadLine();

        Spacer();
        SucessMessage($"Succeded at a size {lastSuccess} board.");
        Spacer();
    }

    private static void Spacer()
        => Console.WriteLine(new string('-', Console.WindowWidth));

    private static void SucessMessage(string message)
        => Message(message, ConsoleColor.DarkGreen);

    private static void ErrorMessage(string message)
        => Message(message, ConsoleColor.DarkRed);

    private static void Message(string message, ConsoleColor color)
    {
        var tmp = Console.BackgroundColor;
        Console.BackgroundColor = color;
        Console.WriteLine(string.Format($"{{0,{-Console.WindowWidth}}}", message));
        Console.BackgroundColor = tmp;
    }
}
