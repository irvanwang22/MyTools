using System;

namespace MyTools.AStar
{
    public class PathfindingNode : IComparable<PathfindingNode>
    {
        public int x;
        public int y;

        public int gCost; // current cost from start to current node
        public int hCost; // estimate cost from current node to end
        public int fCost => gCost + hCost; // total cost

        public PathfindingNode parent;

        public PathfindingNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int CompareTo(PathfindingNode other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }
            return compare;
        }
    }
}