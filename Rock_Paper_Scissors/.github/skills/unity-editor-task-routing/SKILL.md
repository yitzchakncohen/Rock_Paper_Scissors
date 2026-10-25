---
name: unity-editor-task-routing
description: "Use when a request involves Unity Editor tasks (scene, prefab, GameObject, asset, hierarchy, inspector, Play Mode verification, in-editor setup). Route those tasks through Unity CLI Editor-driving commands first; use normal code editing only for coding tasks."
allowed-tools:
  - Bash
---

# Unity Editor task routing (project policy)

For this repository, split work into two buckets:

1. **Editor tasks** (scene/prefab/asset/hierarchy/Play Mode/in-Editor setup)
2. **Coding tasks** (C# scripts, tests, refactors, bug fixes)

## Rule

- **If it is an Editor task, use Unity CLI against a running Editor first.**
- **If it is a coding task, edit code normally.**

## Editor-task checklist

1. Confirm CLI and target Editor:
   - `unity --version`
   - `unity status --project-path "<PROJECT_PATH>"`
2. If needed, open the project:
   - `unity open "<PROJECT_PATH>"`
3. Discover available Editor commands:
   - `unity command --project-path "<PROJECT_PATH>"`
4. Execute Editor changes via CLI commands (or eval command if exposed).
5. Verify in Editor state/output (status, command result, logs) before concluding.

## Guardrails

- Do **not** hand-edit `.unity`, `.prefab`, or `.asset` files when a reachable Editor can be driven through Unity CLI.
- When multiple Editors might be open, always pass `--project-path`.
- If CLI cannot connect, report the failure cause (for example Safe Mode or no reachable instance) and ask before falling back to file-level edits.

## Project path for this repo

`D:\Unity\repos\Rock_Paper_Scissors\Rock_Paper_Scissors`
