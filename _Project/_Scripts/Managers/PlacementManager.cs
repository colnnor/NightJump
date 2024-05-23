using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private PrefabsProvider prefabsProvider;

    [SerializeField] private GameObject movingPlatform;

    private AStar pathFinder;
    private GridValues gridValues;
    private NodeGrid currentGrid;

    private Vector3Int gemPosition;
    private Vector3Int enemyPosition;
    private Vector3Int playerPosition;
    private List<Node> pathToGem = new();
    
    public List<Node> GetPathToGem() => pathToGem;
    public Vector3 GetGemPosition() => gemPosition;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<PlacementManager>(this);
        pathToGem = new();
    }
    private void Start()
    {
        prefabsProvider = ServiceLocator.Instance.GetService<PrefabsProvider>(this);
    }
    public bool AttemptToPlaceObjects(GridGroup group)
    {
        currentGrid = group.Grid;
        pathFinder = group.PathFinder;
        Debug.Log(currentGrid + " " + pathFinder + group.Origin);  
        gridValues = currentGrid.GridValues;
        Debug.Log(gridValues); 
        playerPosition = new Vector3Int(gridValues.MinimumWorldPosition.x, 0, gridValues.MinimumWorldPosition.y);


        for (int i = 0; i < 500; i++)
        {

            currentGrid.RandomizeGrid();
            

            if (!AttemptToPlaceGem()) continue;
            group.GemPosition = gemPosition;

            if (!AttemptToPlaceEnemy()) continue;
            group.EnemyPosition = enemyPosition;
            return true;
        }
        return false;
    }

    bool AttemptToPlaceGem()
    {
        for (int x = gridValues.MinimumWorldPosition.x; x < gridValues.MaximumWorldPosition.x; x++)
        {
            for (int y = gridValues.MinimumWorldPosition.y; y < gridValues.MaximumWorldPosition.y; y++)
            {
                Debug.Log($"-------------------{x}, {y}-------------------");
                Node possibleGemNode = currentGrid.GetNode(x, y);
                if (AttemptToPlace(possibleGemNode, playerPosition, ref gemPosition))
                {
                    return true;

                }
            }
        }

        return false;
    }

    bool AttemptToPlaceEnemy()
    {
        for (int x = gridValues.MinimumWorldPosition.x; x < gridValues.MaximumWorldPosition.x; x++)
        {
            Node possibleEnemyNode = currentGrid.GetNode(x, gridValues.MaximumWorldPosition.y - 1);
            if (AttemptToPlace(possibleEnemyNode, gemPosition, ref enemyPosition))
            {
                pathFinder.FindPath(enemyPosition, gemPosition, out List<Node> list);
                pathToGem = new(list);
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
        Debug.Log(attemptingNode);
        Debug.Log(attemptingNode.GetCoords() + " " + startPosition);
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
        Debug.Log(minPosition);
        Debug.Log(nodeToTry.GetCoords() + " " + minPosition);
        return pathFinder.FindPath(minPosition, nodeToTry.GetCoords(), out _);
    }

    public bool IsPathLongEnough(Node nodeToTry, Vector3 minPosition)
    {
        bool b = pathFinder.FindPath(minPosition, nodeToTry.GetCoords(), out List<Node> list);
        return list.Count > currentGrid.GetGridSize() + 1;
    }
}