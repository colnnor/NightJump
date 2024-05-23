using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using QFSW.QC;
using System.Collections;

public class PlayerController : MonoBehaviour, IDependencyProvider
{
    public static event Action OnPlayerDeath;
    public static event Action<int> OnPlayerDamage;
    
    //Dependency Injection
    [Provide] PlayerController ProvidePlayerController() => this;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerAudio playerAudio;
    [SerializeField] private FeedbacksManager feedbacksManager;
    [SerializeField] private GridManager gridManager;

    [Title("Input")]
    [SerializeField] private InputReader inputReader;

    [Title("Movement Settings")]
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private Transform movingPlatform;

    [Title("Player Settings")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float playerDamageInvisibilityDuration = 1f;

    public int Health => playerHealth._Health;  

    private PlayerFXHandler playerFXHandler;

    //private fields
    private Collider playerCollider;
    private const float k_rotationAmount = 90f;
    private Vector3 startPosition;
    private bool isMoving;
    private Vector3Int nextPosition;

    private bool enemyCollectedGem;
    
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<PlayerController>(this);
        playerFXHandler = GetComponent<PlayerFXHandler>();
        playerCollider = GetComponent<Collider>();
        startPosition = transform.position;
        playerHealth = gameObject.GetOrAddComponent<PlayerHealth>();
    }

    private void Start()
    {
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        feedbacksManager = ServiceLocator.Instance.GetService<FeedbacksManager>(this);
        playerAudio = ServiceLocator.Instance.GetService<PlayerAudio>(this);
    }

    #region input events
    private void OnAnyPressed() => PlayerJump();
    private void OnBackward() => JumpBackward();
    private void OnForward() => JumpForward();
    private void OnLeft() => JumpLeft();
    private void OnRight() => JumpRight();
    #endregion

    #region movement calls

    public void PlayerJump()
    {
        playerFXHandler.PlayShakeParticles();
        playerAudio?.PlayJumpSound();
    }

    public void JumpRight()
    {
        if (!isMoving)
        {
            nextPosition = transform.position.ToInt() + Vector3Int.right;
            MoveCharacter(nextPosition);
            Rotate(new Vector3(0, k_rotationAmount, 0));
        }
    }
    public void JumpLeft()
    {
        if (!isMoving)
        {
            nextPosition = transform.position.ToInt() + Vector3Int.left;
            MoveCharacter(nextPosition);
            Rotate(new Vector3(0, -k_rotationAmount, 0));
        }
    }
    public void JumpForward()
    {
        if (!isMoving)
        {
            nextPosition = transform.position.ToInt() + Vector3Int.forward;
            MoveCharacter(nextPosition);
            Rotate(new Vector3(0, 0, 0));
        }
    }
    public void JumpBackward()
    {
        if (!isMoving)
        {
            nextPosition = transform.position.ToInt() + Vector3Int.back;
            MoveCharacter(nextPosition);
            Rotate(new Vector3(0, 2 * -k_rotationAmount, 0));
        }
    }
    #endregion

    private void OnEnable()
    {
        GameManager.OnGameStart += StartGame;
        LightManager.DelayedLightOff += StartLevel;
        PlatformMovement.OnPlatformMovementComplete += LandedAtNewGrid;
        GridManager.GemCollected += GemCollected;
        inputReader.BackwardEvent += OnBackward;
        inputReader.ForwardEvent += OnForward;
        inputReader.LeftEvent += OnLeft;
        inputReader.RightEvent += OnRight;
        inputReader.AnyPressed += OnAnyPressed;
    }
    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService<PlayerController>(this);
        GameManager.OnGameStart -= StartGame;
        LightManager.DelayedLightOff -= StartLevel;
        PlatformMovement.OnPlatformMovementComplete -= LandedAtNewGrid;
        GridManager.GemCollected -= GemCollected;
        inputReader.BackwardEvent -= OnBackward;
        inputReader.ForwardEvent -= OnForward;
        inputReader.LeftEvent -= OnLeft;
        inputReader.RightEvent -= OnRight;
        inputReader.AnyPressed -= OnAnyPressed;
    }

    public void StartGame()
    {
        ResetPosition();
        playerHealth.ResetHealth();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IDeadly>() != null)
        {
            TakeDamage();
        }
    }
    public void LandedAtNewGrid()
    {
        HandleParent(null);
        playerCollider.enabled = true;
        startPosition = gridManager.GetOrigin();
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        if(gameManager.GameOver) return;
        if(enemyCollectedGem)
        {
            enemyCollectedGem = false;
            playerFXHandler.FlickerPlayerVisibility();
        }
    }

    public void StartLevel()
    {
        EnableInputActions();
    }
    public void GemCollected(bool value)
    {
        if(gameManager.GameOver) return;
        if (!value)
        {
            enemyCollectedGem = true;
            TakeDamage();
        }
        transform.DOKill();
        transform.rotation = Quaternion.identity;
        isMoving = false;
        playerCollider.enabled = false;
        
        EnableInputActions(false);
        HandleParent(movingPlatform);
        transform.localPosition = Vector3.zero;
    }

    void TakeDamage()
    {
        transform.DOKill();
        playerCollider.enabled = false;
        isMoving = false;
        EnableInputActions(false);
        playerHealth.TakeDamage(1);

        OnPlayerDamage?.Invoke(GetHealth());

        if (playerHealth.IsDead())
        {
            OnPlayerDeath?.Invoke();
            return;
        }
        if (enemyCollectedGem) return;

        ResetPosition();
        StartCoroutine(HandleIFrames());
    }

    IEnumerator HandleIFrames()
    {
        yield return Helpers.GetWait(playerDamageInvisibilityDuration);
        EnableInputActions();
        playerFXHandler.FlickerPlayerVisibility();
        playerCollider.enabled = true;
    }

    //properties
    public float GetJumpDuration() => jumpDuration;
    public int GetHealth() => playerHealth._Health;
    public float GetInvincibilityDuration() => playerDamageInvisibilityDuration;

    #region movement methods
    void MoveCharacter(Vector3Int endPosition)
    {
        if(gridManager.GetNodeType(endPosition) == NodeType.Unwalkable) return;

        isMoving = true;
        transform.DOMove(endPosition, jumpDuration).OnComplete(ResetIsMoving);
    }

    void Rotate(Vector3 direction) => transform.DORotate(direction, jumpDuration).SetEase(Ease.InOutQuad);


    private void HandleParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    private void EnableInputActions(bool value = true) => inputReader.EnableInputActions(value);
    void ResetIsMoving()
    {
        transform.position = nextPosition;
        isMoving = false;
    }

    void ResetPosition() => transform.SetPositionAndRotation(startPosition, Quaternion.identity);

    #endregion
}
