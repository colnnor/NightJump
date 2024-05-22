using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    private NodeGrid grid;

    public AStar(NodeGrid grid)
    {
        this.grid = grid;
    }

    private const int k_horizontalMovementCost = 10;
    private const int k_verticalMovementCost = 14;

    public bool FindPath(Vector3 startPosition, Vector3 endPosition, out List<Node> path)
    {
        path = null;

        if (!TryGetStartAndTargetNodes(startPosition, endPosition, out Node startNode, out Node targetNode))
        {
            return false;
        }

        List<Node> openList = new() { startNode };
        HashSet<Node> closedList = new();

        while (openList.Count > 0)
        {
            Node currentNode = FindLowestFCost(openList);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                path = RetracePath(startNode, targetNode);

                return true;
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.GCost + CalculateDistanceCost(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = CalculateDistanceCost(neighbor, targetNode);
                    neighbor.ConnectionNode = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return false;
    }
    private bool TryGetStartAndTargetNodes(Vector3 startPosition, Vector3 endPosition, out Node startNode, out Node targetNode)
    {
        grid.GetCoords(startPosition, out int startX, out int startY);
        grid.GetCoords(endPosition, out int targetX, out int targetY);

        startNode = grid.GetNode(startX, startY);
        targetNode = grid.GetNode(targetX, targetY);
        return startNode != null && targetNode != null && startNode.IsWalkable && targetNode.IsWalkable;
    }

    private List<Node> RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ConnectionNode;
        }

        path.Reverse();
        return path;
    }

    private Node FindLowestFCost(List<Node> nodes)
    {
        Node lowestFCostNode = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].FCost < lowestFCostNode.FCost || (nodes[i].FCost == lowestFCostNode.FCost && nodes[i].HCost < lowestFCostNode.HCost))
            {
                lowestFCostNode = nodes[i];
            }
        }

        return lowestFCostNode;
    }

    private int CalculateDistanceCost(Node a, Node b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        int remaining = Mathf.Abs(dx - dy);
        return (dx + dy) * k_horizontalMovementCost;
    }

    private List<Node> GetNeighbors(Node currentNode)
    {
        List<Node> neighborList = new();

        //Up
        AddNeighborIfExists(currentNode.X, currentNode.Y + 1, neighborList);
        //Down
        AddNeighborIfExists(currentNode.X, currentNode.Y - 1, neighborList);
        //Left
        AddNeighborIfExists(currentNode.X - 1, currentNode.Y, neighborList);
        //Right
        AddNeighborIfExists(currentNode.X + 1, currentNode.Y, neighborList);
        return neighborList;
    }

    private void AddNeighborIfExists(int x, int y, List<Node> neighbors)
    {
        if (grid.IsValidCell(x, y))
        {
            neighbors.Add(grid.GetNode(x, y));
        }
    }

}