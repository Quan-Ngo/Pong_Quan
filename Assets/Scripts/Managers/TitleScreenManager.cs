using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Manages the Title Screen UI and coordinates the start game animation.
/// </summary>
public class TitleScreenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas titleCanvas;
    [SerializeField] private Transform gameplayUI;
    [SerializeField] private Button playButton;
    [SerializeField] private VoidEventChannelSO startGameEvent;

    [Header("Animation Settings")]
    [SerializeField] private float cameraStartYSlope = -15f;
    [SerializeField] private float cameraEndYSlope = 0f;
    [SerializeField] private float uiOffset = 300f;
    [SerializeField] private float panDuration = 1.5f;

    private Vector3 _originalGameplayUiPos;

    private void Awake()
    {
        // 1. Position the camera at the title screen layout
        if (mainCamera != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            camPos.y = cameraStartYSlope;
            mainCamera.transform.position = camPos;
        }

        // 2. Attach the world camera
        if (titleCanvas != null)
        {
            titleCanvas.worldCamera = mainCamera;
        }

        // 3. Cache and offset gameplay UI so it slides in from above
        if (gameplayUI != null)
        {
            _originalGameplayUiPos = gameplayUI.localPosition;
            gameplayUI.localPosition = new Vector3(_originalGameplayUiPos.x, _originalGameplayUiPos.y + uiOffset, _originalGameplayUiPos.z);
        }

        // 4. Hook up play button
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }
    }

    private void OnDestroy()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlayGame);
        }
    }

    public void PlayGame()
    {
        if (playButton != null)
        {
            playButton.interactable = false;
        }

        Sequence introSequence = DOTween.Sequence();

        // Tween camera position up to the game board
        if (mainCamera != null)
        {
            Vector3 targetCamPos = mainCamera.transform.position;
            targetCamPos.y = cameraEndYSlope;
            introSequence.Join(mainCamera.transform.DOMove(targetCamPos, panDuration).SetEase(Ease.InOutCubic));
        }

        // Tween gameplay UI down into position simultaneously
        if (gameplayUI != null)
        {
            introSequence.Join(gameplayUI.DOLocalMoveY(_originalGameplayUiPos.y, panDuration).SetEase(Ease.InOutCubic));
        }

        // Upon completion, finalize Title Screen cleanup and raise the start event
        introSequence.OnComplete(() =>
        {
            if (titleCanvas != null)
            {
                titleCanvas.gameObject.SetActive(false);
            }

            if (startGameEvent != null)
            {
                startGameEvent.RaiseEvent();
            }
        });
    }
}
