using System;
using UnityEngine;

/// <summary>
/// ScriptableObject-based event channel that fires when the ball enters a goal zone.
/// Carries the index of the player who LOST the point (i.e., the ball passed their paddle).
/// </summary>
[CreateAssetMenu(fileName = "GoalScoredEventChannel", menuName = "Pong/Events/Goal Scored Event Channel")]
public class GoalScoredEventChannelSO : ScriptableObject
{
    /// <summary>
    /// Subscribe to react when a goal is scored. Parameter: losingPlayerIndex, ballPosition.
    /// </summary>
    public event Action<int, Vector3> OnEventRaised;

    /// <summary>
    /// Broadcast that a goal was scored.
    /// </summary>
    /// <param name="losingPlayerIndex">Index of the player who failed to block (0 or 1).</param>
    /// <param name="ballPosition">The world position of the ball when the goal was scored.</param>
    public void RaiseEvent(int losingPlayerIndex, Vector3 ballPosition)
    {
        OnEventRaised?.Invoke(losingPlayerIndex, ballPosition);
    }
}
