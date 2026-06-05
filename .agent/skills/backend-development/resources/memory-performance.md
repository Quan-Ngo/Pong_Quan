---
skill: Memory & Performance Optimization
description: Managing assets, memory allocations, and performance for game data systems.
---

# ⚙️ Memory & Performance Optimization Skill

**Context:** Use this skill to prevent performance dips, long loading screens, and memory leaks.

## 1. Asset Management
- **Addressables:** Prefer Unity Addressables over `Resources/` to enable asynchronous loading and smaller initial package sizes.
- **Reference Management:** Use `AssetReference` to avoid hard references that pull unnecessary assets into memory.

## 2. Allocation Control
- **Minimize GC:** Avoid creating new objects (e.g., `new List()`) inside the `Update()` loop.
- **Object Pooling:** Use `UnityEngine.Pool` for high-frequency objects (projectiles, floating combat text).
- **Structs:** Use `readonly struct` for small data containers to keep them on the stack.

## 3. Data Performance
- **Search Optimization:** Use `Dictionary<ID, Data>` for O(1) lookups instead of `List.Find()`.
- **Pre-Caching:** Load common data assets (UI sprites, Item templates) at startup or during loading screens.

## 4. Profiling Tools
- **Memory Profiler:** Use to find leaked textures or unreleased ScriptableObject instances.
- **Deep Profile:** Identify which data-parsing methods are causing CPU spikes.
