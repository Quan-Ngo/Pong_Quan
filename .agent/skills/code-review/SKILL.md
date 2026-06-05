---
name: code-review
description: Ensure code quality through structured reviews, performance checks, and Unity-specific best practices. Use before PRs, after implementing features, or when claiming task completion.
---

# 🔍 Code Review Skill

**Context:** Use this skill for all code quality assessment activities in Unity / C# game development projects.

## Core Principles

**YAGNI** · **KISS** · **DRY** — Always.

- **Technical rigor over social comfort** — No performative agreement ("Great point!")
- **Verify before implementing** — Ask before assuming (e.g., Prefab structure)
- **Evidence before claims** — Fresh verification output required
- **Be honest, be direct, be concise**

## Four Practices

| Practice | When | Reference |
|----------|------|-----------|
| Receiving Feedback | External PR comments, unclear suggestions | `resources/code-review-reception.md` |
| Requesting Review | After tasks, before merge, stuck on problem | `resources/requesting-code-review.md` |
| Edge Case Scouting | After implementation, before review | `resources/edge-case-scouting.md` |
| Verification Gates | Before any completion claim | `resources/verification-before-completion.md` |
| **Unity Review Checklist** | During code review | `resources/unity-review-checklist.md` |

## Unity Standards References

Before reviewing, familiarize yourself with team standards:

| Domain | Standard |
|--------|----------|
| Data | [data-guidelines.md](../../guidelines/data-guidelines.md) |
| UI/UX | [ui-design-guidelines.md](../../guidelines/ui-design-guidelines.md) |
| Design | [design-guidelines.md](../../guidelines/design-guidelines.md) |
| Testing | [testing-guidelines.md](../../guidelines/testing-guidelines.md) |
| Git | [git-workflow-guidelines.md](../../guidelines/git-workflow-guidelines.md) |

## Quick Decision Tree

```
SITUATION?
│
├─ Received feedback → STOP if unclear, verify if external, implement if human partner
├─ Completed work → Scout edge cases → Request code review
└─ About to claim status → RUN verification command FIRST (Tests/Compile)
```

## Unity-Specific Verification Checklist

| Check | Tool / Command | What to Verify |
|-------|---------|----------------|
| Compiles | Unity Editor / Build | No compilation errors in console |
| EditMode Tests | Unity Test Framework | All logic/unit tests pass |
| PlayMode Tests | Unity Test Framework | All mechanical/integration tests pass |
| Perf Hot-paths | Unity Profiler | No GC.Alloc in Update(); frame budget met |
| Addressables | Addressables Groups | New assets assigned to correct groups |
| Prefab Integrity | Prefab Mode | No broken references or missing scripts |

## Unity-Specific Review Focus

- **Hot Path Performance:** No `GetComponent`, `Transform` access, or string concatenation in `Update()`.
- **Memory Management:** Minimize `new` allocations in frequent loops. Use Object Pooling for spawning.
- **Event Lifecycle:** Ensure `OnEnable` subscriptions are unsubscribed in `OnDisable`/`OnDestroy`.
- **Serialized Fields:** Prefer `[SerializeField] private` over public fields for Inspector exposure.
- **ScriptableObjects:** Use for configuration and shared data to reduce scene-specific overrides.
- **Physics Efficiency:** Use `FixedUpdate` for physics; use `NonAlloc` versions of overlap/raycast.

## Bottom Line

1. Technical correctness over social performance
2. Scout edge cases before review
3. Evidence before claims (Profiler logs/Test results)

**Verify. Scout. Question. Then implement. Evidence. Then claim.**
