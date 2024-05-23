using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour, IDependencyProvider
{
    CameraManager ProvideCameraManager() => this;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private LevelMovementValues levelMovementValues;
    [SerializeField] private Transform virtualCamera;
    [SerializeField] private Vector3 cameraOffset = new(0, 11, -3);

    public Camera Camera => Helpers.Camera;
    private Vector3 nextPosition;

    private void OnEnable()
    {
        GridManager.GemCollected += JumpToNextPosition;
        GameManager.OnGameEnd += GameOver;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService<CameraManager>(this);
        GridManager.GemCollected -= JumpToNextPosition;
        GameManager.OnGameEnd -= GameOver;
    }

    private void GameOver()
    {
        StopAllCoroutines();
        virtualCamera.DOKill();
    }

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<CameraManager>(this);
    }
    private void Start()
    {
        ServiceLocator.Instance.RegisterService<CameraManager>(this);
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        Camera.clearFlags = CameraClearFlags.Color;
    }
    public void ChangeCamera(bool b)
    {
        Camera.clearFlags = b ? CameraClearFlags.Skybox : CameraClearFlags.Color;
    }

    public void JumpToNextPosition(bool value)
    {
        StartCoroutine(JumpPosition());
    }

    private IEnumerator JumpPosition()
    {
        if(gameManager.GameOver) yield break;
        yield return null;
        virtualCamera.transform.DOJump(CalculateCameraPosition(), levelMovementValues.ArcHeight, 1, levelMovementValues.Speed).SetSpeedBased().SetEase(levelMovementValues.EaseType);
    }

    public Vector3 CalculateCameraPosition()
    {
        Vector3Int origin = gridManager.GetOrigin();
        int xPosition = origin.x;

        int halfSize = gridManager.GetGridSize() / 2;

        return cameraOffset + origin.With(x: xPosition + halfSize);
    }
}
