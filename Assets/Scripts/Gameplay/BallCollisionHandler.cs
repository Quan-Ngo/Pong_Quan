using UnityEngine;

/// <summary>
/// Handles ball collision responses for walls and paddles.
/// Attached to the same GameObject as BallController.
/// Uses trigger colliders — the ball, walls, and paddles must all have Collider2D (isTrigger = true on ball).
/// </summary>
[RequireComponent(typeof(BallController))]
public class BallCollisionHandler : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("Tags")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private string paddleTag = "Paddle";

    private BallController _ball;

    private void Awake()
    {
        _ball = GetComponent<BallController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(wallTag))
        {
            HandleWallBounce();
        }
        else if (other.CompareTag(paddleTag))
        {
            HandlePaddleBounce(other);
        }
    }

    /// <summary>
    /// Wall bounce: simple Y-velocity inversion.
    /// </summary>
    private void HandleWallBounce()
    {
        _ball.velocity = new Vector2(_ball.velocity.x, -_ball.velocity.y);
    }

    /// <summary>
    /// Paddle bounce: angle determined by where the ball hits the paddle
    /// using the relative intersection method.
    /// </summary>
    private void HandlePaddleBounce(Collider2D paddle)
    {
        // 1. Calculate relative intersection (how far from center the ball hit).
        float paddleCenterY = paddle.transform.position.y;
        float paddleHeight = paddle.bounds.size.y;
        float ballY = transform.position.y;

        float relativeIntersectY = paddleCenterY - ballY;
        float normalizedIntersectY = relativeIntersectY / (paddleHeight / 2f);
        normalizedIntersectY = Mathf.Clamp(normalizedIntersectY, -1f, 1f);

        // 2. Calculate bounce angle.
        float bounceAngleRad = normalizedIntersectY * gameSettings.maxBounceAngle * Mathf.Deg2Rad;

        // 3. Determine horizontal direction (ball should move away from the paddle it just hit).
        float direction = (paddle.transform.position.x < transform.position.x) ? 1f : -1f;

        // 4. Build new velocity unit vector.
        _ball.velocity = new Vector2(direction * Mathf.Cos(bounceAngleRad), -Mathf.Sin(bounceAngleRad)).normalized;

        // 5. Increase speed with cap.
        _ball.currentSpeed = Mathf.Min(
            _ball.currentSpeed * gameSettings.speedMultiplierPerHit,
            gameSettings.maxBallSpeed
        );
    }
}
