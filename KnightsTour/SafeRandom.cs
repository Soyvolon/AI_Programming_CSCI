using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsTour;
internal class SafeRandom
{
    private static readonly Random Random = new();

    /// <summary>
    /// See <see cref="Random.Next(int, int)"/>
    /// </summary>
    public static int GetInteger(int minValue, int maxValue)
    {
        lock(Random)
        {
            return Random.Next(minValue, maxValue);
        }
    }
}
