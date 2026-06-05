using UnityEngine;

/// <summary>
/// Centralized game configuration. Tune values in the Inspector without touching code.
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Pong/Game Settings")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Scoring")]
    [Tooltip("Points needed to win a match.")]
    public int targetWinScore = 7;

    [Header("Ball")]
    [Tooltip("Ball speed on the first serve.")]
    public float initialBallSpeed = 5f;

    [Tooltip("Absolute maximum ball speed after acceleration.")]
    public float maxBallSpeed = 15f;

    [Tooltip("Multiplier applied to ball speed on every paddle hit.")]
    public float speedMultiplierPerHit = 1.05f;

    [Tooltip("Maximum bounce angle in degrees measured from the horizontal.")]
    [Range(30f, 80f)]
    public float maxBounceAngle = 60f;

    [Header("Paddle")]
    [Tooltip("Paddle movement speed (units per second).")]
    public float paddleSpeed = 8f;

    [Header("Serve")]
    [Tooltip("Delay in seconds before the ball is served after a point is scored.")]
    public float serveDelay = 1f;

    [Header("Powerups — Spawning")]
    [Tooltip("Minimum seconds between powerup box spawns.")]
    public float powerupSpawnMinInterval = 4f;

    [Tooltip("Maximum seconds between powerup box spawns.")]
    public float powerupSpawnMaxInterval = 8f;

    [Tooltip("Duration in seconds a powerup remains active after collection.")]
    public float powerupDuration = 7f;

    [Tooltip("Vertical movement speed of the powerup box.")]
    public float powerupBoxSpeed = 2.5f;

    [Header("Powerups — Paddle")]
    [Tooltip("Scale multiplier applied to paddle length during PaddleLengthened.")]
    public float paddleLengthMultiplier = 1.5f;

    [Tooltip("Speed multiplier applied to paddle during PaddleSpeedBoost.")]
    public float paddleSpeedMultiplier = 1.4f;

    [Header("Powerups — Ball")]
    [Tooltip("Speed multiplier when ball moves towards the opponent (OpponentFastBall).")]
    public float fastBallOpponentMultiplier = 1.5f;

    [Tooltip("Speed multiplier when ball is on the owner's half (FriendlySlowBall).")]
    public float slowBallFriendlyMultiplier = 0.6f;
}
