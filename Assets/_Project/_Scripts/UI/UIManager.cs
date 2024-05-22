using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private RectTransform pauseMenuRect;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private GameManager gameManager;

    Vector3 pausedMenuStartPos;
    Vector3 settingsMenuStartPos;
    Vector3 gameOverMenuStartPos;
    Vector3 pauseMenuRectStartPos;

    private void Awake()
    {
        pauseMenuRectStartPos = pauseMenuRect.position;
        pausedMenuStartPos = pausedMenu.transform.localPosition;
        settingsMenuStartPos = settingsMenu.transform.localPosition;
        gameOverMenuStartPos = gameOverMenu.transform.localPosition;
    }

    [SerializeField] private Text scoreText;
    [SerializeField] private Text scoreText2;
    private void OnEnable()
    {
        GameManager.UpdateScore += UpdateScore;
        GameManager.OnGameResume += ResumeGame;
        GameManager.OnGamePause += PauseGame;
        GameManager.OnGameEnd += GameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameResume -= ResumeGame;
        GameManager.OnGameEnd -= GameOver;
        GameManager.OnGamePause -= PauseGame;
        GameManager.UpdateScore -= UpdateScore;
    }

    private void Start()
    {
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        UpdateScore(0);
    }
    public void PauseGame()
    {
        pausedMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        pausedMenu.SetActive(false);
        ResetMenus();
    }

    public void GameOver()
    {
        UpdateScore(gameManager.Score);
        gameOverMenu.SetActive(true);
    }

    public void ToSettings()
    {
        pauseMenuRect.DOMoveX(-1920, .15f).SetEase(Ease.InOutSine);
    }
    public void ToMain()
    {
        pauseMenuRect.DOMoveX(0, .15f).SetEase(Ease.InOutSine);

    }

    void ResetMenus()
    {
        pauseMenuRect.anchoredPosition = new Vector3(-960, -540, 0);

    }

    public void UpdateScore(int score)
    {
        scoreText.UpdateText($"Score: {score}");
        scoreText2.UpdateText($"FinalScore: {score}");
    }
}
