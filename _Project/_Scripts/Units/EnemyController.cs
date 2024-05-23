using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyController : MonoBehaviour, IDeadly, IDependencyProvider
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private EnemyMovementValues enemyMovementValues;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Animator animator;

    [SerializeField] private bool CanMove;

    [SerializeField] private float lightSpeedMultiplier = .25f;

    private List<Node> pathToGem;
    private bool endOfPathReached;
    private bool canMove;
    private int currentPosition = 0;
    private float fastTranslationTime;
    private float slowTranslationTime;
    private float currentTranslationTime;

    private int enemyJumpHash;

    private void Start()
    {
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);

        slowTranslationTime = enemyMovementValues.TranslationTime;
        fastTranslationTime = slowTranslationTime * lightSpeedMultiplier;

        currentTranslationTime = slowTranslationTime;
    }

    private void OnEnable()
    {
        GameManager.OnGameEnd += EndGame;
        GameManager.OnGamePause += PauseGame;
        GameManager.OnGameResume += OnGameStart;
        LightManager.DelayedLightOff += OnGameStart;
        enemyJumpHash = Animator.StringToHash("Jump");
        inputReader.LightEnabledEvent += OnLightEnabled;
        GridManager.GemCollected += OnGemCollected;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd -= EndGame;
        GameManager.OnGamePause -= PauseGame;
        GameManager.OnGameResume -= OnGameStart;
        LightManager.DelayedLightOff -= OnGameStart;
        inputReader.LightEnabledEvent -= OnLightEnabled;
        GridManager.GemCollected -= OnGemCollected;
    }

    private void PauseGame()
    {
        StopAllCoroutines();
        CanMove = false;
        canMove = false;
    }

    private void EndGame()
    {
        transform.DOKill();
        endOfPathReached = false;
        currentPosition = 0;
        EnableMovement(false);
    }

    void EnableMovement(bool value = true)
    {
        canMove = value;
        CanMove = value;    
    }

    private void OnGameStart()
    {
        EnableMovement();
        currentTranslationTime = slowTranslationTime;
    }
    private void OnGemCollected(bool value)
    {
        StopAllCoroutines();
        transform.DOKill();
        transform.rotation = Quaternion.Euler(new(0, 180, 0));
        endOfPathReached = false;
        currentPosition = 0;
        canMove = false;
    }
    private void OnLightEnabled(bool isEnabled)
    {
        currentTranslationTime = isEnabled ? fastTranslationTime : slowTranslationTime;
    }

    private void Update()
    {
        if (canMove && !endOfPathReached && CanMove && !gameManager.GameOver)
        {
            StartMove();
        }

    }
    private void StartMove()
    {
        if (!canMove || !CanMove) return;
        canMove = false;

        animator.SetTrigger(enemyJumpHash);
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 nextPosition = GetNextPosition();
        Vector3 direction = nextPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.DORotate(rotation.eulerAngles, currentTranslationTime * .75f);
        transform.DOMove(nextPosition, currentTranslationTime).OnComplete(() => StartCoroutine(ResetCanMove()));
    }



    Vector3 GetNextPosition()
    {
        pathToGem = new(gridManager.GetCurrentGridAndPathfinder().EnemyPathToGem);

        return pathToGem[currentPosition].GetCoords();
    }
    IEnumerator ResetCanMove()
    {
        currentPosition++;
        endOfPathReached = currentPosition >= pathToGem?.Count;
        yield return Helpers.GetWait(enemyMovementValues.MoveCooldownTime);
        canMove = true;
    }
}