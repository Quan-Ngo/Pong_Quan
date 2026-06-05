using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls a paddle's vertical movement using the new Input System.
/// Assign the matching Input Action in the Inspector (Player 1: W/S, Player 2: Up/Down).
/// </summary>
public class PaddleController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("Input")]
    [SerializeField] private InputActionReference moveInput;

    [Header("Bounds")]
    [Tooltip("Maximum Y position the paddle center can reach.")]
    [SerializeField] private float upperBound = 4f;
    [Tooltip("Minimum Y position the paddle center can reach.")]
    [SerializeField] private float lowerBound = -4f;

    private float _moveDirection;

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

        Vector2 movement = new Vector2(0f, _moveDirection * gameSettings.paddleSpeed * Time.fixedDeltaTime);
        transform.Translate(movement);

        // Clamp position within bounds.
        Vector3 clampedPos = transform.position;
        clampedPos.y = Mathf.Clamp(clampedPos.y, lowerBound, upperBound);
        transform.position = clampedPos;
    }
}
