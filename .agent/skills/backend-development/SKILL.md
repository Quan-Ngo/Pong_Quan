---
name: data-system-development
description: Expert capabilities for building Unity Game Data Systems and Persistence Layers.
---

# ⚙️ Data System Development Skill

**Context:** Use this skill package for all Data Modeling, Persistence (Save/Load), and Asset Serialization logic.

## 1. The Core Stack
*   **Framework:** Unity (C#)
*   **Language:** C#
*   **Storage:** ScriptableObjects, JSON, or SQLite
*   **Serialization:** Newtonsoft.Json or Unity JsonUtility

## 2. Reference Modules
Specific "How-To" guides are located in the references folder:

- **[ScriptableObject Modeling](./resources/scriptable-object-modeling.md)**: Data structures, Templates, and Relation logic.
- **[Save/Load Systems](./resources/save-load-systems.md)**: Managing persistent player state and cloud sync.
- **[Data Versioning](./resources/data-versioning.md)**: Handling schema changes in save files across game versions.
- **[Memory & Performance](./resources/memory-performance.md)**: Asset bundling, Addressables, and GC optimization.

## 3. Best Practices
*   **Static Data First:** Define the ScriptableObject templates before writing gameplay logic.
*   **Decoupled Persistence:** Keep save/load logic separate from MonoBehaviours. Use a dedicated Service or Manager.
*   **Memory Efficiency:** Use Structs for high-frequency data and reference assets via GUIDs or Addressables.
