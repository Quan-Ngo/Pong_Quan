using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central powerup state manager. Tracks active powerups per player,
/// handles timers, and applies/reverts effects by calling into paddles and ball.
/// Supports multiple concurrent powerups on both players independently.
/// </summary>
public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("References")]
    [SerializeField] private PaddleController[] paddles; // Index 0 = Left, 1 = Right
    [SerializeField] private BallController ball;

    [Header("Goal Guard")]
    [Tooltip("Goal guard wall objects for Player 0 (Left). Disabled by default.")]
    [SerializeField] private GameObject[] goalGuardWallsPlayer0;
    [Tooltip("Goal guard wall objects for Player 1 (Right). Disabled by default.")]
    [SerializeField] private GameObject[] goalGuardWallsPlayer1;

    [Header("Events — Listen")]
    [SerializeField] private PowerupCollectedEventChannelSO powerupCollectedEvent;
    [SerializeField] private VoidEventChannelSO onRoundReset;

    /// <summary>
    /// Tracks active powerup coroutines: [playerIndex][powerupType] → running coroutine.
    /// </summary>
    private Dictionary<int, Dictionary<PowerupType, Coroutine>> _activePowerups;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _activePowerups = new Dictionary<int, Dictionary<PowerupType, Coroutine>>
        {
            { 0, new Dictionary<PowerupType, Coroutine>() },
            { 1, new Dictionary<PowerupType, Coroutine>() }
        };
    }

    private void OnEnable()
    {
        if (powerupCollectedEvent != null)
            powerupCollectedEvent.OnEventRaised += OnPowerupCollected;
        if (onRoundReset != null)
            onRoundReset.OnEventRaised += ClearAllPowerups;
    }

    private void OnDisable()
    {
        if (powerupCollectedEvent != null)
            powerupCollectedEvent.OnEventRaised -= OnPowerupCollected;
        if (onRoundReset != null)
            onRoundReset.OnEventRaised -= ClearAllPowerups;
    }

    // ─────────────────────── Public Queries ───────────────────────

    /// <summary>
    /// Returns true if the given player currently has the specified powerup active.
    /// Used by BallController to adjust speed dynamically.
    /// </summary>
    public bool HasActivePowerup(int playerIndex, PowerupType type)
    {
        return _activePowerups.ContainsKey(playerIndex)
            && _activePowerups[playerIndex].ContainsKey(type);
    }

    // ─────────────────────── Collection ───────────────────────

    private void OnPowerupCollected(int playerIndex, PowerupType type)
    {
        // If already active, cancel the old timer and restart (refresh duration, don't stack).
        if (_activePowerups[playerIndex].TryGetValue(type, out Coroutine existing))
        {
            StopCoroutine(existing);
            _activePowerups[playerIndex].Remove(type);
            // We don't revert here — the effect is already applied, just extend duration.
        }
        else
        {
            ApplyEffect(playerIndex, type);
        }

        Coroutine timer = StartCoroutine(PowerupTimer(playerIndex, type));
        _activePowerups[playerIndex][type] = timer;
    }

    private IEnumerator PowerupTimer(int playerIndex, PowerupType type)
    {
        yield return new WaitForSeconds(gameSettings.powerupDuration);

        RevertEffect(playerIndex, type);
        _activePowerups[playerIndex].Remove(type);
    }

    // ─────────────────────── Apply / Revert ───────────────────────

    private void ApplyEffect(int playerIndex, PowerupType type)
    {
        PaddleController paddle = paddles[playerIndex];

        switch (type)
        {
            case PowerupType.PaddleLengthened:
                paddle.SetScaleMultiplier(gameSettings.paddleLengthMultiplier);
                break;

            case PowerupType.PaddleSpeedBoost:
                paddle.SetSpeedMultiplier(gameSettings.paddleSpeedMultiplier);
                break;

            case PowerupType.OpponentFastBall:
                // Ball speed is checked dynamically in BallController.FixedUpdate via HasActivePowerup.
                break;

            case PowerupType.FriendlySlowBall:
                // Ball speed is checked dynamically in BallController.FixedUpdate via HasActivePowerup.
                break;

            case PowerupType.GoalGuard:
                SetGoalGuardActive(playerIndex, true);
                break;
        }
    }

    private void RevertEffect(int playerIndex, PowerupType type)
    {
        PaddleController paddle = paddles[playerIndex];

        switch (type)
        {
            case PowerupType.PaddleLengthened:
                paddle.SetScaleMultiplier(1f);
                break;

            case PowerupType.PaddleSpeedBoost:
                paddle.SetSpeedMultiplier(1f);
                break;

            case PowerupType.OpponentFastBall:
                // No revert needed — dynamic check stops naturally.
                break;

            case PowerupType.FriendlySlowBall:
                // No revert needed — dynamic check stops naturally.
                break;

            case PowerupType.GoalGuard:
                SetGoalGuardActive(playerIndex, false);
                break;
        }
    }

    private void SetGoalGuardActive(int playerIndex, bool active)
    {
        GameObject[] walls = playerIndex == 0 ? goalGuardWallsPlayer0 : goalGuardWallsPlayer1;
        if (walls == null) return;

        foreach (GameObject wall in walls)
        {
            if (wall != null)
                wall.SetActive(active);
        }
    }

    // ─────────────────────── Cleanup ───────────────────────

    /// <summary>
    /// Immediately reverts and clears all active powerups for both players.
    /// Called on round reset.
    /// </summary>
    private void ClearAllPowerups()
    {
        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            // Copy keys to avoid modifying dictionary during iteration.
            var activeTypes = new List<PowerupType>(_activePowerups[playerIndex].Keys);

            foreach (PowerupType type in activeTypes)
            {
                if (_activePowerups[playerIndex].TryGetValue(type, out Coroutine coroutine))
                {
                    StopCoroutine(coroutine);
                }
                RevertEffect(playerIndex, type);
            }

            _activePowerups[playerIndex].Clear();
        }
    }
}
