// See https://aka.ms/new-console-template for more information

using System.Text;
using TileGamePermutationGen;

var parentNode = new Tile(3);
parentNode.positions[0, 0] = 4;
parentNode.positions[0, 1] = 2;
parentNode.positions[0, 2] = 8;
parentNode.positions[1, 0] = 7;
parentNode.positions[1, 1] = 0;
parentNode.whitespace = (1, 1);
parentNode.positions[1, 2] = 6;
parentNode.positions[2, 0] = 3;
parentNode.positions[2, 1] = 5;
parentNode.positions[2, 2] = 1;

var goal = new int[3, 3] { { 1, 2, 3 }, { 4, 5, 6 }, {7, 8, 0 } };

bool useComplexWeight = false;

bool foundGoal = parentNode.CalculateCosts(goal);

PriorityQueue<Tile, int> queue = new();
queue.Enqueue(parentNode, 0);

List<Tile> order = new();

while(!foundGoal && queue.TryDequeue(out var tile, out _))
{
    //Console.WriteLine(tile);

    order.Add(tile);
    var moves = tile.GetFreeMoves();

    for (int i = 0; i < moves.Length; i++)
    {
        if (!moves[i])
            continue;

        Tile? child = null;
        switch(i)
        {
            case 0:
                child = tile.GetWhiteShiftedUp();
                break;
            case 1:
                child = tile.GetWhiteShiftedRight();
                break;
            case 2:
                child = tile.GetWhiteShiftedDown();
                break;
            case 3:
                child = tile.GetWhiteShiftedLeft();
                break;
        }

        if (child is not null && !child.repeat)
        {
            foundGoal = child.CalculateCosts(goal);
            if (order.Any(x => x.PositionsEqual(child)))
            {
                child.repeat = true;
                continue;
            }

            if (foundGoal)
                break;

            if (useComplexWeight)
                queue.Enqueue(child, child.complexCost);
            else
                queue.Enqueue(child, child.basicCost);
        }
    }
}


StringBuilder sb = new();

sb.AppendLine("@startwbs");
int depthCap = 999999;

Stack<Tile> tiles = new();
tiles.Push(parentNode);

while(tiles.TryPop(out var tile))
{
    if (tile.depth > depthCap)
        continue;

    for (int i = 0; i < tile.depth; i++)
        sb.Append('+');
    sb.Append('+');

    if (tile.repeat)
        sb.Append("[#OrangeRed]");
    else if (tile.success)
        sb.Append("[#LightGreen]");

    sb.Append(':');
    sb.Append(tile);
    sb.AppendLine(";");

    foreach (var child in tile.children)
        tiles.Push(child);
}

sb.AppendLine("@endwbs");

File.WriteAllText("output.wsd", sb.ToString());

Console.WriteLine("Done");
Console.ReadLine();