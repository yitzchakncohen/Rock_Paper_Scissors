# Copilot Instructions for Rock_Paper_Scissors

## Build, test, and lint commands

Project root for commands: `Rock_Paper_Scissors/`

- Script compile check (fastest local validation):
  - `dotnet build .\Rock_Paper_Scissors.slnx`
- Unity EditMode tests (all):
  - `"<UNITY_EDITOR_PATH>" -batchmode -quit -projectPath "D:\Unity\repos\Rock_Paper_Scissors\Rock_Paper_Scissors" -runTests -testPlatform EditMode -testResults "Logs\EditModeTests.xml"`
- Unity PlayMode tests (all):
  - `"<UNITY_EDITOR_PATH>" -batchmode -quit -projectPath "D:\Unity\repos\Rock_Paper_Scissors\Rock_Paper_Scissors" -runTests -testPlatform PlayMode -testResults "Logs\PlayModeTests.xml"`
- Single Unity test:
  - `"<UNITY_EDITOR_PATH>" -batchmode -quit -projectPath "D:\Unity\repos\Rock_Paper_Scissors\Rock_Paper_Scissors" -runTests -testPlatform EditMode -testFilter "Fully.Qualified.TestName" -testResults "Logs\SingleTest.xml"`

Notes:
- Unity version in this repo is `6000.3.11f1` (see `ProjectSettings/ProjectVersion.txt`).
- There is no dedicated lint command or checked-in lint config for this project at the moment.

## High-level architecture

- **Scene bootstrap and app flow**
  - `ApplicationManager` is a `DontDestroyOnLoad` singleton that orchestrates menu/game transitions, continues, rewarded ads, and Android review prompts.
  - The project is structured around a two-scene flow: `MenuScene` and `MainScene`, and scene loading is coordinated via `SceneTransitionUI` and the application manager rather than direct scene references.

- **Turn-based tactical loop**
  - `TurnManager` owns the current turn and emits `OnNextTurn` with `IsPlayersTurn`/`Turn` data.
  - `ActionHandler` manages player interactions such as selection, movement, attack attempts, and lock states while waves or game-over logic are active.
  - `EnemyAI` runs when the turn flips to the enemy side; the actual game state is mostly event-driven.
  - `GameplayManager` owns persistent progression data (mode, level, score) and fires `OnGameOver` / `OnLevelCompleted`.

- **Grid and wave flow**
  - `GridManager` creates the runtime board from `LevelData` dimensions and spawn points and tracks occupancy/distance helpers.
  - `WaveManager` spawns unit/building prefabs from `Wave` assets, manages endless-wave timing, and decides when a level starts or continues.
  - Level flow is driven by `ApplicationManager.StartGameRoutine()` and `WaveManager.StartWaveWhenReady(...)` rather than direct start logic embedded in scene objects.

- **Units and state**
  - Units are created from prefab/scriptable definitions and compose behaviors through action components such as movement and attacks.
  - `UnitManager` tracks friendly/enemy unit sets, action-point resets, and remaining actions; save/load code expects unit data to be restored from serialized `SaveUnitData` entries.

- **Save/load subsystem**
  - Save contracts use `ISaveInterface<T>` across core managers, and `SaveManager` writes JSON to `Application.persistentDataPath + "/Saves/save.txt"`.
  - `SaveManager` also rebinds scene-local dependencies after each scene load, validates unit dictionaries against friendly/enemy unit classes, and restores persisted state via `LoadGameAsync(...)`.

- **UI and feedback**
  - The UI is event-driven through menu/button scripts (`MainMenu`, `GameOverMenu`, `SaveButton`, `AdModal`) and uses static events to drive application flow.
  - `AudioManager` is a persistent singleton used for one-shot feedback and menu/gameplay sound cues.

## Key conventions in this codebase

- **Event-driven wiring is the primary integration pattern**
  - Systems communicate through static/global events (`OnNextTurn`, `OnUnitSpawn`, `OnAnyActionCompleted`, etc.) rather than direct references.
  - When adding a new subscription, mirror the existing lifecycle pattern and unsubscribe in `OnDestroy` / `OnDisable` to avoid duplicate listeners across scene loads.

- **Runtime scene references are resolved dynamically**
  - Many managers reacquire scene-local dependencies with `FindObjectOfType` after scene loads.
  - Avoid caching scene object references across scene transitions unless they are explicitly `DontDestroyOnLoad` objects.

- **Data is authored through ScriptableObjects and runtime asset data**
  - Level balance, wave setup, and enemy/friendly unit definitions are expected to come from `LevelData`, `Wave`, and `UnitData` assets rather than hardcoded values.

- **Save compatibility depends on extending both data and serializers**
  - New persistent gameplay state should be added to the matching `Save*Data` struct/class and wired through both `Save()` and `Load()` paths in the owning manager.
  - Save logic is not limited to one component; the repo expects the same data model to be updated across managers and `SaveManager` serialization.

- **Namespace usage is mixed**
  - Most gameplay scripts are under `RockPaperScissors.*`, while some root-level gameplay classes remain in the global namespace.
  - Matches the surrounding file’s namespace style when editing to avoid ambiguous type resolution.

- **Platform-specific behavior is guarded explicitly**
  - Android-specific flows (ads and review prompts) are wrapped in `#if UNITY_ANDROID` checks; preserve that pattern for any platform-dependent SDK usage.

- **Unity MCP configuration is project-local**
  - Workspace-level MCP config exists at `Rock_Paper_Scissors/.vscode/mcp.json` and should remain aligned with the local Unity CLI installation when changing MCP setup.

- **Editor-task routing is skill-driven**
  - For Unity Editor operations (scene, prefab, GameObject, asset, hierarchy, inspector, Play Mode verification), use the project skill at `Rock_Paper_Scissors/.github/skills/unity-editor-task-routing/SKILL.md`.
  - Use Unity CLI Editor-driving commands for those tasks, and reserve direct code edits for coding tasks.

- **Coding standards skills are available**
  - For Unity runtime/gameplay C# patterns, use `Rock_Paper_Scissors/.github/skills/csharp-unity-standards/SKILL.md`.
  - For general non-Unity-specific C# conventions, use `Rock_Paper_Scissors/.github/skills/csharp-coding-standards/SKILL.md`.
