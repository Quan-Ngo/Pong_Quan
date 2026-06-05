using TMPro;
using UnityEngine;

/// <summary>
/// Listens to score events and updates TextMeshPro score labels in the UI.
/// </summary>
public class ScoreDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    [Header("Events")]
    [SerializeField] private ScoreEventChannelSO scoreEvent;

    private void OnEnable()
    {
        if (scoreEvent != null)
            scoreEvent.OnEventRaised += OnScoreUpdated;
    }

    private void OnDisable()
    {
        if (scoreEvent != null)
            scoreEvent.OnEventRaised -= OnScoreUpdated;
    }

    private void OnScoreUpdated(int playerIndex, int newScore)
    {
        if (playerIndex == 0 && player1ScoreText != null)
        {
            player1ScoreText.text = newScore.ToString();
        }
        else if (playerIndex == 1 && player2ScoreText != null)
        {
            player2ScoreText.text = newScore.ToString();
        }
    }
}
