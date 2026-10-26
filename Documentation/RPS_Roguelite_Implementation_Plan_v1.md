# Rock Paper Scissors Roguelite - Implementation Plan (v1)

Source design: [RPS_Roguelite_Production_Design_v0.md](D:/Unity/repos/Rock_Paper_Scissors/Documentation/RPS_Roguelite_Production_Design_v0.md)

This plan translates the design into implementation phases, explicitly mapping:
- existing classes to modify
- new classes/assets to create
- new interactions between systems

---

## 1) Locked design constraints reflected in this plan

- RPS becomes class framework (Rock / Paper / Scissors), each with specialized units.
- Keep **Marbles** as run currency.
- Add **Gold Stars** as meta-progression currency.
- Mid-encounter deployment exists, but is unlocked via meta progression.
- Buildings consume **Build Tokens + Marbles** (not unit AP).
- Sharpness resets each encounter.
- Include objective: **Defeat All Enemies**.
- Maintain and extend existing radial/rotary unit interaction menus.

---

## 2) Phased delivery roadmap

## Phase A - Core architecture and data model
Goal: add run/meta/deployment/objective data scaffolding with minimal gameplay disruption.

## Phase B - Tactical systems expansion
Goal: facing, class secondary mechanics, and multi-target attack framework.

## Phase C - Encounter and map systems
Goal: hex-shaped arenas, objective manager, biome fort variants, obstacles/trampoline rules.

## Phase D - Meta progression and unlock flow
Goal: unlock pipeline for abilities and permanent units; enable mid-encounter deployment unlock.

## Phase E - UI/UX integration and balancing
Goal: integrate with existing rotary menus/HUD and tune numbers for first vertical slice.

---

## 3) Existing classes to touch (by subsystem)

## 3.1 Session flow, game state, and encounters
- [ApplicationManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/ApplicationManager.cs)  
  Add run entry/exit flow, node-to-encounter transitions.
- [GameplayManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/GameplayManager.cs)  
  Add objective completion routing, encounter result payloads, run state hooks.
- [WaveManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/WaveManager.cs)  
  Integrate objective-aware spawning, encounter phase transitions, deploy limits.
- [GameMode.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/GameMode.cs)  
  Extend mode strategy or add run subtype flags.

## 3.2 Grid, map, and board shape
- [GridManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Grid/GridManager.cs)  
  Support large-hex arena bounds, deploy anchor queries, objective zone marking.
- [GridObject.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Grid/GridObject.cs)  
  Add objective/deploy/facing visualization metadata hooks.
- [LevelData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/LevelData.cs)  
  Expand for biome/objective/deploy rule references (or link new EncounterDefinition assets).
- [Wave.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Wave.cs)  
  Keep as spawn payload, optionally nested inside encounter definitions.

## 3.3 Units, combat, and deployment
- [Unit.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/Unit.cs)  
  Add class-role helpers and status/state adapters (Sharpness, Momentum, Loose Leaf).
- [UnitClass.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitClass.cs)  
  Keep current enum or add class-role mapping enum to avoid overloading bit flags.
- [UnitData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitData.cs)  
  Add ability profile links and per-class mechanic tuning values.
- [UnitAttack.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitAttack.cs)  
  Integrate facing bonuses and multi-target attack profiles (AoE/pierce).
- [UnitMovement.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitMovement.cs)  
  Integrate movement bonus from Loose Leaf.
- [UnitProgression.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitProgression.cs)  
  Keep leveling, add stateless hooks for encounter-start reset of mechanic stacks.
- [CombatModifiers.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/CombatModifiers.cs)  
  Add facing and class-secondary modifiers.
- [UnitSpawner.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitSpawner.cs)  
  Integrate Build Token checks and encounter deploy caps.
- [UnitSpawnerData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitSpawnerData.cs)  
  Add deploy-related tuning fields.
- [UnitManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitManager.cs)  
  Add encounter action/deploy summaries and objective kill tracking support.

## 3.4 Input, selection, and AI
- [ActionHandler.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/ActionHandler.cs)  
  Add deploy mode targeting and multi-target preview flow.
- [EnemyAI.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Enemy/EnemyAI.cs)  
  Add objective-aware behavior weighting and deploy/build usage rules.

## 3.5 Currency and save/load
- [CurrencyBank.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Currency/CurrencyBank.cs)  
  Keep Marbles behavior and add optional reason/context events for spend analytics.
- [SaveManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveManager.cs)  
  Serialize run/meta/deploy/objective state.
- [SaveData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveData.cs)  
  Extend with run/meta/objective/deploy blocks.
- [SaveGameplayManagerData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveGameplayManagerData.cs)
- [SaveWaveManagerData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveWaveManagerData.cs)
- [SaveUnitData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveUnitData.cs)

## 3.6 UI integration (rotary-first)
- [BuildingMenu.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Menus/BuildingMenu.cs)  
  Extend radial menu actions for abilities/deploy options.
- [BuildingButton.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Buttons/BuildingButton.cs)
- [BuildUnitButton.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Buttons/BuildUnitButton.cs)
- [UpgradeUnitSpawnerButton.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Buttons/UpgradeUnitSpawnerButton.cs)
- [GameMenu.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Menus/GameMenu.cs)  
  Show objective panel + encounter results.
- [GameOverMenu.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Menus/GameOverMenu.cs)  
  Add run summary + Gold Stars rewards.
- [CurrencyUI.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Currency/CurrencyUI.cs)  
  Keep Marbles display; pair with new Gold Stars UI.
- [StatusUpdateUI.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/StatusUpdateUI.cs)  
  Add deploy/objective state messages.
- [SelectionUI.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/SelectionUI.cs)  
  Show facing/mechanic stacks per selected unit.

---

## 4) New classes/assets to add

## 4.1 Run/meta progression
- `RunManager` (runtime run state machine)
- `RunNodeMap` (runtime current path/visited nodes)
- `RunNodeDefinition` (ScriptableObject node template)
- `BiomeDefinition` (ScriptableObject biome rules, objective pools, fort variant)
- `MetaProgressionManager` (Gold Stars spend/unlock orchestration)
- `MetaProfileData` (serializable persistent meta unlock state)
- `UnlockNodeDefinition` (ScriptableObject unlock node)
- `UnlockType` enum (Ability, Unit, DeployUnlock, LoadoutSlot, etc.)

## 4.2 Encounter and objectives
- `EncounterDefinition` (ScriptableObject replacing/augmenting raw LevelData + Wave)
- `ObjectiveManager` (tracks active objective and completion/failure)
- `ObjectiveType` enum
- `IObjectiveRule` + concrete rules:
  - `DefendFortObjectiveRule`
  - `DefeatAllEnemiesObjectiveRule`
  - `EliminateCaptainObjectiveRule`
  - `HoldZoneObjectiveRule`
- `ObjectiveZoneMarker` (scene/grid marker component)

## 4.3 Deployment and resources
- `BuildTokenManager` (BT budget per turn and per encounter)
- `DeployRuleSet` (ScriptableObject for caps/cooldowns/adjacency rules)
- `DeployAnchor` (component tagging valid deployment origin entities)
- `EncounterDeploymentState` (serializable state block)

## 4.4 Combat expansion
- `FacingComponent` (unit facing and rotation API)
- `FacingCombatModifier` (utility for front/side/back multipliers)
- `AbilityProfile` (ScriptableObject ability metadata and targeting)
- `TargetPatternProfile` (line/pierce/radius shape definition)
- `StatusEffectController` (host for DoT and temporary mechanics)
- `PaperCutStatusEffect`
- `RockMomentumState`
- `PaperLooseLeafState`
- `ScissorsSharpnessState`

## 4.5 UI
- `DeployHUDUI` (Build Tokens, deployment caps, blocked reasons)
- `GoldStarsUI` (meta reward preview and bank)
- `ObjectiveTrackerUI` (active objective + progress)
- `TargetPreviewUI` (AoE/piercing tile highlights)
- `RotaryActionButton` (generic radial action entry for abilities/deploy)

---

## 5) New interactions between classes

## 5.1 Deployment flow
1. `BuildingMenu`/`RotaryActionButton` requests deploy action.
2. `ActionHandler` enters deploy targeting mode.
3. `BuildTokenManager` validates BT and encounter caps.
4. `UnitSpawner`/deployment service validates tile and anchor (`DeployAnchor` + `GridManager`).
5. `CurrencyBank` spends Marbles.
6. `Unit` is instantiated, `UnitManager` updates collections, `GridManager` updates occupancy.
7. `DeployHUDUI` + `StatusUpdateUI` refresh.

## 5.2 Objective flow
1. `RunManager` starts encounter using `EncounterDefinition`.
2. `ObjectiveManager` creates active `IObjectiveRule`.
3. `ObjectiveTrackerUI` subscribes for progress updates.
4. Objective rule listens to existing events (`UnitHealth.OnDeath`, turn events, zone occupancy checks).
5. On complete/fail, `GameplayManager` handles encounter result and reward routing.

## 5.3 Class mechanics flow
1. Combat/move actions trigger mechanic state updates (`RockMomentumState`, `PaperLooseLeafState`, `ScissorsSharpnessState`).
2. `CombatModifiers` queries mechanic state + facing state when computing damage.
3. `StatusEffectController` applies/removes effects like Paper-Cut DoT.
4. `SelectionUI` displays state stacks and bonuses for readability.
5. Encounter start calls reset hooks (Sharpness reset).

## 5.4 Meta unlock flow
1. Encounter end grants Gold Stars through `RunManager` -> `MetaProgressionManager`.
2. Player spends Gold Stars in unlock menu (`UnlockNodeDefinition` graph).
3. Unlocks update `MetaProfileData`.
4. Next run bootstraps pool (units/abilities/deployment availability) from meta profile.

---

## 6) Save/load extension plan

Add new save blocks:
- `SaveRunData`
- `SaveMetaProgressionData`
- `SaveObjectiveData`
- `SaveDeploymentData`

Wire in:
- [SaveData.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveData.cs)
- [SaveManager.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/SaveSystem/SaveManager.cs)

Compatibility strategy:
- Default missing blocks to safe values (legacy saves remain loadable).
- Version stamp save root for future migrations.

---

## 7) Delivery breakdown (implementation order)

## Milestone 1 - Skeleton systems
- Add run/meta/deploy/objective core classes.
- Add save schemas and plumbing.
- Keep current gameplay intact under feature flags.

## Milestone 2 - Deployment + objectives in battle
- Build Tokens in encounter HUD.
- Deploy caps and anchor validation.
- ObjectiveManager with Defend Fort + Defeat All Enemies first.

## Milestone 3 - Combat expansion
- Facing logic and attack preview updates.
- Multi-target attack framework (Rockslide AoE, Scissors pierce line).
- Paper-Cut DoT as Paper move family.

## Milestone 4 - Content and progression
- Initial class roster (including Card Mechanic, Trimmer Trooper).
- Gold Stars unlock loop + unlock UI.
- Mid-encounter deployment unlock in meta tree.

## Milestone 5 - Biomes and polish
- Bedroom biome vertical slice.
- Biome fort variants and objective tuning.
- Balance and readability passes on radial UX and previews.

---

## 8) Risk register and mitigation

- **Risk:** rules overlap (AP vs BT vs Marbles confusion).  
  **Mitigation:** explicit HUD separation and blocked-reason messaging.

- **Risk:** deployment spam slows turns.  
  **Mitigation:** hard per-encounter deployment cap + Build Token limit + anchor constraints.

- **Risk:** multi-target attacks reduce clarity.  
  **Mitigation:** mandatory tile telegraph and preview through existing radial flow.

- **Risk:** save complexity/regressions.  
  **Mitigation:** versioned save data + migration defaults + targeted load tests.

---

## 9) Immediate next implementation tasks

1. Create base data assets: `EncounterDefinition`, `BiomeDefinition`, `DeployRuleSet`, `UnlockNodeDefinition`.
2. Implement `BuildTokenManager` and integrate with [UnitSpawner.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/Unit/UnitSpawner.cs).
3. Implement `ObjectiveManager` with `DefendFortObjectiveRule` and `DefeatAllEnemiesObjectiveRule`.
4. Extend save model with `SaveRunData` and `SaveMetaProgressionData`.
5. Add `DeployHUDUI` + `ObjectiveTrackerUI` and wire to [GameMenu.cs](D:/Unity/repos/Rock_Paper_Scissors/Rock_Paper_Scissors/Assets/Scripts/UI/Menus/GameMenu.cs).
