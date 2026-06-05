using System;
using UnityEngine;

/// <summary>
/// ScriptableObject-based event channel with no parameters.
/// Used for simple signals like game reset or round start.
/// </summary>
[CreateAssetMenu(fileName = "VoidEventChannel", menuName = "Pong/Events/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    /// <summary>
    /// Subscribe to listen for this event.
    /// </summary>
    public event Action OnEventRaised;

    /// <summary>
    /// Broadcast the event to all listeners.
    /// </summary>
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
