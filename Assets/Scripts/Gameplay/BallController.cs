using UnityEngine;

/// <summary>
/// Drives the ball's movement each fixed frame and handles reset/serve logic.
/// Listens to a VoidEventChannel to know when to reset.
/// Queries PowerupManager for active ball-speed powerups each frame.
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onRoundReset;
    [SerializeField] private VoidEventChannelSO onBallServed;

    /// <summary>Current movement direction (unit vector).</summary>
    [HideInInspector] public Vector2 velocity;

    /// <summary>Current scalar speed applied to the velocity direction.</summary>
    [HideInInspector] public float currentSpeed;

    /// <summary>
    /// Index of the player who last hit the ball (0 = Left, 1 = Right).
    /// Updated by BallCollisionHandler on paddle and GoalGuard collisions.
    /// </summary>
    [HideInInspector] public int lastHitterIndex;

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
        if (Mathf.Approximately(currentSpeed, 0f)) return;

        float effectiveSpeed = currentSpeed * GetBallSpeedMultiplier();
        transform.Translate(velocity * effectiveSpeed);
    }

    /// <summary>
    /// Calculates a dynamic speed multiplier based on active ball powerups.
    /// </summary>
    private float GetBallSpeedMultiplier()
    {
        if (PowerupManager.Instance == null) return 1f;

        float multiplier = 1f;

        // Check both players for ball-related powerups.
        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            // OpponentFastBall: ball moves faster when heading AWAY from the owner.
            if (PowerupManager.Instance.HasActivePowerup(playerIndex, PowerupType.OpponentFastBall))
            {
                // Player 0 is on the left. Ball moving right (velocity.x > 0) = away from player 0.
                // Player 1 is on the right. Ball moving left (velocity.x < 0) = away from player 1.
                bool movingAwayFromOwner = (playerIndex == 0 && velocity.x > 0f)
                                        || (playerIndex == 1 && velocity.x < 0f);
                if (movingAwayFromOwner)
                {
                    multiplier *= gameSettings.fastBallOpponentMultiplier;
                }
            }

            // FriendlySlowBall: ball slows when on the owner's half.
            if (PowerupManager.Instance.HasActivePowerup(playerIndex, PowerupType.FriendlySlowBall))
            {
                // Player 0's half is X < 0, Player 1's half is X > 0.
                bool onOwnersHalf = (playerIndex == 0 && transform.position.x < 0f)
                                 || (playerIndex == 1 && transform.position.x > 0f);
                if (onOwnersHalf)
                {
                    multiplier *= gameSettings.slowBallFriendlyMultiplier;
                }
            }
        }

        return multiplier;
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

        // Broadcast that the ball has been served so spawners can start.
        if (onBallServed != null)
        {
            onBallServed.RaiseEvent();
        }
    }

    /// <summary>
    /// Snap ball back to center and zero out velocity. Called on round reset.
    /// </summary>
    private void ResetBall()
    {
        transform.position = _startPosition;
        velocity = Vector2.zero;
        currentSpeed = 0f;
        lastHitterIndex = 0;
    }
}
