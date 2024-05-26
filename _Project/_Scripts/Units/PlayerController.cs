using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using QFSW.QC;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static event Action OnPlayerDeath;
    public static event Action<int> OnPlayerDamage;
    
    private GameManager gameManager;
    private PlayerAudio playerAudio;
    private FeedbacksManager feedbacksManager;
    private GridManager gridManager;

    [Title("Input")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float minimumSwipeMagnitude = 10f;

    [Title("Movement Settings")]
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private Transform movingPlatform;

    [Title("Player Settings")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float playerDamageInvisibilityDuration = 1f;

    public int Health => playerHealth._Health;  

    private PlayerFXHandler playerFXHandler;
    private EnemyController enemyController;

    //private fields
    private Collider playerCollider;
    private const float k_rotationAmount = 90f;
    private Vector3 startPosition;
    private Vector3Int nextPosition;

    private bool isMoving;
    private bool enemyCollectedGem;

    private Vector2 swipeDirection;
    
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<PlayerController>(this);
        playerFXHandler = GetComponent<PlayerFXHandler>();
        playerCollider = GetComponent<Collider>();
        startPosition = transform.position;
        playerHealth = gameObject.GetOrAddComponent<PlayerHealth>();
    }


    private void OnEnable()
    {
        GameManager.OnGameStart += StartGame;
        GameManager.OnGamePause += Pause;
        GameManager.OnGameResume += Resume;
        LightManager.DelayedLightOff += StartLevel;
        PlatformMovement.OnPlatformMovementComplete += LandedAtNewGrid;
        GridManager.GemCollected += GemCollected;
        inputReader.BackwardEvent += OnBackward;
        inputReader.ForwardEvent += OnForward;
        inputReader.LeftEvent += OnLeft;
        inputReader.RightEvent += OnRight;
        inputReader.AnyPressed += OnAnyPressed;
    }

    private void Pause()
    {
        inputReader.EnablePlayerMovement(false);
    }

    private void Resume()
    {
        if (enemyController.CollectedGem)
            return;

        inputReader.EnablePlayerMovement();
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService<PlayerController>(this);
        GameManager.OnGamePause -= Pause;
        GameManager.OnGameResume -= Resume;
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

    private void Start()
    {
        inputReader.TouchComplete += ProcessTouchComplete;
        inputReader.Swipe += ProcessSwipeDelta;
        enemyController = ServiceLocator.Instance.GetService<EnemyController>(this);
        gameManager = ServiceLocator.Instance.GetService<GameManager>(this);
        gridManager = ServiceLocator.Instance.GetService<GridManager>(this);
        playerAudio = ServiceLocator.Instance.GetService<PlayerAudio>(this);
    }

    private void ProcessSwipeDelta(Vector2 direction)
    {
        swipeDirection = direction;
    }
    private void ProcessTouchComplete()
    {
        if(MathF.Abs(swipeDirection.magnitude) < minimumSwipeMagnitude) return;

        if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x))
        {
            if (swipeDirection.y > 0)
            {
                JumpForward();
            }
            else
            {
                JumpBackward();
            }
        }
        else
        {
            if (swipeDirection.x > 0)
            {
                JumpRight();
            }
            else
            {
                JumpLeft();
            }
        }
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
        inputReader.EnableInputActions();
        EnablePlayerMovement();
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
        
        EnablePlayerMovement(false);
        HandleParent(movingPlatform);
        transform.localPosition = Vector3.zero;
    }

    void TakeDamage()
    {
        transform.DOKill();
        playerCollider.enabled = false;
        isMoving = false;
        EnablePlayerMovement(false);
        playerHealth.TakeDamage(1);

        OnPlayerDamage?.Invoke(GetHealth());

        if (playerHealth.IsDead())
        {
            OnPlayerDeath?.Invoke();
            return;
        }
        StartCoroutine(HandleIFrames());
    }

    IEnumerator HandleIFrames()
    {
        yield return Helpers.GetWait(playerDamageInvisibilityDuration);
        if(enemyCollectedGem) yield break;

        ResetPosition();
        EnablePlayerMovement();
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
        if(gridManager && gridManager.GetNodeType(endPosition) == NodeType.Unwalkable) return;

        isMoving = true;
        transform.DOMove(endPosition, jumpDuration).OnComplete(ResetIsMoving);
    }

    void Rotate(Vector3 direction) => transform.DORotate(direction, jumpDuration).SetEase(Ease.InOutQuad);


    private void HandleParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    private void EnablePlayerMovement(bool value = true) => inputReader.EnablePlayerMovement(value);
    void ResetIsMoving()
    {
        transform.position = nextPosition;
        isMoving = false;
    }

    void ResetPosition() => transform.SetPositionAndRotation(startPosition, Quaternion.identity);

    #endregion
}
