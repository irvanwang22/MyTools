using System;

namespace MyTools.AStar
{
    public interface IPathfindingGrid
    {
        int Width { get; }
        int Height { get; }
        bool IsWalkable(int x, int y);
        int GetMovementCost(int x, int y);
    }
}
