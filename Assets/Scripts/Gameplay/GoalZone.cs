using UnityEngine;

/// <summary>
/// Placed behind each paddle at the screen edge.
/// When the ball enters this trigger zone, a goal is scored against the local player.
/// </summary>
public class GoalZone : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The index of the player who LOSES a point when the ball enters this zone. 0 = Player 1, 1 = Player 2.")]
    [SerializeField] private int losingPlayerIndex;

    [Header("Events")]
    [SerializeField] private GoalScoredEventChannelSO goalScoredEvent;

    [Header("Tags")]
    [SerializeField] private string ballTag = "Ball";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ballTag))
        {
            goalScoredEvent.RaiseEvent(losingPlayerIndex);
        }
    }
}
