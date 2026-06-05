using UnityEngine;

/// <summary>
/// Drives the ball's movement each fixed frame and handles reset/serve logic.
/// Listens to a VoidEventChannel to know when to reset.
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onRoundReset;

    /// <summary>Current movement direction (unit vector).</summary>
    [HideInInspector] public Vector2 velocity;

    /// <summary>Current scalar speed applied to the velocity direction.</summary>
    [HideInInspector] public float currentSpeed;

    /// <summary>Serve direction set by the GameManager before serve (+1 = right, -1 = left).</summary>
    private int _serveDirection = 1;

    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = transform.position;
    }

    private void OnEnable()
    {
        if (onRoundReset != null)
            onRoundReset.OnEventRaised += ResetBall;
    }

    private void OnDisable()
    {
        if (onRoundReset != null)
            onRoundReset.OnEventRaised -= ResetBall;
    }

    private void FixedUpdate()
    {
        transform.Translate(velocity * currentSpeed);
    }

    /// <summary>
    /// Set the serve direction before calling Serve().
    /// +1 serves the ball to the right, -1 serves to the left.
    /// </summary>
    public void SetServeDirection(int direction)
    {
        _serveDirection = direction >= 0 ? 1 : -1;
    }

    /// <summary>
    /// Launch the ball from center in the pre-set serve direction at initial speed.
    /// </summary>
    public void Serve()
    {
        currentSpeed = gameSettings.initialBallSpeed;

        // Serve at a slight random angle to avoid perfectly horizontal volleys.
        float randomAngle = Random.Range(-15f, 15f) * Mathf.Deg2Rad;
        velocity = new Vector2(_serveDirection * Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
    }

    /// <summary>
    /// Snap ball back to center and zero out velocity. Called on round reset.
    /// </summary>
    private void ResetBall()
    {
        transform.position = _startPosition;
        velocity = Vector2.zero;
        currentSpeed = 0f;
    }
}
