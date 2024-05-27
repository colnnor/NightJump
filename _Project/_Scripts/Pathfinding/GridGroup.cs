using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
[InlineEditor]
public class GridGroup
{
    private GridObjectSpawner gridObjectSpawner;

    [SerializeField] private List<Node> enemyPathToGem;
    [SerializeField] private List<Node> playerPathToGem;
    [SerializeField] private AStar pathFinder;
    [SerializeField] private NodeGrid grid;
    [SerializeField] private GridValues gridValues;
    [SerializeField] private Vector3Int origin;
    [SerializeField] private Vector3Int gemPosition;
    [SerializeField] private Vector3Int enemyPosition;
    [SerializeField] private int index;

    private GridManager gridManager;
    private int pathSimilarityLimit;
    
    public int Size => grid.GetGridSize();
    public AStar PathFinder => pathFinder;
    public NodeGrid Grid => grid;
    public GridValues GridValues => gridValues;
    public List<Node> EnemyPathToGem => enemyPathToGem;
    public List<Node> PlayerPathToGem => playerPathToGem;
    public Vector3Int Origin => origin;
    public int GridSize { get => Grid.GetGridSize(); }
    public Vector3Int GemPosition { get => gemPosition; set => gemPosition = value; }
    public Vector3Int EnemyPosition { get => enemyPosition; set => enemyPosition = value; }
    public int Index { get => index; }

    public GridGroup(Vector3Int origin, GridData gridData, GridObjectSpawner spawner, int index)
    {
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        this.index = index;
        this.origin = origin;
        gridObjectSpawner = spawner;

        grid = new NodeGrid(gridData, origin);
        pathFinder = new AStar(Grid);
        enemyPathToGem = new();
        playerPathToGem = new();

        pathSimilarityLimit = gridManager.GetPathSimilarityLimit();
        gridValues = grid.GridValues;
    }

    public bool AttemptToPlaceObjects()
    {
        for (int i = 0; i < 5000; i++)
        {
            Grid.RandomizeGrid();

            if (!AttemptToPlaceGem()) continue;

            if (!AttemptToPlaceEnemy()) continue;

            if (PlayerEnemyPathIsTooSimilar()) continue;
            
            return true;
        }
        return false;
    }

    private int ComparePlayerAndEnemyPath()
    {
        int count = 0;
        for(int i = 0; i < playerPathToGem.Count; i++)
        {
            if (enemyPathToGem.Contains(playerPathToGem[i]))
            {
                count++;
            }
        }
        return count;
    }

    bool AttemptToPlaceGem()
    {
        for (int x = GridValues.MinimumWorldPosition.x; x < GridValues.MaximumWorldPosition.x; x++)
        {
            for (int y = GridValues.MinimumWorldPosition.y; y < GridValues.MaximumWorldPosition.y; y++)
            {
                Node possibleGemNode = Grid.GetNode(x, y);
                if (AttemptToPlace(possibleGemNode, Origin, ref gemPosition))
                {
                    PathFinder.FindPath(gemPosition, Origin, out List<Node> list);
                    playerPathToGem = new(list);
                    return true;
                }
            }
        }

        return false;
    }

    bool AttemptToPlaceEnemy()
    {
        for (int x = GridValues.MinimumWorldPosition.x; x < GridValues.MaximumWorldPosition.x; x++)
        {
            Node possibleEnemyNode = Grid.GetNode(x, GridValues.MaximumWorldPosition.y - 1);
            if (AttemptToPlace(possibleEnemyNode, GemPosition, ref enemyPosition))
            {
                PathFinder.FindPath(enemyPosition, gemPosition, out List<Node> list);
                enemyPathToGem = new(list);
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Checks to see if the node is a valid path and if the path is long enough
    /// </summary>
    bool AttemptToPlace(Node attemptingNode, Vector3Int startPosition, ref Vector3Int positionToChange)
    {
        if (!HasValidPath(attemptingNode, startPosition))
        {
            return false;
        }

        if (!IsPathLongEnough(attemptingNode, startPosition))
        {
            return false;
        }
        positionToChange = attemptingNode.GetCoords();
        return true;
    }

    public bool HasValidPath(Node nodeToTry, Vector3Int minPosition)
    {
        return PathFinder.FindPath(minPosition, nodeToTry.GetCoords(), out _);
    }

    public bool IsPathLongEnough(Node nodeToTry, Vector3 minPosition)
    {
        PathFinder.FindPath(minPosition, nodeToTry.GetCoords(), out List<Node> list);
        return list.Count > Grid.GetGridSize() + 1;
    }

    public void InstantiateGrid()
    {
        gridObjectSpawner.InstantiateGrid(grid, index);
    }

    public void ReturnObjectsToPool()
    {
        gridObjectSpawner.ReturnObjectsToPool(index);
    }

    public bool PlayerEnemyPathIsTooSimilar()
    {
        int similarCount = 0;
        for(int i = 0; i < enemyPathToGem.Count; i++)
        {
            if (playerPathToGem.Contains(enemyPathToGem[i]))
            {
                similarCount++;
            }
        }

        return similarCount + 1 >= pathSimilarityLimit;
    }
}
