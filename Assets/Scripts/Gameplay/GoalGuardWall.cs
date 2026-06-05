using UnityEngine;

/// <summary>
/// Marker component attached to goal guard barrier walls.
/// Stores which player owns this barrier so BallCollisionHandler can
/// update lastHitterIndex when the ball bounces off it.
/// </summary>
public class GoalGuardWall : MonoBehaviour
{
    [Tooltip("Index of the player who owns this goal guard (0 = Left, 1 = Right).")]
    public int ownerPlayerIndex;
}
