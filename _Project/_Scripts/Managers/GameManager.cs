using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static event Action OnInitialize;
    public static event Action OnGameStart;
    public static event Action OnGamePause;
    public static event Action OnGameResume;
    public static event Action OnGameEnd;
    public static event Action<int> UpdateScore;
    public static event Action<int> AdjustSize;

    [Title("Game Settings")]
    [SerializeField] private EventChannel sceneLoadedChannel;
    [SerializeField] private EventChannel gameStartChannel;
    [SerializeField] private int completeLevelsToChangeSize = 5;
    [MinValue(1)]
    [SerializeField] private int initialGridsToCreate = 3;
    [SerializeField] private int score;

    [SerializeField] private Vector3Int origin;
    [Space(10)]
    [SerializeField] private InputReader inputReader;

    private InputActionMap uiInputMap;
    private InputActionMap playerInputMap;
    private bool levelLoaded;
    private bool gameStarted;
    private int currentSize = 5;
    private int maxSize = 10;
    bool gameOver;
    public int Score => score;
    public int InitialGridsToCreate => initialGridsToCreate;
    public bool GameOver => gameOver;
    [SerializeField] private bool isPaused;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<GameManager>(this);
        uiInputMap = inputReader.GetInputActions.UI;
        playerInputMap = inputReader.GetInputActions.Player;
        isPaused = false;
    }
    private void OnEnable()
    {
        GridManager.OnLevelLoaded += LevelLoaded;
        inputReader.EnterEvent += StartGame;
        inputReader.PauseEvent += OnPause;
        PlayerController.OnPlayerDeath += EndGame;
        GridManager.GemCollected += IncreaseScore;
    }

    private void OnDisable()
    {
        inputReader.EnterEvent -= StartGame;
        GridManager.OnLevelLoaded -= LevelLoaded;
        ServiceLocator.Instance.DeregisterService<GameManager>(this);
        PlayerController.OnPlayerDeath -= EndGame;
        inputReader.PauseEvent -= OnPause;
        GridManager.GemCollected -= IncreaseScore;
    }
    private void LevelLoaded()
    {
        Debug.Log("Level loaded");

        inputReader.EnableInputActions();
        inputReader.SwitchActionMap(playerInputMap);
        levelLoaded = true;
    }

    private void IncreaseScore(bool value)
    {
        if (value)
        {
            score++;
            UpdateScore?.Invoke(score);
        }

        if ((score-initialGridsToCreate-1) % completeLevelsToChangeSize == 0)
        {
            currentSize++;
            AdjustSize?.Invoke(currentSize);
            currentSize = Mathf.Min(currentSize, maxSize);
        }
    }
    private IEnumerator Start()
    {
        yield return null;
        InitializeGame();
    }

    public void StartGame()
    {
        if (levelLoaded && !gameStarted)
        {
            gameStartChannel?.Invoke(new Empty());
            OnGameStart?.Invoke();
            gameStarted = true;
        }
    }

    public void InitializeGame()
    {
        OnInitialize?.Invoke();
        inputReader.EnablePlayerMovement(false);
    }

    public void EndGame()
    {
        gameOver = true;
        inputReader.SwitchActionMap(uiInputMap);
        OnGameEnd?.Invoke();
    }

    public void OnPause()
    {
        isPaused = !isPaused;
        if (isPaused)
            Pause();
        
        else
            Resume();
    }
    public void Pause()
    {
        isPaused = true;
        OnGamePause?.Invoke();
    }
    public void Resume()
    {
        isPaused = false;
        OnGameResume?.Invoke();
    }
}
