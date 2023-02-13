using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsTour.Data.Structures;
internal readonly struct Position : IEquatable<Position>
{
    public static Position Invalid = new(-1, -1);

    public int X { get; init; } = -1;
    public int Y { get; init; } = -1;

    public Position(int x, int y)
        => (X, Y) = (x, y);

    public override string ToString() => $"({X}, {Y})";

    public bool Equals(Position other)
        => other.GetHashCode().Equals(GetHashCode());

    public override bool Equals(object? obj)
        => obj is Position position && Equals(position);

    public override int GetHashCode()
        => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
}
