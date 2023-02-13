using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileGamePermutationGen;
internal class Tile
{
    public Tile? parent;
    public List<Tile> children = new();

    public int basicCost = 0;
    public int complexCost = 0;

    public bool repeat = false;
    public bool success = false;
    public int depth = 0;

    public int[,] positions = new int[3, 3];
    public (int, int) whitespace = (2, 2);

    public Tile(int size)
    {
        positions = new int[size, size];
    }

    private Tile() { }

    public Tile MakeChild()
    {
        Tile child = new()
        {
            whitespace = whitespace,
            parent = this,
            positions = (int[,])positions.Clone(),
            depth = depth + 1
        };

        children.Add(child);

        return child;
    }

    public bool CalculateCosts(int[,] goal)
    {
        for (int x = 0; x < positions.GetLength(1); x++)
        {
            for (int y = 0; y < positions.GetLength(1); y++)
            {
                if (goal[x, y] != positions[x, y])
                {
                    basicCost += 1;

                    var goalX = (x + y) / positions.GetLength(0);
                    var goalY = (x + y) % positions.GetLength(1);

                    complexCost += Math.Abs(goalX - x) + Math.Abs(goalY - y);
                }
            }
        }
        
        if (basicCost == 0)
            success = true;

        if (parent is not null)
        {
            basicCost += parent.basicCost;
            complexCost += parent.complexCost;
        }

        return success;
    }

    public Tile GetWhiteShiftedUp()
        => GetShifted(0, -1);
    public Tile GetWhiteShiftedRight()
        => GetShifted(1, 0);
    public Tile GetWhiteShiftedDown()
        => GetShifted(0, 1);
    public Tile GetWhiteShiftedLeft()
        => GetShifted(-1, 0);

    public Tile GetShifted(int x, int y)
    {
        var copy = MakeChild();

        copy.whitespace = (whitespace.Item1 + x, whitespace.Item2 + y);

        copy.positions[whitespace.Item1, whitespace.Item2] = positions[copy.whitespace.Item1, copy.whitespace.Item2];
        copy.positions[copy.whitespace.Item1, copy.whitespace.Item2] = 0;

        copy.repeat = InParentTree(copy);

        return copy;
    }

    private bool InParentTree(Tile tile)
    {
        if (whitespace != tile.whitespace)
        {
            if (parent is not null)
                return parent.InParentTree(tile);
            return false;
        }

        if (PositionsEqual(tile))
        {
            if (parent is not null)
                return parent.InParentTree(tile);
            return false;
        }

        return true;
    }

    public bool PositionsEqual(Tile tile)
    {
        for (int x = 0; x < positions.GetLength(1); x++)
        {
            for (int y = 0; y < positions.GetLength(1); y++)
            {
                if (tile.positions[x, y] != positions[x, y])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool[] GetFreeMoves()
    {
        var moves = new bool[4] { false, false, false, false };
        for (int x = whitespace.Item1 - 1; x <= whitespace.Item1 + 1; x++)
        {
            for (int y = whitespace.Item2 - 1; y <= whitespace.Item2 + 1; y++)
            {
                if (x >= 0 && y >= 0 && x < positions.GetLength(0) && y < positions.GetLength(1))
                {
                    // up
                    if (x == whitespace.Item1 && y == whitespace.Item2 - 1)
                        moves[0] = true;
                    // right
                    if (x == whitespace.Item1 + 1 && y == whitespace.Item2)
                        moves[1] = true;
                    // down
                    if (x == whitespace.Item1 && y == whitespace.Item2 + 1)
                        moves[2] = true;
                    // left
                    if (x == whitespace.Item1 - 1 && y == whitespace.Item2)
                        moves[3] = true;
                }
            }
        }

        return moves;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int x = 0; x < positions.GetLength(0); x++)
        {
            for (int y = 0; y < positions.GetLength(1); y++)
            {
                sb.AppendFormat("|{0}", positions[x, y] == 0 ? " " : $" {positions[x, y]} ");
            }

            sb.Append('|');

            if (x != positions.GetLength(0) - 1)
                sb.AppendLine();
        }

        return sb.ToString();
    }
}
