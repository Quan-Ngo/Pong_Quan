using UnityEngine;
using DG.Tweening;
using TMPro;

/// <summary>
/// Attached to the Game Over panel. Slides the panel from offscreen
/// to its target anchored position when enabled.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class GameOverPanel : MonoBehaviour
{
    [Header("Tween Settings")]
    [SerializeField] private float startOffsetY = 800f;
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    [Header("Winner Text Hue Cycle")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private float hueDuration = 2f;
    [Range(0f, 1f)] [SerializeField] private float saturation = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float brightness = 1f;

    private RectTransform _rectTransform;
    private Vector2 _targetAnchoredPosition;
    private bool _hasCachedPosition = false;
    private Tween _hueCycleTween;

    private void Awake()
    {
        CachePosition();
    }

    private void CachePosition()
    {
        if (_hasCachedPosition) return;
        _rectTransform = GetComponent<RectTransform>();
        _targetAnchoredPosition = _rectTransform.anchoredPosition;
        _hasCachedPosition = true;
    }

    private void OnEnable()
    {
        CachePosition();

        // Kill active tweens on this RectTransform to prevent overlap issues
        _rectTransform.DOKill();

        // Position panel offscreen (top)
        _rectTransform.anchoredPosition = new Vector2(_targetAnchoredPosition.x, _targetAnchoredPosition.y + startOffsetY);

        // Smoothly tween to the target position
        _rectTransform.DOAnchorPos(_targetAnchoredPosition, duration)
            .SetEase(easeType)
            .SetUpdate(true); // Runs even if timeScale is paused/modified

        // Start Hue Cycle Tween
        if (winnerText != null)
        {
            _hueCycleTween?.Kill();
            
            float hueVal = 0f;
            winnerText.color = Color.HSVToRGB(hueVal, saturation, brightness);
            
            _hueCycleTween = DOTween.To(() => hueVal, x => {
                hueVal = x;
                winnerText.color = Color.HSVToRGB(hueVal, saturation, brightness);
            }, 1f, hueDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);
        }
    }

    private void OnDisable()
    {
        _hueCycleTween?.Kill();
    }

    private void OnDestroy()
    {
        _hueCycleTween?.Kill();
    }
}
