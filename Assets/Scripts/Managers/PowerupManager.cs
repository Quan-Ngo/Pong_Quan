using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    [Header("Slow Zone")]
    [Tooltip("Slow zone objects for Player 0 (Left). Disabled by default.")]
    [SerializeField] private GameObject[] slowZonesPlayer0;
    [Tooltip("Slow zone objects for Player 1 (Right). Disabled by default.")]
    [SerializeField] private GameObject[] slowZonesPlayer1;

    [Header("Events — Listen")]
    [SerializeField] private PowerupCollectedEventChannelSO powerupCollectedEvent;
    [SerializeField] private VoidEventChannelSO onRoundReset;

    /// <summary>
    /// Tracks active powerup coroutines: [playerIndex][powerupType] → running coroutine.
    /// </summary>
    private Dictionary<int, Dictionary<PowerupType, Coroutine>> _activePowerups;

    // Cache for visual tweens and default states
    private Dictionary<GameObject, float> _slowZoneDefaultAlphas = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Tween> _slowZoneTweens = new Dictionary<GameObject, Tween>();
    private Dictionary<GameObject, Vector3> _goalGuardDefaultPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Tween> _goalGuardTweens = new Dictionary<GameObject, Tween>();

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

    private bool _isFastBallActiveOnBall = false;

    private void Start()
    {
        CacheGoalGuards(goalGuardWallsPlayer0);
        CacheGoalGuards(goalGuardWallsPlayer1);
        CacheSlowZones(slowZonesPlayer0);
        CacheSlowZones(slowZonesPlayer1);
    }

    private void CacheGoalGuards(GameObject[] walls)
    {
        if (walls == null) return;
        foreach (GameObject wall in walls)
        {
            if (wall != null)
            {
                _goalGuardDefaultPositions[wall] = wall.transform.localPosition;
                wall.SetActive(false);
            }
        }
    }

    private void CacheSlowZones(GameObject[] zones)
    {
        if (zones == null) return;
        foreach (GameObject zone in zones)
        {
            if (zone != null)
            {
                SpriteRenderer spriteRenderer = zone.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    _slowZoneDefaultAlphas[zone] = spriteRenderer.color.a;
                }
                else
                {
                    _slowZoneDefaultAlphas[zone] = 1f;
                }
                zone.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        if (powerupCollectedEvent != null)
            powerupCollectedEvent.OnEventRaised += OnPowerupCollected;
        if (onRoundReset != null)
            onRoundReset.OnEventRaised += ClearAllPowerups;
        if (ball != null)
            ball.OnLastHitterChanged += HandleLastHitterChanged;
    }

    private void OnDisable()
    {
        if (powerupCollectedEvent != null)
            powerupCollectedEvent.OnEventRaised -= OnPowerupCollected;
        if (onRoundReset != null)
            onRoundReset.OnEventRaised -= ClearAllPowerups;
        if (ball != null)
            ball.OnLastHitterChanged -= HandleLastHitterChanged;
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
        PaddlePowerupVisuals visuals = paddle.GetComponent<PaddlePowerupVisuals>();

        switch (type)
        {
            case PowerupType.PaddleLengthened:
                paddle.SetScaleMultiplier(gameSettings.paddleLengthMultiplier);
                break;

            case PowerupType.PaddleSpeedBoost:
                paddle.SetSpeedMultiplier(gameSettings.paddleSpeedMultiplier);
                if (visuals != null) visuals.SetSpeedBoostVfxActive(true);
                break;

            case PowerupType.OpponentFastBall:
                UpdateFastBallMultiplier();
                if (visuals != null) visuals.SetFastBallVfxActive(true);
                break;

            case PowerupType.FriendlySlowBall:
                SetSlowZoneActive(playerIndex, true);
                break;

            case PowerupType.GoalGuard:
                SetGoalGuardActive(playerIndex, true);
                break;
        }
    }

    private void RevertEffect(int playerIndex, PowerupType type)
    {
        PaddleController paddle = paddles[playerIndex];
        PaddlePowerupVisuals visuals = paddle.GetComponent<PaddlePowerupVisuals>();

        switch (type)
        {
            case PowerupType.PaddleLengthened:
                paddle.SetScaleMultiplier(1f);
                break;

            case PowerupType.PaddleSpeedBoost:
                paddle.SetSpeedMultiplier(1f);
                if (visuals != null) visuals.SetSpeedBoostVfxActive(false);
                break;

            case PowerupType.OpponentFastBall:
                UpdateFastBallMultiplier();
                if (visuals != null) visuals.SetFastBallVfxActive(false);
                break;

            case PowerupType.FriendlySlowBall:
                SetSlowZoneActive(playerIndex, false);
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
            if (wall == null) continue;

            if (_goalGuardTweens.TryGetValue(wall, out Tween activeTween))
            {
                activeTween?.Kill();
                _goalGuardTweens.Remove(wall);
            }

            if (!_goalGuardDefaultPositions.TryGetValue(wall, out Vector3 defaultPos))
            {
                defaultPos = wall.transform.localPosition;
            }

            if (active)
            {
                wall.SetActive(true);
                Vector3 offScreenPos = defaultPos;
                offScreenPos.y = defaultPos.y >= 0f ? 7f : -7f;
                wall.transform.localPosition = offScreenPos;

                Tween tween = wall.transform.DOLocalMove(defaultPos, 0.5f).SetEase(Ease.OutBack);
                _goalGuardTweens[wall] = tween;
            }
            else
            {
                Vector3 offScreenPos = defaultPos;
                offScreenPos.y = defaultPos.y >= 0f ? 7f : -7f;

                Tween tween = wall.transform.DOLocalMove(offScreenPos, 0.5f).SetEase(Ease.InQuad)
                    .OnComplete(() => wall.SetActive(false));
                _goalGuardTweens[wall] = tween;
            }
        }
    }

    private void SetSlowZoneActive(int playerIndex, bool active)
    {
        GameObject[] zones = playerIndex == 0 ? slowZonesPlayer0 : slowZonesPlayer1;
        if (zones == null) return;

        foreach (GameObject zone in zones)
        {
            if (zone == null) continue;

            if (_slowZoneTweens.TryGetValue(zone, out Tween activeTween))
            {
                activeTween?.Kill();
                _slowZoneTweens.Remove(zone);
            }

            SpriteRenderer spriteRenderer = zone.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null) continue;

            float defaultAlpha = _slowZoneDefaultAlphas.ContainsKey(zone) ? _slowZoneDefaultAlphas[zone] : 1f;

            if (active)
            {
                zone.SetActive(true);
                Color col = spriteRenderer.color;
                col.a = 0f;
                spriteRenderer.color = col;

                Tween tween = spriteRenderer.DOFade(defaultAlpha, 0.5f).SetEase(Ease.OutQuad);
                _slowZoneTweens[zone] = tween;
            }
            else
            {
                Tween tween = spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InQuad)
                    .OnComplete(() => zone.SetActive(false));
                _slowZoneTweens[zone] = tween;
            }
        }
    }

    private void HandleLastHitterChanged(int hitterIndex)
    {
        UpdateFastBallMultiplier();
    }

    private void UpdateFastBallMultiplier()
    {
        // If the last hitter has the OpponentFastBall powerup, they get the fast ball.
        bool shouldBeFast = HasActivePowerup(ball.lastHitterIndex, PowerupType.OpponentFastBall);

        if (shouldBeFast && !_isFastBallActiveOnBall)
        {
            ball.AddSpeedMultiplier(gameSettings.fastBallOpponentMultiplier);
            _isFastBallActiveOnBall = true;
        }
        else if (!shouldBeFast && _isFastBallActiveOnBall)
        {
            ball.RemoveSpeedMultiplier(gameSettings.fastBallOpponentMultiplier);
            _isFastBallActiveOnBall = false;
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

        if (_isFastBallActiveOnBall)
        {
            ball.RemoveSpeedMultiplier(gameSettings.fastBallOpponentMultiplier);
            _isFastBallActiveOnBall = false;
        }

        foreach (var kvp in _goalGuardTweens) { kvp.Value?.Kill(); }
        _goalGuardTweens.Clear();
        ResetAllGoalGuards();

        foreach (var kvp in _slowZoneTweens) { kvp.Value?.Kill(); }
        _slowZoneTweens.Clear();
        ResetAllSlowZones();
    }

    private void ResetAllGoalGuards()
    {
        ResetGoalGuardPositions(goalGuardWallsPlayer0);
        ResetGoalGuardPositions(goalGuardWallsPlayer1);
    }

    private void ResetGoalGuardPositions(GameObject[] walls)
    {
        if (walls == null) return;
        foreach (GameObject wall in walls)
        {
            if (wall != null)
            {
                if (_goalGuardDefaultPositions.TryGetValue(wall, out Vector3 defaultPos))
                    wall.transform.localPosition = defaultPos;
                wall.SetActive(false);
            }
        }
    }

    private void ResetAllSlowZones()
    {
        ResetSlowZoneAlphas(slowZonesPlayer0);
        ResetSlowZoneAlphas(slowZonesPlayer1);
    }

    private void ResetSlowZoneAlphas(GameObject[] zones)
    {
        if (zones == null) return;
        foreach (GameObject zone in zones)
        {
            if (zone != null)
            {
                var sr = zone.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && _slowZoneDefaultAlphas.TryGetValue(zone, out float defaultAlpha))
                {
                    Color col = sr.color;
                    col.a = defaultAlpha;
                    sr.color = col;
                }
                zone.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var tween in _goalGuardTweens.Values) tween?.Kill();
        foreach (var tween in _slowZoneTweens.Values) tween?.Kill();
    }
}
