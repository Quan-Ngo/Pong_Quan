# 🚀 Unity Agent Team: The Ultimate Prompt Guide

This guide provides **50 Best Practice Use Cases** (10 per Agent) to help you maximize the potential of your AI Team in Unity Game Development.

---

## 🏛️ 1. System Architect (The City Planner)
*Best for: High-Level Design, System Boundaries, Communication Patterns*

1.  **System Initiation:** "Act as the System Architect. Interview me to define the high-level architecture for a 'Turn-Based Combat System'. Then produce a System Design Document (SDD)."
2.  **Decoupling Strategy:** "We need to extend the 'Inventory System' to support 'Crafting'. Design a decoupled pattern using C# Events or ScriptableObject-based architecture to keep the systems independent."
3.  **State Machine Design:** "Plan the state machine for our 'Enemy AI'. Outline the states (Patrol, Chase, Attack, Flee) and the transition logic to ensure smooth behavior transitions."
4.  **Communication Patterns:** "Compare 'Observer Pattern' vs. 'Command Pattern' for handling character abilities. List the pros/cons for our specific requirements regarding network synchronization."
5.  **Modular Design:** "Review our Combat Manager. Identify how to split it into smaller, testable modules (Damage Calculator, Turn Queue, Buff Processor) using Separation of Concerns."
6.  **Technology Selection:** "Research the trade-offs between using Unity's 'Built-in Physics' vs. a custom 'Raycast-based' motor for a fast-paced platformer. Consider performance and feel."
7.  **Save/Load Architecture:** "Design the high-level flow for our Save System. How should we handle cross-scene persistence and versioning of save files?"
8.  **Project Organization:** "Propose a folder structure and Assembly Definition (.asmdef) strategy for a project that will grow to include 50+ unique enemy types and 200+ items."
9.  **Feasibility Study:** "Can we use 'Unity Addressables' to manage 10GB of textures for a mobile game? Check the memory limitations and loading performance impact."
10. **Refactoring Roadmap:** "Our 'Player Controller' is a 2000-line monolith. Propose a plan to break it down into modular components (Input, Movement, Animation, Combat) using a Component-based approach."

---

## ⚙️ 2. Data Architect (The Engine Builder)
*Best for: ScriptableObjects, Serialized Data, Save Systems, Data Models*

1.  **ScriptableObject Design:** "Draft the base class for an 'Item' ScriptableObject. Include fields for Name, Description, Icon (Sprite), and Weight. Add a virtual method `OnUse()`."
2.  **Save System Schema:** "Create a JSON-serializable class structure for 'PlayerProgress'. It must store health, position (Vector3), and a list of acquired Item IDs."
3.  **Complex Logic Implementation:** "Implement a 'Damage Formula' class in C#. It needs to take into account Attacker Stats, Defender Stats, Element Weaknesses, and Random Variance."
4.  **Data Validation:** "Scan our 'Ability' data assets. Find all instances where a 'Cooldown' is set to 0 and report them as potential gameplay bugs."
5.  **Addressables Integration:** "Write a wrapper script to load 'Enemy Prefabs' using Unity Addressables. Handle asynchronous loading and ensure assets are released properly."
6.  **CSV/JSON Importer:** "Write an Editor Script that imports 'Weapon Stats' from a CSV file and populates an array of ScriptableObjects in the project."
7.  **Serialization Refactor:** "Our current Save System uses BinaryFormatter (unsafe). Refactor it to use `JsonUtility` or `Newtonsoft.Json` for better security and readability."
8.  **Database Strategy:** "Design a 'Dialogue System' data structure that supports branching choices, speaker IDs, and localized text strings."
9.  **Object Pooling:** "Implement a 'Projectile Pool' system. It should reuse Prefabs to avoid GC spikes during heavy combat scenarios."
10. **Data Seeding:** "Generate 50 realistic 'Loot Tables' for different dungeon tiers. Include drop probabilities and quantity ranges for each item category."

---

## 🎨 3. UI Lead (The Pixel Architect)
*Best for: Unity UI (Canvas), HUD Design, Interaction Logic*

1.  **HUD Scaffolding:** "Create a 'Player HUD' using Unity UI (Canvas). It must include a Health Bar, Mana Bar, and an XP Gauge that animates smoothly using DOTween."
2.  **Inventory UI Logic:** "Implement a grid-based 'Inventory Screen' controller. It needs to handle slot hover effects, item icons, and drag-and-drop functionality."
3.  **Dynamic UI Lists:** "Build a 'Quest Log' UI that dynamically generates entries based on active quests. Use a Vertical Layout Group and Content Size Fitter for responsiveness."
4.  **UX Integration:** "Write a Custom UI Hook that updates the 'Ammo Count' display whenever the player fires a weapon. Ensure it only updates when the value actually changes."
5.  **Visual Verification:** "Compare this UI Mockup [upload image] with our current In-Game HUD. List 3 violations regarding font consistency and alignment."
6.  **UI Performance:** "The 'World Map' UI is causing frame drops. Analyze the 'Layout Rebuilds'. Suggest ways to optimize the hierarchy and reduce the number of active components."
7.  **Accessibility (A11y):** "Audit our 'Settings Menu'. Ensure it supports full Keyboard/Controller navigation (Navigation Events) and has high-contrast options for text."
8.  **Responsive Layouts:** "Refactor the 'Shop UI' to work on both 16:9 and 21:9 aspect ratios. Ensure the text remains legible and buttons stay within the 'Safe Area'."
9.  **Theming System:** "Implement a 'UI Theme' manager using ScriptableObjects. We should be able to swap button colors, fonts, and panel sprites across the entire game instantly."
10. **Dialogue UI:** "Build a typewriter-style Dialogue Box. It should handle multiple lines, 'Skip' input, and trigger 'Close' events when the conversation ends."

---

## 🧪 4. QA Engineer (The Safety Architect)
*Best for: UTF Tests, Debugging, Destructive Testing*

1.  **Test Strategy:** "Designing the QA Strategy for the 'Combat System'. Define the mix of EditMode unit tests and PlayMode integration tests for 100% confidence."
2.  **PlayMode UTF Test:** "Write a Unity Test Framework (UTF) PlayMode script that spawns a Player, an Enemy, and verifies that the 'Attack' command deals the correct damage."
3.  **Negative Testing:** "Write a test case that tries to 'Save' the game when the disk is full or during an active Boss Battle. Ensure the system handles it without crashing."
4.  **Stress Testing:** "Create a test script that spawns 500 'Projectile' objects in a single frame. We need to verify the physics engine doesn't collapse."
5.  **Logic Debugging:** "The 'Health Regen' logic fails when the player has a 'Poison' debuff. Analyze the script. Is it a timing issue between `Update` calls? Add a log trace."
6.  **Edge Case Testing:** "Write a script to test the 'Inventory Limit'. Fill the inventory to 100% and then try to pick up a 'Legendary' item. Ensure it drops to the ground."
7.  **Save Data Integrity:** "Write a script that generates 100 corrupted save files with random null values. Verify that the 'Save Manager' can detect and ignore them during loading."
8.  **Automated Smoke Test:** "Set up an automated scene-loader test that opens every scene in the project and reports any Console Errors or Missing Prefab references."
9.  **Visual Regression:** "Set up a comparison test for the 'Skill Effects'. It should flag if a change in the shader broke the transparency or color of the 'Fireball'."
10. **Network Simulation:** "Simulate 'High Latency' (200ms+) during a multiplayer duel. Verify that the 'Lag Compensation' logic keeps the character positions synced."

---

## 🛣️ 5. Performance Engineer (The Optimizer)
*Best for: Profiling, Optimization, Framerate Stability*

1.  **CPU Profiling:** "Analyze this CPU Profiler capture. We are seeing spikes during 'Scene Loading'. Identify if it's too many `Awake` calls or heavy Asset loading."
2.  **Memory Management:** "Review our 'Texture Usage' in the current scene. Identify any textures that are not compressed or have unnecessary Mipmaps to save VRAM."
3.  **Draw Call Reduction:** "Analyze the 'Frame Debugger'. We have 500+ draw calls. Suggest a batching strategy (Static vs. Dynamic) and identify materials that can be merged."
4.  **Physics Optimization:** "The 'Collision Matrix' seems bloated. Recommend a layer setup that ignores unnecessary collisions between VFX and environment triggers."
5.  **Script Optimization:** "Analyze this 'AI Controller'. It uses `GetComponent` in `Update()`. Refactor it to cache references and use a 'Manager' pattern for processing."
6.  **Shader Optimization:** "Check this 'Water Shader'. It's too heavy for mobile. Suggest ways to simplify the transparency and reflection logic to hit 60 FPS."
7.  **Garbage Collection:** "We are seeing GC spikes every 10 seconds. Scan our 'Combat Logic' for string concatenations or `new` allocations happening in tight loops."
8.  **LOD Implementation:** "Set up a LOD (Level of Detail) strategy for our 'Forest' biome. Define distances for swapping high-poly trees with billboards."
9.  **UI Canvas Optimization:** "Explain why our HUD is causing 'Layout Rebuilds' every frame. Suggest ways to split the Canvas to isolate frequently changing elements."
10. **Bottleneck Analysis:** "Identify the main bottleneck: CPU-bound (Logic), CPU-bound (Render Commands), or GPU-bound (Fill Rate). Provide a fix for the primary issue."
---

## 🤝 6. Team-Level Workflows (The Swarm)
*Best for: Complex mechanics requiring Data + UI + QA + Performance.*
**How to use:** Address the "Principal Architect" or the "Team" generically.

1.  **The "Modular Ability System" (Full Stack Mechanic):**
    > "Team, I need a data-driven ability system. It should allow designers to define new skills (e.g., Fireball, Heal, Teleport) via ScriptableObjects. We need a 'Skill Bar' UI and a robust 'Effect Processor'."
    *   *Result:* Principal Plans -> System Architect Designs Interface -> Data Architect Builds SO -> UI Lead Builds Skill Bar.

2.  **The "Multiplayer Sync" (Strategy + Execution):**
    > "We are moving from Local to Online Multiplayer. Analyze our existing 'Input Controller', then scaffold a new architecture that uses Client-Side Prediction."
    *   *Result:* System Architect analyzes Latency -> Data Architect builds Sync Models -> Performance Engineer optimizes Packets.

3.  **The "Performance Overhaul" (Review + Fix):**
    > "The 'Combat Scene' drops below 30 FPS. Conduct a full audit and fix the bottlenecks in both scripts and rendering."
    *   *Result:* QA measures FPS -> Performance Engineer optimizes Shaders/Batching -> Data Architect optimizes AI Logic.

4.  **The "New Game Mode" (End-to-End):**
    > "Build a 'Boss Rush' mode. It needs a UI for boss selection, a data layer for tracking progress, and a system to sequence the encounters."
    *   *Result:* Sequential execution across all roles.

5.  **The "Console Port Prep" (Audit):**
    > "Team, preparing for Console release. Optimize the UI for controllers, ensure memory usage is within hardware limits, and harden the Save System."
    *   *Result:* UI Lead adds Controller Support -> Performance Engineer optimizes VRAM -> QA tests on DevKit.

---

## 🏭 7. RPG Systems Special (The Mechanics Suite)
*Best for: Deep-dive scenarios in Character progression, Combat, and World building.*
**Context:** Use these prompts to build your "Ultimate RPG" Platform.

### ⚔️ Combat & Mechanics
6.  **"The Combo System Flow"**
    > "Team, build a 'Melee Combo' system. It needs to track 'Window Timing' for inputs. Upon successful timing, trigger a follow-up animation. If timing is missed, reset to 'Idle'."
7.  **"The Damage Pop-up Tracker"**
    > "We need a UI system that spawns 'Damage Numbers' over enemies. They should float upwards, fade out, and vary in color based on 'Critical' or 'Resisted' hits."
8.  **"The Status Effect Sync"**
    > "Design an event-driven pattern for Status Effects (Burn, Freeze, Stun). When a character is 'Burned', their HP should drain every second, and their sprite should tint red."
9.  **"The Projectile Trajectory"**
    > "Build an 'Arrow' system that uses parabolic curves for trajectory. It should rotate to face the direction of travel and trigger 'Impact' effects on collision."

### 📜 Character & Narrative
10. **"The Branching Dialogue Engine"**
    > "Team, we need a 'Dialogue' system that supports Player Choices. Choices should set 'Global Flags' (e.g., `helpedVillagers = true`) that change future interactions."
11. **"The Character Stat Sheet"**
    > "Build a 360-degree View for the Player. Fetch 'Base Stats' from ScriptableObjects and merge them with 'Modifier Stats' (from Equipment). Display as a comparison table."
12. **"The Inventory Sort Utility"**
    > "We need a 'Sort' button for the inventory. It should organize items by 'Category' (Weapon, Armor, Consumable) then by 'Rarity'."
13. **"The Skill Tree UI"**
    > "Build a node-based Skill Tree. Players spend 'Skill Points' to unlock nodes. Locked nodes must be greyed out, and paths must light up when unlocked."

### 🗺️ World & Exploration
14. **"The Fog of War"**
    > "Create a 'Fog of War' system for the Minimap. As the player explores, the black texture should be carved away to reveal the terrain below."
15. **"The Day/Night Cycle"**
    > "Implement a 'Global Light' controller that rotates a Directional Light to simulate a 24-hour cycle. Change the 'Ambient Sky' color at Dawn and Dusk."
16. **"The Fast Travel System"**
    > "Build a 'Waystone' system. Players interact with a stone to unlock it. The 'Map UI' then allows clicking unlocked stones to teleport the character instantly."
17. **"The Interactive Object System"**
    > "Build a generic 'Interactable' interface. Whether it's a Chest, a Lever, or a Door, they should all use the same 'Press E to Interact' prompt."

### 🎒 Items & Equipment
18. **"The Equipment Slot Logic"**
    > "Characters have slots for 'Head', 'Chest', 'Hands', and 'Feet'. When equipping an item, automatically unequip the previous item and update the player's 3D model."
19. **"The Crafting Recipe Validator"**
    > "Allow players to combine items. The system must check if the 'Inventory' contains all ingredients in a 'Recipe'. If yes, consume ingredients and spawn the result."
20. **"The Loot Drop Table"**
    > "When an enemy dies, calculate loot based on a weighted table. 'Common' items have 70% weight, 'Legendary' have 1%. Drop the items as physical objects in the world."
21. **"The Durability System"**
    > "Weapons lose 'Durability' with every hit. At 0 durability, the weapon's damage is halved and the icon shows a 'Broken' red overlay."

### 🧾 Economy & Shops
22. **"The Merchant Buy/Sell Logic"**
    > "Build a 'Shop' interface. Buying an item subtracts 'Gold' and adds to 'Inventory'. Selling does the opposite. Price should be 50% for selling back."
23. **"The Dynamic Economy"**
    > "Implement a 'Supply and Demand' logic. If the player sells 100 'Potions' to the same merchant, the buy price should gradually decrease."
24. **"The Currency Exchange"**
    > "Players have 'Copper', 'Silver', and 'Gold'. Implement a system that auto-converts 100 Copper -> 1 Silver, and 100 Silver -> 1 Gold."
25. **"The Auction House Search"**
    > "Build a searchable UI for a global marketplace. Players can filter items by 'Price Range', 'Level Requirement', and 'Attribute Bonus'."

### 🛡️ Progression & Mastery
26. **"The Level-Up Flow"**
    > "When XP reaches the threshold, trigger a 'Level Up' effect. Increase 'Base Stats' based on the character's 'Class' and restore Health/Mana to full."
27. **"The Achievement System"**
    > "Build a background tracker for achievements (e.g., 'Kill 100 Goblins'). When a milestone is hit, show a UI toast and grant a 'Title' to the player."
28. **"The Mastery Bonus"**
    > "As players use 'Fire Spells', their 'Fire Mastery' level increases. Each level grants a +2% damage bonus to all Fire-based abilities."
29. **"The Quest Progression"**
    > "Track quest objectives (e.g., 'Slay the Dragon'). When the objective is met, update the 'Quest Log' and point the 'Navigation Arrow' to the Turn-In NPC."
30. **"The Difficulty Scaler"**
    > "Build a utility that scales Enemy HP and Damage based on the player's current 'Level'. We need a 'Heroic' mode that adds a 1.5x multiplier."

