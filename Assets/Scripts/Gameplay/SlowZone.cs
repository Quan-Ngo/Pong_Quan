using UnityEngine;

/// <summary>
/// A trigger zone that slows down the ball while it is inside.
/// Enabled by the FriendlySlowBall powerup.
/// </summary>
public class SlowZone : MonoBehaviour
{
    [Tooltip("The speed multiplier applied to the ball while in this zone.")]
    public float multiplier = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            var ball = other.GetComponent<BallController>();
            if (ball != null)
            {
                ball.AddSpeedMultiplier(multiplier);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            var ball = other.GetComponent<BallController>();
            if (ball != null)
            {
                ball.RemoveSpeedMultiplier(multiplier);
            }
        }
    }
}
