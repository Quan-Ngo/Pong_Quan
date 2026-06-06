using DG.Tweening;
using UnityEngine;

/// <summary>
/// Handles all visual feedback in the game, such as camera shake, 
/// particle explosions, and background flashes based on ball collisions.
/// </summary>
public class GameVisualEffectsManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private BallCollisionEventChannelSO ballCollisionEventChannel;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer bgRenderer;

    [Header("Camera Shake Settings")]
    [SerializeField] private float baseShakeDuration = 0.2f;
    [SerializeField] private float baseShakeStrength = 0.15f;
    [SerializeField] private float baseRollStrength = 3f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;

    [Header("Background Flash Settings")]
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float backGroundFlashBrightness = 0.1f;

    [Header("Shockwave Settings")]
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private float baseShockwaveDuration = 0.3f;
    [SerializeField] private float baseShockwaveEndScale = 2.5f;

    [Header("Color Saturation Settings")]
    [SerializeField] private float initialSaturationValue = 0.2f;
    [SerializeField] private float speedScaleFactor = 0.8f;

    private Vector3 _cameraStartPos;
    private Quaternion _cameraStartRot;
    private Tween _cameraTween;
    private Tween _bgTween;

    private void Awake()
    {
        if (mainCamera != null)
        {
            _cameraStartPos = mainCamera.transform.localPosition;
            _cameraStartRot = mainCamera.transform.localRotation;
        }
    }

    private void OnEnable()
    {
        if (ballCollisionEventChannel != null)
        {
            ballCollisionEventChannel.OnEventRaised += HandleBallCollision;
        }
    }

    private void OnDisable()
    {
        if (ballCollisionEventChannel != null)
        {
            ballCollisionEventChannel.OnEventRaised -= HandleBallCollision;
        }
        
        _cameraTween?.Kill();
        _bgTween?.Kill();
    }

    private void HandleBallCollision(Vector3 position, float speed, string hitTag)
    {
        // Calculate normalized speed, clamped between 0 and 1.
        float normalizedSpeed = 0f;
        if (gameSettings != null)
        {
            float range = gameSettings.maxBallSpeed - gameSettings.initialBallSpeed;
            if (range > 0f)
            {
                normalizedSpeed = Mathf.Clamp01((speed - gameSettings.initialBallSpeed) / range);
            }
        }

        TriggerCameraShake(normalizedSpeed);
        TriggerBackgroundFlash(normalizedSpeed);
        SpawnShockwave(position, normalizedSpeed);
    }

    private void TriggerCameraShake(float normalizedSpeed)
    {
        if (mainCamera == null) return;

        // Scale shake intensity based on speed
        float posStrength = baseShakeStrength * (1f + normalizedSpeed);
        float rollStrength = baseRollStrength * (1f + normalizedSpeed);

        // Kill active tween and reset to starting position to prevent drifting
        _cameraTween?.Kill();
        mainCamera.transform.localPosition = _cameraStartPos;
        mainCamera.transform.localRotation = _cameraStartRot;

        Sequence shakeSequence = DOTween.Sequence();
        shakeSequence.Join(mainCamera.transform.DOShakePosition(baseShakeDuration, posStrength, shakeVibrato, shakeRandomness, fadeOut: true));
        shakeSequence.Join(mainCamera.transform.DOShakeRotation(baseShakeDuration, new Vector3(0, 0, rollStrength), shakeVibrato, shakeRandomness, fadeOut: true));
        
        _cameraTween = shakeSequence;
    }

    private void TriggerBackgroundFlash(float normalizedSpeed)
    {
        if (bgRenderer == null) return;

        Color targetColor = GenerateSpeedScaledColor(normalizedSpeed, backGroundFlashBrightness);

        _bgTween?.Kill();
        bgRenderer.color = targetColor;
        _bgTween = bgRenderer.DOColor(Color.black, flashDuration).SetEase(Ease.OutQuad);
    }

    private void SpawnShockwave(Vector3 position, float normalizedSpeed)
    {
        if (shockwavePrefab == null) return;

        GameObject shockwaveObj = Instantiate(shockwavePrefab, position, Quaternion.identity);
        if (shockwaveObj == null) return;
        
        ShockwaveRing shockwave = shockwaveObj.GetComponent<ShockwaveRing>();
        if (shockwave == null) return;

        Color shockwaveColor = GenerateSpeedScaledColor(normalizedSpeed);

        float duration = baseShockwaveDuration;
        float startScale = 0.1f;
        float endScale = baseShockwaveEndScale * (0.8f + 0.6f * normalizedSpeed);

        shockwave.Initialize(duration, startScale, endScale, shockwaveColor);
    }

    private Color GenerateSpeedScaledColor(float normalizedSpeed, float value = 0.9f)
    {
        float hue = Random.Range(0f, 1f);
        float saturation = Mathf.Clamp01((initialSaturationValue + normalizedSpeed) * speedScaleFactor);
        Color color = Color.HSVToRGB(hue, saturation, value);
        color.a = 1f; // Ensure alpha is fully opaque
        return color;
    }
}
