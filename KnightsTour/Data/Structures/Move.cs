using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsTour.Data.Structures;
internal readonly struct Move
{
    public Position StartingPosition { get; init; }
    public Position EndingPosition { get; init; }

    public Move(Position startingPosition, Position endingPosition)
        => (StartingPosition, EndingPosition) = (startingPosition, endingPosition);

    public override string ToString()
        => $"<{StartingPosition}, {EndingPosition}>";
}
