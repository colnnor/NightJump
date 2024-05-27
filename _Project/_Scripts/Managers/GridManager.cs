using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;
using Sirenix.OdinInspector;
using System;

using Random = UnityEngine.Random;


public class GridManager : MonoBehaviour
{
    public static event Action<bool> GemCollected;
    public static event Action OnLevelLoaded;   
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridObjectSpawner gridObjectSpawner;

    [SerializeField] private Transform enemy;
    [SerializeField] private Transform gem;
    [SerializeField] private PlatformMovement platformMovement;
    [SerializeField] private int pathSimilarityLimitAddition = 2;

    [Title("Awake Settings")]
    [Range(5, 10)]
    [SerializeField] private int gridDataToStart = 5;
    [SerializeField] private Vector3Int originToStart = Vector3Int.zero;
    [SerializeField] private GridDataGroup gridDataGroup;
    [Space(10)]
    [SerializeField] private int totalGridsCreated = 0;
    [SerializeField] private int currentGridIndex = 0;

    private Dictionary<int, GridGroup> gridGroupsDict = new();

    private GridGroup currentGridGroup;
    private GridData currentGridData;
    private int initialGridsToCreate;

    public int GetPathSimilarityLimit() => currentGridData.Size + pathSimilarityLimitAddition + 100;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<GridManager>(this);
    }

    private void Start()
    {
        gridObjectSpawner = ServiceLocator.Instance.GetService<GridObjectSpawner>(this);
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        initialGridsToCreate = gameManager.InitialGridsToCreate;
        currentGridData = GetGridData(gridDataToStart);
    }
    private void OnEnable()
    {
        GameManager.AdjustSize += UpdateGridData;
        GameManager.OnInitialize += GameStart;
        Gem.GemCollected += NextGrid;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService<GridManager>(this);
        GameManager.AdjustSize += UpdateGridData;
        GameManager.OnInitialize -= GameStart;
        Gem.GemCollected -= NextGrid;
    }

    public void GameStart()
    {
        CreateNewGrid(Vector3Int.zero);

        for (int i = 0; i < initialGridsToCreate - 1; i++)
        {
            GridGroup group = gridGroupsDict[totalGridsCreated-1];

            Vector3Int offset = GetRandomOffset(currentGridData.Size);
            Vector3Int newPosition = group.Origin + offset;

            CreateNewGrid(newPosition);
        }

        currentGridIndex = 0;
        currentGridGroup = gridGroupsDict[currentGridIndex];
        SetGemAndEnemyPosition(currentGridGroup);
        platformMovement.transform.position = currentGridGroup.GemPosition;
        OnLevelLoaded?.Invoke();
    }
    

    public void NextGrid(bool value)
    {
        if (currentGridIndex > 0)
        {
            GridGroup finalGroup = gridGroupsDict[totalGridsCreated - 1];

            CreateNewGrid(finalGroup.Origin + GetRandomOffset(finalGroup.GridSize));
            gridGroupsDict[totalGridsCreated - initialGridsToCreate - 1].ReturnObjectsToPool();
        }

        currentGridIndex++;
        currentGridGroup = gridGroupsDict[currentGridIndex];
        SetGemAndEnemyPosition(currentGridGroup);
        GemCollected?.Invoke(value);
    }

    public void CreateNewGrid(Vector3Int origin)
    {
        GridGroup group = new(origin, currentGridData, gridObjectSpawner, totalGridsCreated);
        gridGroupsDict.Add(totalGridsCreated, group);

        //ADD A DICTIONARY FOR THE GRID GROUPS AND INDICES
        bool b = group.AttemptToPlaceObjects();
        if (!b)
        {
            Debug.LogError("Failed to place objects... Aborting.");
        }

        group.InstantiateGrid();

        totalGridsCreated++;
    }

    private void SetGemAndEnemyPosition(GridGroup group)
    {
        enemy.position = group.EnemyPosition;
        gem.position = group.GemPosition;
    }

    private Vector3Int GetRandomOffset(int gridSize)
    {
        int randomX = Random.Range(-gridSize, gridSize * 2);
        int randomZ = Random.Range(gridSize * 2, gridSize * 3);
        Vector3Int offset = new(randomX, 0, randomZ);

        return offset;
    }

    private GridData GetGridData(int index) => gridDataGroup.GridDatas[index];

    public void UpdateGridData(int index)
    {
        currentGridData = GetGridData(index);
    }

    public NodeType GetNodeType(int x, int y) => GetCurrentGrid().GetNodeType(x, y);
    public NodeType GetNodeType(Vector3 position) => GetCurrentGrid().GetNodeType(position);
    public Node GetNode(int x, int y) => GetCurrentGrid().GetNode(x, y);

    public GridGroup GetCurrentGridAndPathfinder() => currentGridGroup;
    public int GetGridSize() => currentGridGroup.Size;
    public NodeGrid GetCurrentGrid() => currentGridGroup.Grid;
    public GridValues GetGridValues() => currentGridGroup.GridValues;
    public Vector3Int GetOrigin() => currentGridGroup.Origin;
    public Vector3Int GetGemPosition() => currentGridGroup.GemPosition;
}
