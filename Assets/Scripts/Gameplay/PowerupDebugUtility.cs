using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Editor-only debug utility to trigger powerups on demand.
/// Pressing 1-5 will activate one of the 5 powerups for the target player.
/// </summary>
public class PowerupDebugUtility : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Configuration")]
    [SerializeField] private PowerupCollectedEventChannelSO powerupCollectedEvent;
    
    [Tooltip("Which player will receive the powerup when triggered (0 = Left, 1 = Right).")]
    [Range(0, 1)]
    [SerializeField] private int targetPlayerIndex = 0;

    private void Update()
    {
        if (powerupCollectedEvent == null) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) TriggerPowerup(PowerupType.PaddleLengthened);
        else if (keyboard.digit2Key.wasPressedThisFrame) TriggerPowerup(PowerupType.PaddleSpeedBoost);
        else if (keyboard.digit3Key.wasPressedThisFrame) TriggerPowerup(PowerupType.OpponentFastBall);
        else if (keyboard.digit4Key.wasPressedThisFrame) TriggerPowerup(PowerupType.FriendlySlowBall);
        else if (keyboard.digit5Key.wasPressedThisFrame) TriggerPowerup(PowerupType.GoalGuard);
    }

    private void TriggerPowerup(PowerupType type)
    {
        Debug.Log($"[PowerupDebugUtility] Raising debug powerup collection event for Player {targetPlayerIndex}: {type}");
        powerupCollectedEvent.RaiseEvent(targetPlayerIndex, type);
    }
#endif
}
