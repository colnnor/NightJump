using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MenuState
{
    Main,
    Settings,
    GameOver,
    None
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private float animTime;
    [SerializeField] private Ease easeType = Ease.InOutQuart;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text finalScoreText;

    private Dictionary<MenuState, GameObject> menuScales;

    private GameManager gameManager;
                
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
        menuScales = new()
        {
            { MenuState.Main, pausedMenu },
            { MenuState.Settings, settingsMenu },
            { MenuState.GameOver, gameOverMenu },
        };
        SetMenuState(MenuState.None);
        EnableMenus(false);
        
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        UpdateScore(0);
    }
    public void PauseGame()
    {
        EnableMenus();
        SetMenuState(MenuState.Main);
    }

    public void EnableMenus(bool enabled = true)
    {
        panel.SetActive(enabled);
        foreach (var menu in menuScales)
        {
            if (!enabled)
                menu.Value.transform.localScale = Vector3.zero;
            menu.Value.SetActive(enabled);
        }
    }

    public void ResumeGame()
    {
        EnableMenus(false);
    }

    public void GameOver()
    {
        EnableMenus();
        SetMenuState(MenuState.GameOver);
        UpdateScore(gameManager.Score);
    }
    public void ToSettings()
    {
        SetMenuState(MenuState.Settings);
    }
    public void ToMain()
    {
        SetMenuState(MenuState.Main);
    }
    public void ToGameOver()
    {
        SetMenuState(MenuState.GameOver);
    }
    private void SetMenuState(MenuState state)
    {
        foreach (var menu in menuScales)
        {
            if (!menu.Value.activeSelf)
                return;

            if (menu.Key == state)
            {
                menu.Value.transform.DOScale(Vector3.one, animTime).SetEase(easeType);
            }
            else
            {
                menu.Value.transform.DOScale(Vector3.zero, animTime).SetEase(easeType);
            }
        }
    }

    public void UpdateScore(int score)
    {
        scoreText.UpdateText($"Score: {score}");
        finalScoreText.UpdateText($"Final Score: {score}");
    }
}

