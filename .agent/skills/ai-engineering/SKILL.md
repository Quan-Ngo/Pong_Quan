---
name: ai-engineering
description: Expertise in Generative AI, LLM Orchestration, and Unity AI Integration.
---

# 🤖 AI Engineering

**Definition:** The discipline of integrating "Stochastic" AI components into "Deterministic" game engines. It is not just prompting; it is about Reliability, Performance, and Runtime Architecture.

## 🔑 Key Concepts

1.  **RAG (Retrieval-Augmented Generation):**
    *   Do not trust the model's internal training data for game engine documentation facts.
    *   **Architecture:** Query -> Embed -> Vector Search -> Context Injection -> LLM.
    *   **Unity Context:** Use local embeddings (Sentis) or cloud-based search for runtime knowledge.

2.  **Orchestration (LangChain/Unity Systems):**
    *   LLMs are just API calls. In games, they must be tied to **Behavior Trees** or code triggers.
    *   Use **Tools** (Function Calling) to let the AI interact with the engine (e.g., "Create Prefab", "Delete Script").

3.  **Unity AI Integration:**
    *   Use **Unity Sentis** for local inference (ONNX) or Cloud Models for dynamic content.
    *   **Constraint:** NEVER block the main game thread. ALWAYS use asynchronous patterns to maintain FPS.

4.  **Multi-Agent Systems:**
    *   Decompose complex behaviors into "Crews" (Director, Researcher, Programmer).
    *   **Pattern:** Hierarchical Delegation vs. Event-Driven Pipelines.

## 🛠️ Best Practices

*  **System Prompts:** Treat them as code. Version them. Test them in-engine.
*  **Structured Output:** Force the LLM to return JSON/YAML. Never parse natural language for game state updates.
*  **Evaluation (Evals):** Use automated playtesting with "LLM-as-a-Judge" to score responses for accuracy and relevance.

## 📚 References
*  [Unity AI Integration Strategy](resources/unity-ai-core.md)
*  [Procedural Narrative (LLM-based)](resources/document-extraction.md)
*  [Agentic Game Workflows](resources/agentic-workflows.md)
*  [Context Engineering for Games](resources/context-engineering.md)
*  [AI Safety & NPC Guardrails](resources/ai-safety-and-ops.md)
*  [Multi-Agent Systems in Games](resources/multi-agent-systems.md)
