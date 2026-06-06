using System;
using UnityEngine;

/// <summary>
/// ScriptableObject-based event channel that fires when the ball collides with a paddle, wall, or goal guard.
/// Carries the collision position, ball speed, and the tag of the hit object.
/// </summary>
[CreateAssetMenu(fileName = "BallCollisionEventChannel", menuName = "Pong/Events/Ball Collision Event Channel")]
public class BallCollisionEventChannelSO : ScriptableObject
{
    /// <summary>
    /// Subscribe to react when a collision occurs. Parameters: collisionPosition, ballSpeed, hitTag.
    /// </summary>
    public event Action<Vector3, float, string> OnEventRaised;

    /// <summary>
    /// Broadcast that a collision occurred.
    /// </summary>
    public void RaiseEvent(Vector3 position, float speed, string hitTag)
    {
        OnEventRaised?.Invoke(position, speed, hitTag);
    }
}
