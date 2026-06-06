using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns powerup boxes at the center of the screen at random intervals.
/// Starts spawning when the ball is served, stops and clears on round reset.
/// </summary>
public class PowerupSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameSettingsSO gameSettings;

    [Header("References")]
    [SerializeField] private GameObject powerupBoxPrefab;

    [Header("Events — Listen")]
    [SerializeField] private VoidEventChannelSO onBallServed;
    [SerializeField] private VoidEventChannelSO onRoundReset;
    [SerializeField] private VoidEventChannelSO gameOverEvent;

    /// <summary>Tracks all active powerup boxes so we can destroy them on round reset.</summary>
    private readonly List<GameObject> _activeBoxes = new List<GameObject>();
    private Coroutine _spawnRoutine;

    private void OnEnable()
    {
        if (onBallServed != null)
            onBallServed.OnEventRaised += StartSpawning;
        if (onRoundReset != null)
            onRoundReset.OnEventRaised += StopAndClear;
        if (gameOverEvent != null)
            gameOverEvent.OnEventRaised += StopAndClear;
    }

    private void OnDisable()
    {
        if (onBallServed != null)
            onBallServed.OnEventRaised -= StartSpawning;
        if (onRoundReset != null)
            onRoundReset.OnEventRaised -= StopAndClear;
        if (gameOverEvent != null)
            gameOverEvent.OnEventRaised -= StopAndClear;
    }

    private void StartSpawning()
    {
        // Prevent duplicate coroutines.
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void StopAndClear()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        // Destroy all active powerup boxes.
        foreach (GameObject box in _activeBoxes)
        {
            if (box != null)
                Destroy(box);
        }
        _activeBoxes.Clear();
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = Random.Range(gameSettings.powerupSpawnMinInterval, gameSettings.powerupSpawnMaxInterval);
            float elapsed = 0f;

            while (elapsed < delay)
            {
                // Pause the countdown if a box is currently active on the field
                if (HasActiveBox())
                {
                    yield return null;
                    continue;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            SpawnBox();
        }
    }

    private bool HasActiveBox()
    {
        _activeBoxes.RemoveAll(box => box == null);
        return _activeBoxes.Count > 0;
    }

    private void SpawnBox()
    {
        if (powerupBoxPrefab == null) return;

        // Spawn at center X, random Y.
        float randomY = Random.Range(-3f, 3f);
        Vector3 spawnPos = new Vector3(0f, randomY, 0f);

        GameObject box = Instantiate(powerupBoxPrefab, spawnPos, Quaternion.identity);
        _activeBoxes.Add(box);
    }
}
