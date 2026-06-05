using System;
using UnityEngine;

/// <summary>
/// ScriptableObject-based event channel for powerup collection.
/// Carries the collecting player's index and the type of powerup collected.
/// </summary>
[CreateAssetMenu(fileName = "PowerupCollectedEventChannel", menuName = "Pong/Events/Powerup Collected Event Channel")]
public class PowerupCollectedEventChannelSO : ScriptableObject
{
    /// <summary>
    /// Subscribe to react when a powerup is collected. Parameters: (playerIndex, powerupType).
    /// </summary>
    public event Action<int, PowerupType> OnEventRaised;

    /// <summary>
    /// Broadcast that a powerup was collected.
    /// </summary>
    /// <param name="playerIndex">Index of the player who collected the powerup (0 or 1).</param>
    /// <param name="type">The type of powerup collected.</param>
    public void RaiseEvent(int playerIndex, PowerupType type)
    {
        OnEventRaised?.Invoke(playerIndex, type);
    }
}
