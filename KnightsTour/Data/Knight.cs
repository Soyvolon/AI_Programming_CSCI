using KnightsTour.Data.Enums;
using KnightsTour.Data.Structures;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsTour.Data;
internal class Knight
{
    private CancellationToken? _cancellationToken;

    public Board Board { get; init; }

    public ConcurrentDictionary<Position, List<Position>> InvalidMoves { get; init; } = new();
    public Stack<Position> PreviousPositionsOrdered { get; init; } = new();
    public HashSet<Position> PreviousPositions { get; init; } = new();

    public Position LastVisited { get; private set; } = Position.Invalid;
    public Position CurrentPosition { get; private set; }
    public Position ForwardTrack { get; private set; } = Position.Invalid;

    public bool Completed { get; private set; }
    public bool Success { get; private set; } = false;

    public bool LoggingTask { get; set; } = true;

    public Knight(Board board)
    {
        Board = board;

        var x = SafeRandom.GetInteger(0, Board.Size);
        var y = SafeRandom.GetInteger(0, Board.Size);

        CurrentPosition = new(x, y);
    }

    public Task Start(CancellationToken? cancellationToken = null)
    {
        _cancellationToken = cancellationToken;

        while (_cancellationToken?.IsCancellationRequested ?? true
            && !Completed)
        {
            DoMove();

            if (LoggingTask)
            // TODO make this thing actually threadsafe and queued.
            // And a task.
            {
                Console.WriteLine();
                Console.WriteLine(ToString());
            }
        }

        return Task.CompletedTask;
    }

    private void DoMove()
    {
        var move = GenerateValidMoves(CurrentPosition);

        if (move.Equals(Position.Invalid))
        {
            var oldCur = CurrentPosition;

            if (!RemoveUpToVisitedPosition(LastVisited))
            {
                Success = false;
                Completed = true;
            }

            InvalidMoves.AddOrUpdate(CurrentPosition, new List<Position>() { oldCur },
                (x, y) =>
                {
                    y.Add(oldCur);
                    return y;
                });
        }
        else
        {
            SetNewPosition(move);

            if (BoardFullyOccupied())
            {
                Success = true;
                Completed = true;
            }
        }
    }

    private Position GenerateValidMoves(Position curPos)
    {
        Position move = Position.Invalid;
        if (!InvalidMoves.TryGetValue(CurrentPosition, out var invalid))
            invalid = new();

        foreach (var moveType in Enum.GetValues<MoveType>())
        {
            if (Board.TryGetPosition(curPos, moveType, out var newPos))
            {
                if (!IsVisited(newPos) 
                    && !invalid.Contains(newPos))
                {
                    move = newPos;
                    break;
                }
            }
        }

        return move;
    }

    private bool BoardFullyOccupied()
    {
        var uniqueSpots = Board.Size * Board.Size;
        var places = PreviousPositions.Count + 1; // Include the current pos;

        return uniqueSpots == places;
    }

    private bool IsVisited(Position position)
        => PreviousPositions.Contains(position);

    private bool RemoveUpToVisitedPosition(Position position)
    {
        if (!IsVisited(position))
            return false;

        bool exit = false;
        while (!exit && PreviousPositionsOrdered.TryPop(out var backtrackPos))
        {
            exit = backtrackPos.Equals(position);
            PreviousPositions.Remove(backtrackPos);
            _ = InvalidMoves.TryRemove(ForwardTrack, out _);

            if (exit)
            {
                ForwardTrack = CurrentPosition;
                CurrentPosition = backtrackPos;
                if (PreviousPositionsOrdered.TryPeek(out var previous))
                    LastVisited = previous;
            }
        }

        return true;
    }

    private void SetNewPosition(Position newPos)
    {
        PreviousPositions.Add(CurrentPosition);
        PreviousPositionsOrdered.Push(CurrentPosition);

        LastVisited = CurrentPosition;
        CurrentPosition = newPos;
    }

    public override string ToString()
    {
        StringBuilder builder = new();

        for (int y = 0; y < Board.Positions.GetLength(1); y++)
        {
            builder.AppendLine(GetSeparator());
            builder.Append('|');

            for (int x = 0; x < Board.Positions.GetLength(0); x++)
            {
                var pos = new Position(x, y);

                int elemPos = PreviousPositions.Count + 1;
                if (!pos.Equals(CurrentPosition))
                {
                    bool found = false;
                    foreach (var element in PreviousPositionsOrdered)
                    {
                        elemPos--;

                        found = pos.Equals(element);

                        if (found)
                            break;
                    }

                    if (!found)
                        elemPos = -1;
                }

                builder.AppendFormat("{0,2:G}|", elemPos);
            }

            builder.AppendLine();
        }

        builder.Append(GetSeparator());

        return builder.ToString();
    }

    private string GetSeparator()
    {
        string sep = "";
        for (int i = 0; i < Board.Positions.GetLength(0); i++)
            sep += "|--";

        return sep + "|";
    }
}
