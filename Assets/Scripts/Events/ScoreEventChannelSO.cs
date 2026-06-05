using System;
using UnityEngine;

/// <summary>
/// ScriptableObject-based event channel that broadcasts score updates.
/// Carries the player index (0 or 1) and their new score.
/// </summary>
[CreateAssetMenu(fileName = "ScoreEventChannel", menuName = "Pong/Events/Score Event Channel")]
public class ScoreEventChannelSO : ScriptableObject
{
    /// <summary>
    /// Subscribe to receive score updates. Parameters: (playerIndex, newScore).
    /// </summary>
    public event Action<int, int> OnEventRaised;

    /// <summary>
    /// Broadcast a score update.
    /// </summary>
    /// <param name="playerIndex">0 for Player 1, 1 for Player 2.</param>
    /// <param name="newScore">The updated score value.</param>
    public void RaiseEvent(int playerIndex, int newScore)
    {
        OnEventRaised?.Invoke(playerIndex, newScore);
    }
}
