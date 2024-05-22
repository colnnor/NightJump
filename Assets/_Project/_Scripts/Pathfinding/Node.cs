using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Node
{
    public Action<int, int> OnValueChanged;
    //void InvokeOnValueChanged() => OnValueChanged?.Invoke(X, Y); 

    public int X;
    public int Y;
    
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public NodeGrid Grid;
    public Node ConnectionNode;

    public Vector3Int GetCoords()
    {
        return new Vector3Int(X, 0, Y);
    }

    public bool IsWalkable => GetNodeType == NodeType.Walkable;

    NodeType nodeType;
    public NodeType GetNodeType => nodeType;
    public void SetNodeType(NodeType nodeType) => this.nodeType = nodeType;

    public Node(int x, int y, NodeGrid grid, NodeType? nodeType = NodeType.Walkable)
    {
        this.X = x;
        this.Y = y;
        Grid = grid;

        this.nodeType = nodeType.Value;
    }
}

public enum NodeType
{
    Walkable,
    Unwalkable,
    Deadly
}