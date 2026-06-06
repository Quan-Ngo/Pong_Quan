# Lessons Learned

## 1. Replacing vs. Layering Visual Effects
- **Correction**: The user pointed out that `GameVisualEffectsManager` was still using the old particle explosions alongside the new vector ring shockwave.
- **Context**: The user asked for a "simple clean vector ring" *instead* of particle explosions. However, the initial implementation left the particle prefab and spawning calls active, running both effects simultaneously.
- **Rule**: When a new feature is requested as an alternative or replacement to an existing feature (e.g., "instead of", "look more like"), proactively disable or completely remove the superseded code, fields, and assets to ensure clean behavior and avoid code bloat.

## 2. Prefab GameObject Instantiation and Typing
- **Correction**: The user pointed out that `shockwavePrefab` was resolving to a script reference instead of an actual GameObject, and the code failed to compile due to implicit cast errors (CS0029) when instantiating a `GameObject` directly into a component type.
- **Context**: The field was defined as `GameObject shockwavePrefab` but was instantiated as `ShockwaveRing shockwave = Instantiate(shockwavePrefab, ...)`. This mismatch breaks compilation and can corrupt scene references in the Unity Inspector when field types are changed.
- **Rule**:
  1. Declare prefabs as `GameObject` fields in serializable fields when they represent full prefab assets.
  2. Instantiate the `GameObject` using `Instantiate(prefab)`, then retrieve component scripts via `GetComponent<T>()`.
  3. After changing serialized field types, programmatically or manually re-verify and save all scene references to prevent missing reference errors.

## 3. Event Handling and Local State Overwrites
- **Correction**: The user pointed out the paddle was still respawning despite the game being over, because the game over flag was being inadvertently cleared.
- **Context**: When responding to a goal being scored, the paddle immediately set `_isGameOver = false` within its `ExplodeAndRespawnRoutine` coroutine. Because event channel execution order can be arbitrary (e.g., `GameManager` might raise the `gameOverEvent` before or after `PaddleController` has started its respawn routine), hardcoding local state resets inside an event response can override global state updates.
- **Rule**: Never arbitrarily reset global state flags (like `_isGameOver`) inside specific event handlers (like taking damage or scoring) unless the intent is specifically to override. State resets should only occur during explicit state-change methods (e.g., `ResetPaddleState` or `StartNewGame`).
