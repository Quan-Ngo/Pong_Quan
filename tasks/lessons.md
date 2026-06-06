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
