---
skill: ScriptableObject Modeling
description: Advanced data modeling and template design using Unity ScriptableObjects.
---

# ⚙️ ScriptableObject Modeling Skill

**Context:** Use this skill when designing data containers, item templates, or configuration assets.

## 1. Data Modeling Standards
- **Use SerializeField:** Keep fields private but accessible to the Inspector:
    ```csharp
    [SerializeField] private string itemName;
    [SerializeField] private int baseHealth;
    ```
- **GUIDs:** Use internal GUIDs or `AssetDatabase` paths for unique identification. Avoid simple integer IDs for game entities to prevent merge conflicts.
- **Naming:**
    - Classes: `PascalCase` (e.g., `WeaponData`, `EnemyTemplate`).
    - Assets: Use a prefix (e.g., `SO_Sword_Iron.asset`).
    - Avoid context repetition (e.g., use `name` instead of `weaponName` inside `WeaponData`).

## 2. Deep Structures (Nesting & Collections)
Use **Classes/Structs** with `[Serializable]` for nested data.
```csharp
[Serializable]
public class StatModifier {
    public StatType type;
    public float value;
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
public class ItemData : ScriptableObject {
    public List<StatModifier> modifiers;
}
```

## 3. Reference Data & Facades
Design ScriptableObjects as the "Source of Truth" for read-only data.
- **Exposure:** Use public getters or read-only properties to expose data to Systems.
- **Runtime Modifiers:** Never modify ScriptableObject values at runtime (as changes persist in the editor). Use temporary Runtime Instances or Wrapper classes.
```csharp
public class ItemInstance {
    public ItemData template;
    public float currentDurability; // Modified at runtime
}
```

## 4. Verification
- Use `OnValidate()` to ensure data consistency (e.g., check that Health > 0) in the Inspector.
```csharp
private void OnValidate() {
    if (baseHealth < 0) baseHealth = 0;
}
```
