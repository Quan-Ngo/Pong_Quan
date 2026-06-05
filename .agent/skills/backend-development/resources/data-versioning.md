---
skill: Data Versioning & Migration
description: Managing schema changes in persistent game data across versions.
---

# ⚙️ Data Versioning & Migration Skill

**Context:** Use this skill when you change your `SaveData` structure and need to avoid breaking existing players' save files.

## 1. Migration Mindset
- **The Golden Rule:** Never delete or rename fields in the serialization DTOs. Add new fields instead.
- **Header Check:** Check the `version` integer in the save file before parsing.

## 2. Implementation Strategies
- **Default Values:** Use `[DefaultValue]` or handle null/missing fields during the "hydration" phase to provide backwards compatibility.
- **Transformation Pipeline:** If the schema changes drastically, implement a `MigrationProcessor`.

```csharp
public class MigrationProcessor {
    public SaveData Migrate(SaveData oldData, int fromVersion, int toVersion) {
        if (fromVersion == 1 && toVersion == 2) {
            // Convert 'List<string> inventory' to 'List<IntID> inventory'
        }
        return oldData;
    }
}
```

## 3. Best Practices
- **Snapshot Testing:** Keep a suite of old save files (v1, v2, v3) and ensure the current game can still load them.
- **Fail Gracefully:** If a save is too old to migrate, provide a clear error or offer to reset rather than crashing.
