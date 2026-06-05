using TMPro;
using UnityEngine;

/// <summary>
/// Game Over overlay. Activated by the GameOver event channel.
/// Shows the winner and provides a restart button.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI winnerText;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO gameOverEvent;

    private void OnEnable()
    {
        if (gameOverEvent != null)
            gameOverEvent.OnEventRaised += ShowGameOver;
    }

    private void OnDisable()
    {
        if (gameOverEvent != null)
            gameOverEvent.OnEventRaised -= ShowGameOver;
    }

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void ShowGameOver()
    {
        int winnerIndex = gameManager.GetWinnerIndex();
        string winnerName = (winnerIndex == 0) ? "Player 1" : "Player 2";

        if (winnerText != null)
            winnerText.text = $"{winnerName} Wins!";

        if (panel != null)
            panel.SetActive(true);
    }

    /// <summary>
    /// Called from the Restart button's OnClick event.
    /// </summary>
    public void OnRestartClicked()
    {
        if (panel != null)
            panel.SetActive(false);

        gameManager.StartNewGame();
    }
}
