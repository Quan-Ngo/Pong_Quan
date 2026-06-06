using System.Collections;
using UnityEngine;

/// <summary>
/// Central game state controller. Tracks scores, determines serve direction,
/// checks for win conditions, and orchestrates round flow.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("References")]
    [SerializeField] private BallController ball;

    [Header("Event Channels — Listen")]
    [SerializeField] private GoalScoredEventChannelSO goalScoredEvent;
    [SerializeField] private VoidEventChannelSO paddleRespawnedEvent;

    [Header("Event Channels — Broadcast")]
    [SerializeField] private ScoreEventChannelSO scoreEvent;
    [SerializeField] private VoidEventChannelSO roundResetEvent;
    [SerializeField] private VoidEventChannelSO gameOverEvent;

    private int[] _scores = new int[2];
    private bool _gameOver;

    private void OnEnable()
    {
        if (goalScoredEvent != null)
            goalScoredEvent.OnEventRaised += OnGoalScored;
        if (gameOverEvent != null)
            gameOverEvent.OnEventRaised += OnGameOver;
        if (paddleRespawnedEvent != null)
            paddleRespawnedEvent.OnEventRaised += OnPaddleRespawned;
    }

    private void OnDisable()
    {
        if (goalScoredEvent != null)
            goalScoredEvent.OnEventRaised -= OnGoalScored;
        if (gameOverEvent != null)
            gameOverEvent.OnEventRaised -= OnGameOver;
        if (paddleRespawnedEvent != null)
            paddleRespawnedEvent.OnEventRaised -= OnPaddleRespawned;
    }

    private void Start()
    {
        StartNewGame();
    }

    /// <summary>
    /// Reset scores and serve the first ball.
    /// </summary>
    public void StartNewGame()
    {
        _gameOver = false;
        _scores[0] = 0;
        _scores[1] = 0;

        scoreEvent.RaiseEvent(0, 0);
        scoreEvent.RaiseEvent(1, 0);

        // First serve goes to the right (towards Player 2) by default.
        ball.SetServeDirection(Random.Range(-1, 1));
        StartCoroutine(ServeAfterDelay());
    }

    private void OnGoalScored(int losingPlayerIndex, Vector3 ballPosition)
    {
        if (_gameOver) return;

        // The scoring player is the opponent of the losing player.
        int scoringPlayerIndex = (losingPlayerIndex == 0) ? 1 : 0;
        _scores[scoringPlayerIndex]++;

        scoreEvent.RaiseEvent(scoringPlayerIndex, _scores[scoringPlayerIndex]);

        // Reset ball to center.
        roundResetEvent.RaiseEvent();

        // Check win condition.
        if (_scores[scoringPlayerIndex] >= gameSettings.targetWinScore)
        {
            _gameOver = true;
            gameOverEvent.RaiseEvent();
            return;
        }

        // Serve towards the player who lost the point.
        // Player 0 is on the left (-1), Player 1 is on the right (+1).
        int serveDirection = (losingPlayerIndex == 0) ? -1 : 1;
        ball.SetServeDirection(serveDirection);
        
        // Note: Serve is now deferred until the paddle respawns.
        // OnPaddleRespawned will trigger the serve timer.
    }

    private void OnPaddleRespawned()
    {
        if (!_gameOver)
        {
            StartCoroutine(ServeAfterDelay());
        }
    }

    private IEnumerator ServeAfterDelay()
    {
        yield return new WaitForSeconds(gameSettings.serveDelay);
        if (!_gameOver)
        {
            ball.Serve();
        }
    }

    private void OnGameOver()
    {
        _gameOver = true;
    }

    /// <summary>
    /// Returns the index of the winning player (0 or 1), or -1 if no winner yet.
    /// </summary>
    public int GetWinnerIndex()
    {
        for (int i = 0; i < _scores.Length; i++)
        {
            if (_scores[i] >= gameSettings.targetWinScore)
                return i;
        }
        return -1;
    }
}
