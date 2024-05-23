using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CameraManager ProvideCameraManager() => this;

    [SerializeField] private LevelMovementValues levelMovementValues;
    [SerializeField] private Transform virtualCamera;
    [SerializeField] private Vector3 cameraOffset = new(0, 11, -3);

    private Camera mainCam;
    private GameManager gameManager;
    private GridManager gridManager;
    private Vector3 nextPosition;
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this);
        mainCam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        GridManager.GemCollected += JumpToNextPosition;
        GameManager.OnGamePause += Pause;
        GameManager.OnGameResume += Resume;
        GameManager.OnGameEnd += GameOver;
    }



    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService(this);
        GameManager.OnGamePause -= Pause;
        GameManager.OnGameResume -= Resume;
        GridManager.GemCollected -= JumpToNextPosition;
        GameManager.OnGameEnd -= GameOver;
    }
    private void Start()
    {
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        mainCam.clearFlags = CameraClearFlags.Color;
    }
    
    
    
    private void Pause() => virtualCamera.transform.DOPause();
    private void Resume() => virtualCamera.transform.DOPlay();

    private void GameOver()
    {
        StopAllCoroutines();
        virtualCamera.DOKill();
    }

    public void ChangeCamera(bool b)
    {
        mainCam.clearFlags = b ? CameraClearFlags.Skybox : CameraClearFlags.Color;
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
