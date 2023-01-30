using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueensProblem.Data;
internal class Board
{
    public int Size { get; init; }
    public int Queens { get; private set; }
    public int[,] Positions { get; init; }
    public SortedList<int, (int, int)> FreePositons { get; init; }

    public Stack<((int, int), HashSet<(int, int)>)> LastAdditions { get; init; } = new();
    public HashSet<int> TestedIndexes { get; init; } = new();

    private readonly bool _silent;

    public Board(int size, bool silent = false)
    {
        Size = size;
        Queens = 0;

        Positions = new int[size, size];
        FreePositons = new();

        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                FreePositons.Add(GetKey(x, y), (x, y));
                Positions[x, y] = 0;
            }
        }

        _silent = silent;
    }

    public bool IsAtEndState()
        => Queens == Size;

    public bool TryAddQueen(int startRow, int startCol, [NotNullWhen(true)] out (int, int)? coords)
    {
        int key;
        if (FreePositons.Count == Size * Size)
        {
            key = GetKey(startRow, startCol);
        }
        else
        {
            bool next = false;
            do
            {
                key = GetNextFreeKey();

                if (key == 0)
                {
                    //TestedIndexes.Clear();
                    //if (!AdditionFailure())
                    //{
                    //    coords = null;
                    //    return false;
                    //}

                    //Queens--;
                    //next = true;

                    coords = null;
                    return false;
                }
                else
                {
                    next = false;
                }
            } while (next);
        }

        coords = FreePositons[key];

        FillQueen(coords.Value);

        if (coords.Value.Item1 != Size - 1
            && !CheckNextRow(coords.Value.Item1))
        {
            _ = AdditionFailure();

            var res = TryAddQueen(startRow, startCol, out coords);
            return res;
        }

        Queens++;
        return true;
    }

    private bool CheckNextRow(int row)
    {
        bool found = false;
        int nextRow = row + 1;
        for (int i = 0; i < Size; i++)
        {
            found = FreePositons.TryGetValue(GetKey(nextRow, i), out _);

            if (found)
                break;
        }

        return found;
    }

    private void FillQueen((int, int) start)
    {
        LastAdditions.Push((start, new()));

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;

                FillDirection(start, (x, y));
            }
        }

        Positions[start.Item1, start.Item2] = 1;
    }

    private void FillDirection((int, int) start, (int, int) direction)
    {
        int x = start.Item1;
        int y = start.Item2;
        while (x < Size && y < Size && x >= 0 && y >= 0)
        {
            if (FreePositons.Remove(GetKey(x, y)))
            {
                if (Positions[x, y] != -1)
                {
                    Positions[x, y] = -1;
                    LastAdditions.Peek().Item2.Add((x, y));
                }
            }

            x += direction.Item1;
            y += direction.Item2;
        }
    }

    private bool AdditionFailure()
    {
        if (LastAdditions.TryPop(out var item))
        {
            var x = item.Item1.Item1;
            var y = item.Item1.Item2;

            Positions[x, y] = -1;

            foreach (var pair in item.Item2)
            {
                Positions[pair.Item1, pair.Item2] = 0;
                _ = FreePositons.TryAdd(GetKey(pair.Item1, pair.Item2), pair);
            }

            TestedIndexes.Add(GetKey(x, y));

            return true;
        }

        return false;
    }

    private int GetKey(int x, int y)
        => ((x * Size) + y) + 1;
    private int GetNextFreeKey()
    {
        int key;
        int index = 0;
        do
        {
            key = FreePositons.Keys.ElementAtOrDefault(index++);
        } while (TestedIndexes.Contains(key));

        return key;
    }

    public void Print(bool force = false)
    {
        if (_silent && !force) return;

        Console.Write($"\nSize: {Size}x{Size}\n" + GetSeparator());
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                Console.Write("|");

                var val = Positions[x, y];

                var tmpColor = Console.BackgroundColor;
                Console.BackgroundColor = GetColor(val, tmpColor);

                Console.Write(string.Format("{0,2:G}", val));

                Console.BackgroundColor = tmpColor;
            }

            Console.Write("|\n" + GetSeparator());
        }
    }

    private string GetSeparator()
    {
        string sep = "";
        for (int i = 0; i < Size; i++)
            sep += "|--";

        return sep + "|\n";
    }

    private ConsoleColor GetColor(int val, ConsoleColor old)
        => val switch
        {
            -1 => ConsoleColor.DarkRed,
            1 => ConsoleColor.DarkGreen,
            _ => old
        };
}
