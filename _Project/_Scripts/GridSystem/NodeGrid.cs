using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public struct GridValues
{
    /// <summary>
    /// used for placement of object in the world
    /// </summary>
    public Vector2Int MinimumWorldPosition;
    public Vector2Int MaximumWorldPosition;
    /// <summary>
    /// used for pathfinding
    /// </summary>
    public Vector2Int MinimumGridPosition;
    public Vector2Int MaximumGridPosition;

    public Vector3Int origin;
    public int Size;

    public GridValues(Vector3Int origin, GridData gridData)
    {
        Size = gridData.Size;
        this.origin = origin;
        MinimumWorldPosition = new Vector2Int(origin.x, origin.z);
        MaximumWorldPosition = new Vector2Int(origin.x + gridData.Size, origin.z + gridData.Size);

        MinimumGridPosition = new Vector2Int(0, 0);
        MaximumGridPosition = new Vector2Int(gridData.Size, gridData.Size);
    }
}

public class NodeGrid
{
    private GridData gridData;
    private Vector3Int origin;

    public GridValues GridValues { get; private set; }

    Dictionary<Vector2, Node> nodes;

    public int GetGridSize() => gridData.Size;

    int currentBoulders;
    const int k_maxBoulders = 5;


    public NodeGrid(GridData gridData, Vector3Int origin)
    {
        this.gridData = gridData;
        this.origin = origin;

        GridValues = new(origin, gridData);

        Initialize();
    }

    public void Initialize()
    {
        nodes = new();

        RunLoop((x, y) => CreateAndAddNode(x, y));
    }

    private void CreateAndAddNode(int x, int y)
    {
        Vector2Int vector = new(x, y);

        Node pathNode = new(vector.x, vector.y, this);
        nodes.Add(new(vector.x, vector.y), pathNode);
    }
    public void RandomizeGrid()
    {
        int seed = 0;
        seed = seed.Random(0, 99999);
        Random.InitState(seed);

        RunLoop((x,y) => RandomizeNodeType(x,y));
    }
    private void RunLoop(Action<int, int> action)
    {
        for (int i = GridValues.MinimumWorldPosition.x; i < GridValues.MaximumWorldPosition.x; i++)
        {
            for (int j = GridValues.MinimumWorldPosition.y; j < GridValues.MaximumWorldPosition.y; j++)
            {
                action(i, j);
            }
        }
    }


    private void RandomizeNodeType(int x, int y)
    {
        float randomFloat = Random.Range(0f, 1f);
        //0 == walkable
        int tileInt = randomFloat < gridData.ClampValue ? 0 : 1;

        if (x == GridValues.MinimumWorldPosition.x && y == GridValues.MinimumWorldPosition.y)
        {
            tileInt = 0;
        }
        SetNodeType(x, y, DetermineNodeType(tileInt));
    }

    private NodeType DetermineNodeType(int tileInt)
    {
        if (tileInt == 0)
            return NodeType.Walkable;
        
        if (currentBoulders < k_maxBoulders)
        {
            int randomInt = Random.Range(0, 2);
            currentBoulders++;
            return (randomInt == 0) ? NodeType.Unwalkable : NodeType.Deadly;
        }
        
        return NodeType.Deadly;
    }


    #region getters

    public Vector3 GetWorldPosition(int x, int y) => new Vector3(x, 0, y) + origin;

    public NodeType GetNodeType(Vector3 worldPosition)
    {
        GetCoords(worldPosition, out int x, out int y);
        return GetNodeType(x, y);
    }
    public NodeType GetNodeType(int x, int y)
    {
        return IsValidCell(x, y) ? GetNode(x, y).GetNodeType : NodeType.Unwalkable ;
    }

    public Node GetNode(int x, int y)
    {
        if (IsValidCell(x, y))
        {
            Vector2Int vector = new(x, y);

            return nodes[vector];
        }
        return null;
    }

    public void GetCoords(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / gridData.CellSize);
        y = Mathf.FloorToInt(worldPosition.z / gridData.CellSize);
    }
    #endregion

    #region setters
    public void SetNode(int x, int y, Node value)
    {
        if (IsValidCell(x,y))
        {
            nodes[new Vector2(x, y)] = value;
        }
    }   

    public void SetNode(Vector3 worldPosition, Node value)
    {
        int x, y;
        GetCoords(worldPosition, out x, out y);
        SetNode(x, y, value);
    }
    public void SetNodeType(int x, int y, NodeType type) => GetNode(x, y).SetNodeType(type);
    public bool IsValidCell(int x, int y) => x >= GridValues.MinimumWorldPosition.x && x < GridValues.MaximumWorldPosition.x && y >= GridValues.MinimumWorldPosition.y && y < GridValues.MaximumWorldPosition.y;
    #endregion
}
