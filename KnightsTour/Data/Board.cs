using KnightsTour.Data.Enums;
using KnightsTour.Data.Structures;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsTour.Data;
internal class Board
{
    public int Size { get; init; }

    /// <summary>
    /// The positions on the board.
    /// </summary>
    /// <remarks>
    /// Board goes from 0 to <see cref="Size"/> - 1, both axis.
    /// <br /><br />
    /// Moving positive is moving up the axis, negative goes down the axis. Or,
    /// positive is up or right, negaive is down or left.
    /// </remarks>
    public Position[,] Positions { get; init; }

    public Board(int size)
    {
        Size = size;
        Positions = new Position[Size, Size];
    }

    public static Position GetNextPosition(Position start, MoveType move)
        => move switch
        {
            MoveType.UpRightRight => start with { Y = start.Y + 1, X = start.X + 2 },
            MoveType.UpLeftLeft => start with { Y = start.Y + 1, X = start.X - 2 },
            MoveType.UpUpRight => start with { Y = start.Y + 2, X = start.X + 1 },
            MoveType.UpUpLeft => start with { Y = start.Y + 2, X = start.X - 1 },
            MoveType.DownRightRight => start with { Y = start.Y - 1, X = start.X + 2 },
            MoveType.DownLeftLeft => start with { Y = start.Y - 1, X = start.X - 2 },
            MoveType.DownDownRight => start with { Y = start.Y - 2, X = start.X + 1 },
            MoveType.DownDownLeft => start with { Y = start.Y - 2, X = start.X - 1 },
            _ => throw new ArgumentException("Invalid move type", nameof(move)),
        };

    public bool TryGetPosition(Position start, MoveType move, out Position end)
    {
        var pos = GetNextPosition(start, move);

        // Ensure the position isn't out of bounds
        if (pos.X >= Positions.GetLength(0) || pos.X < 0 || pos.Y >= Positions.GetLength(1) || pos.Y < 0)
        {
            end = default;
            return false;
        }

        end = pos;
        return true;
    }
}
