using UnityEngine;
using DG.Tweening;

/// <summary>
/// Controls the animation of an expanding circular shockwave ring
/// that fades out over time.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ShockwaveRing : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(float duration, float startScale, float endScale, Color color)
    {
        transform.localScale = Vector3.one * startScale;
        _spriteRenderer.color = color;

        // Animate expansion using DOScale
        transform.DOScale(endScale, duration).SetEase(Ease.OutQuad);

        // Animate fade out and destroy on completion
        _spriteRenderer.DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}
