using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Bounds")]
    [Tooltip("Maximum Y position the paddle center can reach.")]
    [SerializeField] private float upperBound = 4f;
    [Tooltip("Minimum Y position the paddle center can reach.")]
    [SerializeField] private float lowerBound = -4f;

    private float _moveDirection;
    private float _speedMultiplier = 1f;
    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Disable();
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
        Vector3 newScale = _originalScale;
        newScale.y *= multiplier;
        transform.localScale = newScale;
    }
}
