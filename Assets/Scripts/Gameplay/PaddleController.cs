using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Controls a paddle's vertical movement using the new Input System.
/// Assign the matching Input Action in the Inspector (Player 1: W/S, Player 2: Up/Down).
/// Exposes speed and scale multiplier methods for external systems (e.g., powerups)
/// without knowing what triggers them.
/// </summary>
public class PaddleController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("Identity")]
    [Tooltip("Player index: 0 = Left (W/S), 1 = Right (Up/Down).")]
    public int playerIndex;

    [Header("Input")]
    [SerializeField] private InputActionReference moveInput;

    [Header("Events")]
    [SerializeField] private GoalScoredEventChannelSO goalScoredEvent;
    [SerializeField] private VoidEventChannelSO paddleRespawnedEvent;
    [SerializeField] private VoidEventChannelSO gameOverEvent;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1.0f;
    [SerializeField] private float respawnDuration = 0.5f;

    [Header("Bounds")]
    [Tooltip("Maximum Y position the paddle center can reach.")]
    [SerializeField] private float upperBound = 4f;
    [Tooltip("Minimum Y position the paddle center can reach.")]
    [SerializeField] private float lowerBound = -4f;

    private float _moveDirection;
    private float _speedMultiplier = 1f;
    private Vector3 _originalScale;
    private bool _isGameOver;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private Tween _scaleTween;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // Lock controls at startup until the game officially starts.
        // GameManager's StartNewGame() will call ResetPaddleState() to re-enable this.
        enabled = false;
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Disable();
        }
    }

    private void OnEnable()
    {
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Enable();
        }
        if (goalScoredEvent != null)
        {
            goalScoredEvent.OnEventRaised += OnGoalScored;
        }
        if (gameOverEvent != null)
        {
            gameOverEvent.OnEventRaised += OnGameOver;
        }
    }

    private void OnDisable()
    {
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Disable();
        }
        if (goalScoredEvent != null)
        {
            goalScoredEvent.OnEventRaised -= OnGoalScored;
        }
        if (gameOverEvent != null)
        {
            gameOverEvent.OnEventRaised -= OnGameOver;
        }
    }

    private void OnGameOver()
    {
        _isGameOver = true;
    }

    private void OnGoalScored(int losingPlayerIndex, Vector3 ballPosition)
    {
        if (losingPlayerIndex == playerIndex)
        {
            StartCoroutine(ExplodeAndRespawnRoutine());
        }
    }

    private IEnumerator ExplodeAndRespawnRoutine()
    {
        // 1. Disable controls
        enabled = false;
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Disable();
        }
        _moveDirection = 0f;

        // 2. Hide paddle and disable collision
        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        if (_boxCollider != null) _boxCollider.enabled = false;

        // 3. Wait for delay
        yield return new WaitForSeconds(respawnDelay);

        if (_isGameOver)
        {
            yield break;
        }

        // 4. Reset position Y to 0, start scale from 0
        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;
        
        transform.localScale = Vector3.zero;
        if (_spriteRenderer != null) _spriteRenderer.enabled = true;

        // 5. Scale tween in
        // Determine target scale based on original scale and any powerup multipliers
        Vector3 targetScale = _originalScale;
        targetScale.y *= _speedMultiplier; // Wait, actually powerups set the transform scale directly via SetScaleMultiplier, but it's simpler to just tween to the target Y scale.
        // Wait, to be safe, SetScaleMultiplier will be called during reset, so the paddle will already have the correct multiplier.
        // However, we just tween to `_originalScale`. If a powerup is active, it changes Y. But during RoundReset, powerups are cleared.
        // Let's tween to _originalScale directly.
        bool tweenComplete = false;
        transform.DOScale(_originalScale, respawnDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => tweenComplete = true);

        yield return new WaitUntil(() => tweenComplete);

        // 6. Restore physics, controls, and raise event
        if (_boxCollider != null) _boxCollider.enabled = true;
        enabled = true;
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Enable();
        }

        if (paddleRespawnedEvent != null)
        {
            paddleRespawnedEvent.RaiseEvent();
        }
    }

    private void Update()
    {
        // Read input every frame for responsiveness.
        if (moveInput != null && moveInput.action != null)
        {
            _moveDirection = moveInput.action.ReadValue<float>();
        }
    }

    private void FixedUpdate()
    {
        if (Mathf.Approximately(_moveDirection, 0f)) return;

        float speed = gameSettings.paddleSpeed * _speedMultiplier;
        Vector2 movement = new Vector2(0f, _moveDirection * speed * Time.fixedDeltaTime);
        transform.Translate(movement);

        // Clamp position within bounds.
        Vector3 clampedPos = transform.position;
        clampedPos.y = Mathf.Clamp(clampedPos.y, lowerBound, upperBound);
        transform.position = clampedPos;
    }

    // ─────────────────────── External Modifiers ───────────────────────

    /// <summary>
    /// Instantly resets the paddle to its active, default state.
    /// Used when starting/restarting the game.
    /// </summary>
    public void ResetPaddleState()
    {
        // Stop any running respawn coroutines
        StopAllCoroutines();

        _isGameOver = false;

        // Stop any running tweens
        _scaleTween?.Kill();

        // Reset scale and position
        transform.localScale = _originalScale;
        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;

        // Enable components
        if (_spriteRenderer != null) _spriteRenderer.enabled = true;
        if (_boxCollider != null) _boxCollider.enabled = true;

        enabled = true;
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Enable();
        }
    }

    /// <summary>
    /// Set the speed multiplier. 1f = normal speed. Called by external systems.
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }

    /// <summary>
    /// Set the Y-scale multiplier. 1f = original size. Called by external systems.
    /// </summary>
    public void SetScaleMultiplier(float multiplier)
    {
        _scaleTween?.Kill();

        Vector3 newScale = _originalScale;
        newScale.y *= multiplier;

        Ease easeType = multiplier > 1f ? Ease.OutBack : Ease.InQuad;
        _scaleTween = transform.DOScale(newScale, 0.3f).SetEase(easeType);
    }
}
