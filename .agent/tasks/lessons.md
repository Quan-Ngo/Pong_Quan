# Lessons Learned

## Tooling & Pathing
- **Artifacts vs Regular Files:** When writing to files in the workspace (like `tasks/todo.md`), set `IsArtifact: false`. The `IsArtifact: true` flag is reserved for the system-defined brain directory and will error on workspace paths.
- **Absolute Paths:** Always use absolute paths for `TargetFile` to avoid "path is not absolute" errors.

## Content Scaling
- **Minimal Pivot Strategy:** To align a skill doc with a different platform (e.g., SAP -> Unity) while remaining minimal, focus on:
    1. Swapping vendor-specific tech (HANA -> Sentis, BTP -> Asynchrony).
    2. Adjusting the "context" domain (Business -> Game Engine Facts).
    3. Updating references to match the new ecosystem.
- **Handling Special Characters:** When editing Markdown files containing emojis (✅, ❌) or tree structure characters (├──, └──), use `view_file` to copy the exact character sequences. Standard `cat` output or manual typing may cause encoding mismatches that lead to "target content not found" errors during replacement.
