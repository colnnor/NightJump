using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public class GridObjectSpawner : MonoBehaviour, IDependencyProvider
{

    GridObjectSpawner ProvideGridObjectSpawner() => this;

    [Title("Pools")]
    [SerializeField] private ObjectPool walkableObjectPool;
    [SerializeField] private ObjectPool deadlyObjectPool;
    [SerializeField] private ObjectPool UnwalkableObjectPool;

    [SerializeField] private Dictionary<int, List<GameObject>> pooledObjectsDict = new();

    private ObjectPool poolToPullFrom;

    [Title("Settings")]
    [MinMaxSlider(-.1f, .1f, true)]
    [SerializeField] private Vector2 tileYPositionMinMax = new(-.02f, .02f);

    GridValues gridValues;
    private NodeGrid grid;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<GridObjectSpawner>(this);
    }


    float tileYPosition = 0;

    public void InstantiateGrid(NodeGrid grid, int index)
    {
        this.grid = grid;
        gridValues = grid.GridValues;

        InstantiateNewObjects(index);
    }

    private void InstantiateNewObjects(int index)
    {
        List<GameObject> list = new();
        tileYPosition = 0;

        for (int x = gridValues.MinimumWorldPosition.x; x < gridValues.MaximumWorldPosition.x; x++)
        {
            for (int y = gridValues.MinimumWorldPosition.y; y < gridValues.MaximumWorldPosition.y; y++)
            {
                GetPoolAndSetYPosition(x,y);

                Vector3 newTileposition = new(x, tileYPosition, y);
                GameObject obj = poolToPullFrom.GetObject(newTileposition, Quaternion.identity);
                list.Add(obj);
            }
        }
        pooledObjectsDict.Add(index, list);
    }

    private void GetPoolAndSetYPosition(int x, int y)
    {
        NodeType nodeType = grid.GetNodeType(x, y);

        switch (nodeType)
        {
            case NodeType.Walkable:
                poolToPullFrom = walkableObjectPool;
                SetRandomTileYPosition();
                break;

            case NodeType.Unwalkable:
                poolToPullFrom = UnwalkableObjectPool;
                break;

            case NodeType.Deadly:
                poolToPullFrom = deadlyObjectPool;
                break;
        }
    }

    public void SetRandomTileYPosition() => tileYPosition = tileYPosition.Random(tileYPositionMinMax.x, tileYPositionMinMax.y);

    public void ReturnObjectsToPool(int index)
    {
        //Return all objects to each pool
        List<GameObject> list = pooledObjectsDict[index];

        foreach (GameObject obj in list)
        {
            Vector3 position = obj.transform.position;
            grid.GetCoords(position, out int x, out int y);
            NodeType objectNoteType = grid.GetNodeType(x, y);

            switch (objectNoteType)
            {
                case NodeType.Walkable:
                    walkableObjectPool.ReturnObject(obj);
                    break;
                case NodeType.Unwalkable:
                    UnwalkableObjectPool.ReturnObject(obj);
                    break;
                case NodeType.Deadly:
                    deadlyObjectPool.ReturnObject(obj);
                    break;
            }
        }
        pooledObjectsDict.Remove(index);
    }
}
