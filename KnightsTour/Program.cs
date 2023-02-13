using KnightsTour.Data;

namespace KnightsTour;

internal class Program
{
    static TimeSpan delay = TimeSpan.FromSeconds(1);
    static void Main(string[] args)
    {
        for (int i = 3; i <= 5; i++)
        {
            var board = new Board(i);

            for (int x = 0; x < 5; x++)
            {
                var knight = new Knight(board)
                {
                    LoggingTask = false
                };

                var start = DateTime.UtcNow;

                knight.Start().GetAwaiter().GetResult();

                var end = DateTime.UtcNow;
                var elapsed = (end - start);

                Console.WriteLine($"Board: {i}x{i}");
                Console.WriteLine($"Elapsed Time: {elapsed:G}");

                if (knight.Success)
                {
                    Console.WriteLine(knight.ToString());

                    Console.WriteLine("--- SUCCESS ---");
                }
                else
                {
                    Console.WriteLine("--- FAILURE ---");
                }

                Console.WriteLine($"Waiting {delay:g} . . .");
                Task.Delay(delay).GetAwaiter().GetResult();
                Console.WriteLine();
            }
        }
    }
}
