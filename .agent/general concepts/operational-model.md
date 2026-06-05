# ⚙️ Operational Model: The Sequential Swarm

**Context:** How does the "Agent Team" actually work together on a task?

## 1. The Core Concept
The team does **not** run as 6 independent, disconnected bots.
They work as a **Single Intelligent Entity** that "shifts gears" (Personas) based on the sub-task.

## 2. The Workflow (Sequential Handoffs)

### Step 1: Admission & Strategy (The Principal & System Architect)
*   **Actors:** `Principal Agent Architect` & `System Architect`
*   **Action:** Analyzes the User Request. Defines system boundaries and communication patterns.
*   **Decision:** "This requires a new ScriptableObject structure, then a UI Controller."

### Step 2: Data & Core Logic (The Data Architect)
*   **Actor:** `Data Architect`
*   **Input:** The Plan from Step 1.
*   **Work:** Generates `ScriptableObjects`, `DataModels`, and core processing logic.
*   **Output:** Updated Data Layer + "Core Logic Ready" status.

### Step 3: UI & Interaction (The UI Lead)
*   **Actor:** `UI Lead`
*   **Input:** The data structures and logic from Step 2.
*   **Work:** Builds `Unity UI` (Canvas), Controllers, and interactive elements.
*   **Output:** Completed Feature UI.

### Step 4: Verification & Performance (QA & Performance)
*   **Actors:** `QA Engineer` & `Performance Engineer`
*   **Action:** Scans code for regressions (UTF) and profiles performance (FPS/Memory).
*   **Output:** "Pass" or "Optimization Required".

## 3. Why this way?
*   **Shared Context:** Everyone sees the same file system.
*   **No Hallucinations:** The UI Lead reads the *actual* ScriptableObject definitions the Data Architect just created, guaranteeing the Inspector fields match.
*   **Simplicity:** No complex message buses. The **Unity Project (Assets/Scripts)** is the message bus.

## 4. How to Invoke
You (The User) speak to the **Team**.
*   **"Build this mechanic"** -> Activates the Swarm (Principal -> Data -> UI).
*   **"Fix this framerate drop"** -> Activates the **Performance Engineer** directly.
