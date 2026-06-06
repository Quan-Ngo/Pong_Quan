using UnityEngine;

/// <summary>
/// A powerup box that floats vertically at the center of the screen.
/// When the ball enters its trigger, it awards a random powerup to the
/// player who last hit the ball, then destroys itself.
/// </summary>
public class PowerupBox : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("Events")]
    [SerializeField] private PowerupCollectedEventChannelSO powerupCollectedEvent;

    [Header("Bounds")]
    [Tooltip("Maximum Y position the box can reach.")]
    [SerializeField] private float upperBound = 4f;
    [Tooltip("Minimum Y position the box can reach.")]
    [SerializeField] private float lowerBound = -4f;

    [Header("Tags")]
    [SerializeField] private string ballTag = "Ball";

    /// <summary>Current vertical movement direction (+1 or -1).</summary>
    private int _direction = 1;

    private void Start()
    {
        // Randomize starting direction.
        _direction = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector2(0f, _direction * gameSettings.powerupBoxSpeed * Time.fixedDeltaTime));

        // Bounce off bounds.
        if (transform.position.y >= upperBound)
        {
            _direction = -1;
            Vector3 pos = transform.position;
            pos.y = upperBound;
            transform.position = pos;
        }
        else if (transform.position.y <= lowerBound)
        {
            _direction = 1;
            Vector3 pos = transform.position;
            pos.y = lowerBound;
            transform.position = pos;
        }
    }

    [Header("Visuals")]
    [SerializeField] private GameObject explosionVfxPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(ballTag)) return;

        // Determine who last hit the ball.
        BallController ball = other.GetComponent<BallController>();
        if (ball == null) return;

        int collectingPlayer = ball.lastHitterIndex;

        // Pick a random powerup type.
        PowerupType[] types = (PowerupType[])System.Enum.GetValues(typeof(PowerupType));
        PowerupType selectedType = types[Random.Range(0, types.Length)];

        // Broadcast collection.
        if (powerupCollectedEvent != null)
        {
            powerupCollectedEvent.RaiseEvent(collectingPlayer, selectedType);
        }

        if (explosionVfxPrefab != null)
        {
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
