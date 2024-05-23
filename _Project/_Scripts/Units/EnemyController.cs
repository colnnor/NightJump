using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyController : MonoBehaviour, IDeadly
{

    [SerializeField] private EnemyMovementValues enemyMovementValues;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Animator animator;

    [SerializeField] private bool CanMove;

    [SerializeField] private float lightSpeedMultiplier = .25f;

    private List<Node> pathToGem;
    private GridManager gridManager;
    private GameManager gameManager;
    
    private bool endOfPathReached;
    private bool canMove;
    private bool paused;
    public bool CollectedGem { get; private set;}

    private Collider enemyCollider;
    
    private int currentPosition = 0;
    private int enemyJumpHash;
   
    private float fastTranslationTime;
    private float slowTranslationTime;
    private float currentTranslationTime;
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this);
        enemyCollider = GetComponent<Collider>();
    }

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
        enemyJumpHash = Animator.StringToHash("Jump");

        GameManager.OnGameEnd += EndGame;
        GameManager.OnGamePause += PauseGame;
        GameManager.OnGameResume += OnGameResume;
        LightManager.DelayedLightOff += OnGameStart;
        inputReader.LightEnabledEvent += OnLightEnabled;
        GridManager.GemCollected += OnGemCollected;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService(this);

        GameManager.OnGameEnd -= EndGame;
        GameManager.OnGamePause -= PauseGame;
        GameManager.OnGameResume -= OnGameResume;
        LightManager.DelayedLightOff -= OnGameStart;
        inputReader.LightEnabledEvent -= OnLightEnabled;
        GridManager.GemCollected -= OnGemCollected;
    }

    private void PauseGame()
    {
        paused = true;
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
        enemyCollider.enabled = true;
        canMove = value;
        CanMove = value;    
    }

    private void OnGameResume()
    {
        paused = false;
        if(CollectedGem) return;
        EnableMovement();
        currentTranslationTime = slowTranslationTime;
    }
    private void OnGameStart()
    {
        CollectedGem = false;   
        if(paused) return;
        EnableMovement();
        currentTranslationTime = slowTranslationTime;
    }
    private void OnGemCollected(bool value)
    {
        CollectedGem = true;
        enemyCollider.enabled = false;
        StopAllCoroutines();
        if (transform)
        {
        transform.DOKill();
        transform.rotation = Quaternion.Euler(new(0, 180, 0));

        }
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