using System.Collections.Generic;
using UnityEngine;

namespace MyTools.AStar
{
    public class AStarPathfinder
    {
        private IPathfindingGrid grid;
        private PathfindingNode[,] nodes;

        public AStarPathfinder(IPathfindingGrid grid)
        {
            this.grid = grid;
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            nodes = new PathfindingNode[grid.Width, grid.Height];

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    nodes[x, y] = new PathfindingNode(x, y);
                }
            }
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            if (!IsValid(start) || !IsValid(end) || !grid.IsWalkable(end.x, end.y))
                return null;

            // Reset all Nodes
            ResetNodes();

            PathfindingNode startNode = nodes[start.x, start.y];
            PathfindingNode endNode = nodes[end.x, end.y];

            List<PathfindingNode> openSet = new List<PathfindingNode>();
            HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathfindingNode current = GetLowestFCostNode(openSet);

                if (current == endNode)
                    return RetracePath(startNode, endNode);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (PathfindingNode neighbor in GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    int moveCost = grid.GetMovementCost(neighbor.x, neighbor.y);
                    int newGCost = current.gCost + moveCost;

                    if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newGCost;
                        neighbor.hCost = GetHeuristic(neighbor.x, neighbor.y, end.x, end.y);
                        neighbor.parent = current;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // No path found
        }

        private void ResetNodes()
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    nodes[x, y].gCost = int.MaxValue;
                    nodes[x, y].hCost = 0;
                    nodes[x, y].parent = null;
                }
            }
        }

        private bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < grid.Width && pos.y >= 0 && pos.y < grid.Height;
        }

        private PathfindingNode GetLowestFCostNode(List<PathfindingNode> nodes)
        {
            PathfindingNode lowest = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].fCost < lowest.fCost ||
                    (nodes[i].fCost == lowest.fCost && nodes[i].hCost < lowest.hCost))
                {
                    lowest = nodes[i];
                }
            }
            return lowest;
        }

        private List<PathfindingNode> GetNeighbors(PathfindingNode node)
        {
            List<PathfindingNode> neighbors = new List<PathfindingNode>();

            // directions
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int checkX = node.x + dx[i];
                int checkY = node.y + dy[i];

                if (checkX >= 0 && checkX < grid.Width &&
                    checkY >= 0 && checkY < grid.Height &&
                    grid.IsWalkable(checkX, checkY))
                {
                    neighbors.Add(nodes[checkX, checkY]);
                }
            }

            return neighbors;
        }

        private int GetHeuristic(int x1, int y1, int x2, int y2)
        {
            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

        private List<Vector2Int> RetracePath(PathfindingNode start, PathfindingNode end)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathfindingNode current = end;

            while (current != start)
            {
                path.Add(new Vector2Int(current.x, current.y));
                current = current.parent;
            }

            path.Reverse();
            return path;
        }
    }
}