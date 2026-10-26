# Rock Paper Scissors: Roguelite Tactics - Production Design Doc (v0.1)

## 1) Product Direction

### High concept
Turn-based **roguelite tactics** game with a playful schoolyard vibe, where players defend and expand a pillow-fort squad across short tactical encounters on a hex board.

### Design pillars
1. **Readable Tactics**: Every turn should present clear, high-value choices.
2. **RPS Identity First**: Rock, Paper, Scissors is always legible and always useful.
3. **Playground Personality**: Puns, toy-like props, and bedroom/schoolyard fantasy.
4. **Run Variety, Low Confusion**: High replayability without overloading the player.
5. **Snappy Session Length**: Mobile-friendly runs with Steam-depth build expression.

---

## 2) Core Structure

## 2.1 Run structure (macro loop)
- Start run with a starter squad + starter fort.
- Progress through a node map of short encounters:
  - Standard Fight
  - Elite Fight
  - Event
  - Shop
  - Rest/Upgrade
  - Boss
- Complete biome -> choose next biome branch.
- Run ends on fort destruction or final boss clear.

## 2.2 Encounter structure (micro loop)
- Hex-grid tactical battle on a **large hex-shaped arena**.
- Player turn alternates with enemy turn.
- Win by completing objective (defend, survive, capture, etc.).

---

## 3) Map and Biomes

## 3.1 Board geometry
- Keep existing hex tiles.
- Arena boundary should also be a **large hexagon** (not rectangular footprint).
- Keep map readability high with visual edge treatment (chalk border, carpet edge, toy wall).

## 3.2 Biome plan (simple at first)
Limit initial objective set for clarity:
1. **Bedroom Defense** (flagship): Defend pillow fort for N turns.
2. **Playground Skirmish**: Eliminate key enemy squad.
3. **Classroom Control**: Hold 1-2 zones for consecutive turns.

Each biome gets 1 mechanical identity + 1 visual identity:
- Bedroom: furniture obstacles, blanket cover lanes.
- Playground: trampoline chains, open flanks.
- Classroom: desk chokepoints, paper-themed hazards.

---

## 4) Class System: Rock / Paper / Scissors as Unit Classes

Rock, Paper, Scissors are **classes**, not single unit types.  
Each class has multiple specialized units (pun-forward naming/theme).

## 4.1 Class identity
- **Rock (Tank/Frontline)**
  - Role: durability, space control, initiation.
  - Advantage: bonus vs Scissors.
  - Secondary mechanic: **Momentum ("Rock and Roll")**.
- **Paper (Control/Debuff)**
  - Role: weaken enemies, reposition tempo, tactical control.
  - Advantage: bonus vs Rock.
  - Secondary mechanic: **Loose Leaf** (move farther when damaged as paper tears away).
- **Scissors (Offense/Buffs)**
  - Role: burst and target removal.
  - Advantage: bonus vs Paper.
  - Secondary mechanic: **Sharpness** (attack scaling through chained attacks).

## 4.2 Initial unit examples
### Rock
- **Pet Rock**: low-cost blocker.
- **Rock Golem**: slow, high armor anchor.
- **Rock Star**: support tank that grants nearby morale/tempo buff.

### Paper
- **Paper Crane**: high mobility support/debuff.
- **Notebook Binder**: root/silence-style control.
- **Card Mechanic**: structure-focused support that deploys/maintains Card Tower positions.
- **Card Tower**: temporary cover/line-of-sight blocker with fragile durability.

### Scissors
- **Safety Scissors**: starter striker with safer engage.
- **Trimmer Trooper**: precision finisher.
- **Craft Scissors**: applies stacking paper-cut DoT.

---

## 5) Combat Extensions (while preserving RPS readability)

## 5.0 Input/UI continuity rule
- Reuse the **existing unit rotary menus** as the primary combat interaction surface.
- Keep action selection (Move / Attack / Ability / Deploy where applicable) inside the rotary flow to preserve current player muscle memory.
- Any new ability types must be readable through the same radial UX pattern (clear iconography, AP/Marble cost, and target preview).

## 5.1 Facing system (new)
Facing matters for both offense and defense:
- **Front attack**: baseline.
- **Side attack**: +X% hit chance / +Y damage.
- **Back attack**: +higher bonus and/or ignores part of armor.
- Tanks (Rock) should have tools to rotate and protect flanks.

Implementation note:
- Keep telegraphing simple (visible facing arrow + highlighted flank arcs).
- Avoid hidden calculations; show bonuses in attack preview UI.

## 5.2 Class secondary passives
### Rock - Momentum
- Gains 1 Momentum when moving at least 2 tiles then attacking.
- At 3 stacks, next attack applies knockback or stun-lite.
- Lose 1 stack if ending turn without moving.

### Paper - Loose Leaf
- For each % HP missing, gain movement bonus in discrete thresholds.
- Example: below 70% HP +1 move, below 40% HP +2 move.
- Encourages tactical risk and reposition gameplay.
- Paper class also owns **Paper-Cut** damage-over-time effects as a move family.

### Scissors - Sharpness
- Gain Sharpness stack on successful attacks (cap N).
- Each stack grants +attack or +crit chance.
- Lose all/most stacks if skipping attack for a turn.

## 5.3 Multi-target attack patterns (new)
Introduce controlled multi-target attacks to deepen tactics and class identity:

- **Area attacks (example: Rockslide)**  
  - Rock-focused ability that hits a target tile and adjacent tiles.
  - Best use: zone denial, punishing clustered enemies, objective defense.
- **Piercing attacks (example: Scissor Line Cut)**  
  - Scissors-focused attack that travels in a line and can hit multiple units.
  - Best use: lane control and rewarding smart positioning/facing setup.

Design guardrails:
- Telegraph affected tiles before confirm.
- Keep damage tuning below single-target finishers unless setup conditions are met.
- Respect readability: avoid VFX clutter that obscures hex targeting.

---

## 6) Obstacles and Trampolines

## 6.1 Obstacles
- Purpose: lane variety, cover, pathing puzzles.
- Types:
  - Hard block (cannot pass)
  - Soft block (can pass with cost)
  - Interactable block (destroyable)

## 6.2 Trampolines (high-priority fun feature)
- Launch unit to linked tile(s).
- Can be chained for long movement lines.
- Risks:
  - Predictability: make landing previews explicit.
  - Abuse: add cooldown or once-per-turn-per-unit use.

---

## 7) Progression Design

## 7.1 Currency model
- Keep **Marbles** as primary run currency.
- Recommended second meta currency: **Gold Stars**.

### Marbles (run currency)
- Earned from encounter rewards, objectives, class synergy bonuses.
- Spent in-run on:
  - recruiting units
  - building/trap deployment
  - shop upgrades

### Gold Stars (meta currency)
- Earned from milestones, elites, run completion.
- Spent between runs on unlock trees and starting loadout options.

## 7.2 In-run progression
After each encounter, choose 1 of 3 rewards:
- Unit recruit / class-specific unit card
- Building/trap blueprint
- Team perk (global modifier)

Keep reward choices to 3 max for speed/readability.

## 7.3 Meta progression
Account-level progression tree with three tracks:
- Rock track (durability/control)
- Paper track (economy/control mobility)
- Scissors track (tempo/damage scaling)

Primary meta unlock categories:
- **Ability unlocks**: new active abilities and class-specific variants (for example alternate Rock AoE, Paper control tools, Scissors line attacks).
- **Permanent unit unlocks**: adds new units to the run draft/recruit pool.
- **Loadout slots**: expands pre-run options (starter unit choice, one starter perk, etc.).

Balance rule:
- Keep meta progression focused on **breadth (new options)** over **raw permanent power**.
- Permanent stat bonuses should be limited and small to avoid invalidating tactics difficulty.

---

## 8) Single-Round Progression and Deployment Rules

This is the key design decision from current discussion.

## Recommended approach
Use a **hybrid deployment model**:

1. **Between encounters (primary)**  
   Main roster growth and building selection happen here.

2. **During encounters (limited tactical deployment)**  
   Allow controlled in-battle placement/spawn to keep existing fun from current systems.

### Why this model fits
- Preserves your current unit/build placement DNA.
- Supports roguelite run strategy (macro choices matter).
- Prevents board-state clutter and decision overload.

## During-encounter constraints (proposed)
- Deployment costs Marbles.
- Deployment uses a limited **Build Tokens** budget per turn.
- Building placements are capped per encounter or per objective phase.
- Reinforcements may require adjacency to fort/outpost/spawner nodes.
- Emergency deploy can be a class perk/event reward, not baseline spam.

## Resource definitions (clarity)
- **Action Points (AP)**: Used by units already on the board for tactical actions like move, attack, and ability use.
- **Build Tokens (BT)**: Separate per-turn budget for placing new units/buildings/traps during an encounter.
- AP controls **what existing units do**. BT controls **whether you can add new board pieces**.

## Deployment Rules v1 (prototype defaults)
Use these values for first playable balance passes:

- **Start-of-encounter deployment phase**
  - Free placement phase before turn 1 using normal Marble costs.
  - No Build Token cost during this setup phase.
  - Building cap during setup: **max 2** non-fort structures.

- **Mid-encounter deployment**
  - Player gets **1 Build Token per player turn**.
  - Deploying any unit/building/trap consumes **1 Build Token**.
  - Mid-encounter deployment cap: **3 total deployments per encounter**.
  - Deployment still costs Marbles.
  - Deployment must be adjacent to a friendly **Pillow Fort / Outpost / valid spawner anchor**.

- **Type-specific limits**
  - Units: allowed mid-encounter.
  - Buildings: max **1** mid-encounter building deploy per encounter.
  - Traps/utility: allowed mid-encounter, but each trap type has **1-turn local cooldown** before another of the same type can be placed.

- **Fail-safe pacing rules**
  - If no valid deploy tile exists, Build Token is not consumed.
  - If encounter objective is in final state (for example final defense wave), disable new building placements but allow unit reinforcements.

- **Building cost model (locked v1)**
  - Buildings consume **Build Tokens + Marbles**, but do **not** consume unit AP.
  - This keeps AP readable for combat turns while letting deployment remain strategically gated.

## Player clarity rule
Always show:
- remaining Marbles
- remaining Build Tokens
- remaining placements this encounter

---

## 9) Objective Variety (non-confusing set)

Ship with a small, readable objective pool:
1. **Defend Fort (Bedroom)** - survive N waves/turns.
2. **Defeat All Enemies** - clear all enemy units on the map.
3. **Eliminate Captain** - kill marked elite unit.
4. **Hold Zone** - control a highlighted tile area for N turns.

Add new objective types only after these feel polished and readable.

---

## 10) MVP Scope Proposal

## Vertical Slice (first milestone)
- 1 biome: Bedroom Defense.
- 3 classes with 2 units each.
- Facing bonuses.
- One secondary mechanic per class (Momentum / Loose Leaf / Sharpness).
- Trampolines + 2 obstacle variants.
- Node-map prototype with 3 encounter nodes + 1 reward node.
- Marbles + Gold Stars.

## Success criteria
- Player can complete a 15-25 minute run.
- Build choices feel meaningful by encounter 3+.
- At least 2 viable playstyles emerge naturally.
- RPS relationship remains easy to understand.

---

## 11) Decision Log (locked)

1. **During-encounter deployment availability**
   - Unlocked via meta progression (not always available at run start).
2. **Building resource model**
   - Locked v1: Buildings consume Build Tokens + Marbles, not unit AP.
3. **Sharpness persistence**
   - Reset each encounter.
4. **Fort variants by biome**
   - Yes (for example Bed Fort, Desk Fort, Jungle Gym Fort).
5. **Elite anti-class inversion**
   - No partial inversion of core RPS assumptions.
