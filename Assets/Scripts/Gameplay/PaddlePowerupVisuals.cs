using UnityEngine;

/// <summary>
/// Manages the visual feedback effects for paddle-related powerups.
/// This decouples particle system generation and playback from the core PaddleController.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PaddlePowerupVisuals : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _speedBoostVfx;
    private ParticleSystem _fastBallVfx;

    [Tooltip("Prefab for the Speed Boost electric arcs effect.")]
    [SerializeField] private GameObject speedBoostVfxPrefab;
    
    [Tooltip("Prefab for the Fast Ball fiery aura effect.")]
    [SerializeField] private GameObject fastBallVfxPrefab;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ─────────────────────── Speed Boost VFX ───────────────────────

    public void SetSpeedBoostVfxActive(bool active)
    {
        if (_speedBoostVfx == null && speedBoostVfxPrefab != null)
        {
            GameObject vfxObj = Instantiate(speedBoostVfxPrefab, transform);
            _speedBoostVfx = vfxObj.GetComponent<ParticleSystem>();
            
            // Re-assign the shape module to emit from this specific paddle's sprite renderer
            if (_speedBoostVfx != null)
            {
                var shape = _speedBoostVfx.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
                shape.spriteRenderer = _spriteRenderer;
            }
        }

        if (_speedBoostVfx != null)
        {
            if (active) _speedBoostVfx.Play();
            else _speedBoostVfx.Stop();
        }
    }

    // ─────────────────────── Fast Ball VFX ───────────────────────

    public void SetFastBallVfxActive(bool active)
    {
        if (_fastBallVfx == null && fastBallVfxPrefab != null)
        {
            GameObject vfxObj = Instantiate(fastBallVfxPrefab, transform);
            _fastBallVfx = vfxObj.GetComponent<ParticleSystem>();

            // Re-assign the shape module to emit from this specific paddle's sprite renderer
            if (_fastBallVfx != null)
            {
                var shape = _fastBallVfx.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
                shape.spriteRenderer = _spriteRenderer;
            }
        }

        if (_fastBallVfx != null)
        {
            if (active) _fastBallVfx.Play();
            else _fastBallVfx.Stop();
        }
    }
}
