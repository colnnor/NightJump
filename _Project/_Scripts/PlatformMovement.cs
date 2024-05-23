using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;


public class PlatformMovement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private GameManager gameManager;

    [Title("Main Move Settings")]
    [SerializeField] private LevelMovementValues levelMovementValues;

    public static event Action OnPlatformMovementComplete;

    private Vector3 currentEndPoint;
    private Vector3 centerPoint;

    private void Start()
    {
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        cameraManager = ServiceLocator.Instance.GetService<CameraManager>(this);
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
    }

    void CalculateCenterPoint()
    {
        centerPoint = (transform.position + currentEndPoint) / 2f;
        centerPoint.y += levelMovementValues.ArcHeight;
    }

    private void OnEnable()
    {
        GameManager.OnGameEnd += GameOver;
        GridManager.GemCollected += MovePlatform;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd -= GameOver;
        GridManager.GemCollected -= MovePlatform;
    }

    private void GameOver()
    {
        Debug.Log("Killing transform movement"); 
        transform.DOKill();
    }

    public void MovePlatform(bool value)
    {
        if(gameManager.GameOver) return;
        currentEndPoint = gridManager.GetOrigin();
        CalculateCenterPoint();

        transform.DOJump(currentEndPoint, levelMovementValues.ArcHeight, 1, levelMovementValues.Speed).SetSpeedBased().SetEase(levelMovementValues.EaseType).OnComplete(ExecuteCallBack);
        return;
    }

    void ExecuteCallBack()
    {
        OnPlatformMovementComplete?.Invoke();
        transform.position = gridManager.GetGemPosition();
    }
}
