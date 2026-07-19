# Battle System Overview

High-level architecture of MARDEK Engine's turn-based battle system. This is a big-picture reference, not an exhaustive API reference — see the listed files for implementation details.

## Core Flow

Battles are driven by a central state machine, **`BattleManager`** (`Scripts/MARDEK/Battle/BattleManager.cs`), which cycles through states: `Idle → ChoosingAction → ActionPerforming → Concluding`. On start it spins up an `Encounter`, builds `HeroBattleParty`/`EnemyBattleParty` from the active roster, and hands turn order over to the **`TurnManager`**.

Turn order uses an **ATB (Active Time Battle) system**: each combatant accumulates an `ACT` meter (0–1000) at a rate based on their Agility stat. Whoever fills the meter first acts next. Player turns wait for input through the action UI; enemies act out a skill/target chosen ahead of time (see Enemy actions below) — the choice itself is still random (real enemy AI is a stubbed/unimplemented placeholder).

Performing an action flows: **UI/AI selection → gather eligible reaction skills → optional reaction-bar minigame → `IBattleAction.TryPerformAction` → battle model animation (melee/breath/cast) → effect resolution → end-of-turn status ticks → turn cleanup**, which checks for battle end. Defeated combatants stay in their party lists with `IsDead` set (a dead enemy's visual model is hidden, not destroyed); turn order, targeting, and the victory check all filter on `IsDead`.

## Characters & Stats

- **`Character`** (ScriptableObject) is persistent character data, subclassed into **`CharacterPlayable`** (heroes — carries four reaction-skill lists split by damage type/direction — `PhysicalAttackReactions`, `MagicAttackReactions`, `PhysicalDefenseReactions`, `MagicDefenseReactions` — and a `PassiveSkillset`) and **`CharacterUnplayable`** (enemies — carries loot `Drops` and an `ExperienceReward`). `HeroBattleCharacter.Character`/`EnemyBattleCharacter.Character` are typed to the correct subclass (not the base `Character`), so this hero/enemy-specific data is actually reachable at battle time.
- In battle, characters are wrapped as **`BattleCharacter`** (hero/enemy subclasses) — a MonoBehaviour on the root of the character's battle-model prefab, initialized from the persistent `Character` via `LoadCharacter`. It holds a runtime copy of stats, current HP/MP, ACT meter, and a link to the `BattleModelAnimator` (visuals live on a "Model" child, so they can be hidden on death while the component stays alive).
- **`CoreStats`** defines the stat block: STR/VIT/SPI/AGI, Attack/Defense/Magic Defense, elemental absorption, Accuracy/CritRate, and status resistances. Max HP/MP are computed via pluggable calculator assets.
- **Status effects** use a buildup-vs-resistance model. `TickStartOfTurnEffects` (from `BeginTurn`) and `TickEndOfTurnEffects` (from `BattleManager.EndTurn`) are separate passes, so effects that trigger "at the start" vs. "at the end" of a turn (e.g. Bleed vs. Poison) actually fire at different points rather than both ticking from the same call. Both read resistance/max-HP from `VolatileStats` (equipment-aware), not the base profile. Implemented:
  - **Poison** — 5% max-HP damage at the end of the turn.
  - **Bleed** — 5% max-HP damage at the start of the turn.
  - **Regen** — 5% max-HP heal at the end of the turn.
  - **Paralysis** — toggles a stun flag every start-of-turn tick, so it skips every other turn.
  - **Sleep** — blocks acting entirely while active (checked in `BeginTurn`); clears immediately when the character takes damage (via the shared `BattleCharacter.TakeDamage` chokepoint), otherwise decays like any other status.
  - **Blindness** — halves the character's effective Accuracy and makes normally-guaranteed magic damage roll a miss chance too (see `RollHit` below).
  - **Silence** / **Numbness** — cut outgoing magic/physical damage to 1/3 (`BattleCharacter.MagicDamageMultiplier`/`PhysicalDamageMultiplier`).
  - **Curse** — blocks the Skillset menu from populating (`ListCharacterSkillset`); basic Attack and items are separate UI paths and stay usable.
  - **Zombification** — only its "take damage from healing" half is wired (`DefaultHeal` inverts for it, same as the permanent Undead check); the "attack allies with basic attacks" half is deferred (see Notable Gaps).
  - Buildup for **Confusion**, **Berserk**, and **Haste** decays normally but has no behavior wired yet (also see Notable Gaps).

## Actions

Skills are **data-driven via ScriptableObjects** rather than hardcoded per-character logic. "Actions" is the umbrella for anything a combatant can do on their turn — skills and items both plug into the same pipeline.

### The `IBattleAction` pipeline

`IBattleAction` (icon, display name, `TryPerformAction(user, targets, reactionModifiers)`) is the shared contract. Two things implement it:

- **`ActionSkill : Skill, IBattleAction`** — has an MP `Cost` and a `BattleAction`. `TryPerformAction` checks `user.CurrentMP >= Cost` (doubled when targeting everyone on a side — see Targeting), applies the action, then deducts the cost. It also grants the caster flat skill-use experience, but only when the action didn't kill anyone (see Rewards).
- **`ExpendableItem : Item, IBattleAction`** — mirrors the same shape (own `BattleAction`, delegates to `action.Apply`) but has no MP gate and is excluded from the reaction system. Basic **"Attack" is not a special case** — `AttackButton` just holds a serialized `ActionSkill` (with `Cost = 0`), reusing the exact same skill machinery as spells.

`BattleAction` itself is the reusable effect-executor both of the above wrap: it holds an `Element`, a list of `SoundEffect`s, a `TargetScope`, and a polymorphic array of `ActionEffects` (`[SerializeReference, SubclassSelector]`, so designers compose new skills from effect building blocks directly in the inspector without code). `Apply(user, targets, efficacyMultiplier, reactionModifiers)` runs every effect against every target in order, then plays the sound effects once regardless of target count.

### Action Effects catalog

Each `ActionEffects` subclass implements `ApplyEffect(user, target, element, efficacyMultiplier, reactionModifiers)`:

| Effect | Behavior |
|---|---|
| `DealMeleeDamageStandard` | Rolls a hit check (`RollHit`, see below); `attack = motionValue * user.Attack * elementalVulnerability`, run through the shared damage formula against `target.Defense` and `user.Strength`, then scaled/reduced by `efficacyMultiplier`, Silence/Numbness, and any active `ReactionModifiers` |
| `DealMagicDamageStandard` | Same shape, but vs. `target.MagicDefense` using `user.Spirit` as the power stat. Normally never misses — a miss check only rolls if the attacker is Blinded or a reaction is affecting accuracy |
| `DefaultHeal` | `heal = (numerator + target.MagicDefense) * user.Spirit * user.Level / denominator`; inverts into damage against Undead targets **or** a Zombified target |
| `ManaHeal` | Flat MP restore |
| `ConstHeal` | Flat HP restore, no stat scaling |
| `DealStatusDamage` | Adds to `target.StatusBuildup` for a given status effect - unless a successful defensive reaction has that status in `ReactionModifiers.ResistedStatuses`, in which case it's blocked entirely |

The shared damage formula (`CalculateDamageStandard`): defense is halved and subtracted from attack, scaled by an attack/(attack+defense) ratio, then multiplied by a power term derived from the relevant stat and level, and finally randomized by a variance window (e.g. ±10%). Elemental vulnerability is `1 - absorption/100` from the target's `CoreStats`.

**`RollHit(user, accuracyMultiplier)`** computes a hit chance from `user.Accuracy` (halved if the user is Blinded) times whatever multiplier the caller passes in (a skill's own accuracy rating, folded together with any active `ReactionModifiers.AccuracyMultiplier`), then rolls against it with `Random.value`.

### Targeting

`BattleAction.TargetScope` controls how many targets an action can hit:

- **`SingleOnly`** — always one target, no toggle offered.
- **`SingleOrAll`** — the player can toggle between one target and everyone on a side.
- **`AllOnly`** — always hits everyone on a side, no single-target fallback.
- **`Multiselect`** — reserved for choosing an arbitrary subset of targets; not implemented yet.

`BattleCharacterPicker` is the cursor that walks the hero/enemy party lists (sorted by screen position). Pressing further into the side already being viewed toggles targeting everyone on it (`TargetingAll`) instead of doing nothing — only offered when there's more than one eligible target, so it's never a no-op stand-in for single-select. Moving the cursor vertically always cancels back to single-target. `AllOnly` actions skip navigation entirely and lock onto everyone eligible. Downed heroes stay individually selectable (e.g. for a future revival skill) but are excluded from "heal all" (`HealableHeroes`) since there's no resurrection mechanic yet.

Hitting everyone on a side costs double MP and applies `efficacyMultiplier = 0.5` per target (see `ActionSkill.TryPerformAction`) — determined purely by target count, so an action that only has one eligible target left is charged the normal single-target price. Breath actions used against everyone don't approach a specific target's hit point (there isn't one) — they move to a fixed **formation-center** point instead (`BattleManager.EnemyFormationCenter`/`HeroFormationCenter`), based on whichever side is being hit, mirroring how single-target breath already moves adjacent to a specific target's hit point.

### Call chain & ordering

1. Player picks an action in the UI → target picker arms with that `IBattleAction`.
2. On confirm: the picker (or, for enemies, `EnemyBattleCharacter.TakeAction`) gathers whichever reaction list is eligible — the acting hero's `PhysicalAttackReactions`/`MagicAttackReactions` if the actor is a hero, or the targeted hero's `PhysicalDefenseReactions`/`MagicDefenseReactions` if the target is a hero — picking physical vs. magic from `BattleAction.IsPhysicalAttack`/`IsMagicAttack` (neither for a non-damaging action, which gets no reaction window) — and calls `BattleManager.PerformActionToTarget(action, target(s), offensiveSkills, defensiveSkills)`.
3. `PerformActionToTarget` sets state to `ActionPerforming`, plays the reaction-bar minigame if either skill list is populated (see Reactions below), then calls `attacker.PerformAction(action, targets, reactionModifiers)`.
4. `BattleCharacter.PerformAction` calls `action.TryPerformAction(actor, targets, reactionModifiers)` **synchronously inside the local `ApplyAction()` function** — cost is checked, `BattleAction.Apply` runs, and every effect (damage/heal, HP/MP changes, floating damage numbers, "Hurt" reaction animation) is fully resolved for every target *before* any attack animation plays.
5. Only afterward does `BattleModelAnimator.PlayAction` run: Melee/Breath play the full approach → strike → jump-back sequence (or the formation-center variant for a multi-target breath); Spellcast/Item just play their animation in place. In other words the strike/cast animation is cosmetic and sequenced after the numbers are already applied, rather than driving the effect timing.
6. The coroutine finishes → `BattleManager.EndTurn()` ticks end-of-turn status effects for the character whose turn it was, checks win/lose, and advances turn order.

### Enemy actions

Enemies use the **identical** pipeline — there's no separate resolution path, beyond enemies never getting their own reaction window (see Reactions). Each `EnemyBattleCharacter` holds `NextAction`/`NextTarget`, populated by `ChooseNextAction` — currently a uniform-random pick of an `ActionSkill` from the enemy's skillset and a living hero target. This is also where the earlier-noted "no real AI" gap lives — selection is uniform-random, not strategic. Every enemy is queued up once at battle start (`BattleManager.InstantiateEncounter`), and each enemy re-queues its *next* attack immediately after consuming the current one in `TakeAction` — so a living enemy always has a decided-but-not-yet-executed action. If the queued target died in the meantime, `TakeAction` re-picks a living target at execution time rather than using the stale one.

### Turn economy

There's no action-point or extra-turn system beyond MP: one `IBattleAction` always consumes exactly one turn (doubled MP cost for hitting everyone on a side), `EndTurn()` runs unconditionally after every action regardless of type, and ACT (the turn-order meter) only resets for the acting character once their turn concludes. Skills are grouped per-character into skillsets (action/passive), stored as data assets — passive skills exist as a type but aren't wired into the action pipeline at all (see Notable Gaps).

## Reactions

A timing-based mini-game that lets a hero modify the outcome of an attack, either one they're making or one they're on the receiving end of. `Scripts/MARDEK/UI/Battle/ReactionBar.cs` positions a stationary `reactionRegion` on the canvas, then slides a `slider` marker across it over `TimeToCross` seconds (after an initial `DelaySeconds`); pressing the same "Interact" input `BattleCharacterPicker` uses for confirmation succeeds if the slider is currently overlapping the region. Missing (wrong timing, or never pressing) has no penalty — the action just resolves normally.

- **Reaction skills** live directly on `CharacterPlayable` as four typed lists — `PhysicalAttackReactions` (`List<PhysicalAttackReactionSkill>`), `MagicAttackReactions` (`List<MagicAttackReactionSkill>`), `PhysicalDefenseReactions` (`List<PhysicalDefenseReactionSkill>`), `MagicDefenseReactions` (`List<MagicDefenseReactionSkill>`) — rather than a shared skillset asset, since a skillset added no value over the lists themselves. Each of the four is its own concrete C# type (`Scripts/MARDEK/Character/Skills/ReactionSkill.cs`), not just a shared class filed into four lists, so the Inspector's object picker for e.g. `MagicAttackReactions` can only ever offer `MagicAttackReactionSkill` assets. The four all derive from two shared abstract layers: `ReactionSkill` (holds `Strength`, declares `Apply(ReactionModifiers)`) → `OffensiveReactionSkill`/`DefensiveReactionSkill` (hold the actual `Apply` logic, switching on an enum - `OffensiveReactionType`: PercentDamageBonus, CritRateBonus, AccuracyBonus; `DefensiveReactionType`: FlatDamageReduction, PercentDamageReduction, ElementalDamageReduction, StatusResist, EvasionBonus, the latter two carrying an `Element`/`StatusEffect` respectively for the Elemental/Status cases) → the four leaf classes, which add nothing beyond a distinct identity and a `[CreateAssetMenu]`. Which pair of lists (physical vs. magic) is eligible for a given action is decided by `BattleAction.IsPhysicalAttack`/`IsMagicAttack` (does its `actionEffects` array contain a `DealMeleeDamageStandard` or `DealMagicDamageStandard`), not by `ActionType` — a Breath skill built from magic-damage effects draws from the magic lists just like a Spellcast would.
- **`ReactionModifiers`** (`Scripts/MARDEK/Battle/ReactionModifiers.cs`) is the mutable bag a successful reaction writes into: `DamageMultiplier`, `FlatDamageReduction`, a per-element damage multiplier, `AccuracyMultiplier`, `ResistedStatuses`, and `CritRateBonus` (stored but not read anywhere yet — there's no crit-hit system). A fresh instance is all-neutral, so "no reaction happened" needs no special-casing downstream — it flows through `TryPerformAction`/`BattleAction.Apply` into every `ActionEffects.ApplyEffect` call regardless of whether a reaction actually fired.
- **Player-only**: enemies never get a reaction window of their own, whether attacking or defending. Offensive and defensive windows are also mutually exclusive per action in practice — whichever side the caller gathers skills for is the only one that can trigger.
- **No selection UI yet** — if a hero has multiple matching reaction skills, all of them apply together on a successful hit (`BattleManager.PlayReactionAndApply`), rather than the player picking one.
- `BattleManager`'s `DefaultReactionParams` (bar width, time to cross, line position, start delay) are placeholder constants shared by every reaction right now, not yet driven by per-skill data.

## UI Integration

- **`BattleUIManager`** coordinates the action-selection menu and character inspection, and fires victory events.
- **`BattleCharacterPicker`** handles targeting — cycling between valid hero/enemy targets, toggling target-all, and invoking the chosen action.
- **`ReactionBar`** plays the timing mini-game described above.
- Supporting UI: action menus/tabs, HP/MP/Experience bars, floating damage numbers, and a post-battle loot screen.

## Rewards

- **Gold**: `BattleManager.CalculateGoldReward` rolls `randInt(0, level² + randInt(1, 11)) × random(0.5, 1.5)` per defeated enemy and sums it, using each `EnemyBattleParty` member's own level. Like loot items, it's a pending reward (`GoldGained`) that isn't banked into the party's `InventorySO.Money` until collected from the loot screen's Get All button (`CollectGoldReward`).
- **Experience**: `HeroBattleCharacter.GrantKillExperience(baseReward, enemyLevel)` scales the killed enemy's `ExperienceReward` (a per-enemy value on `CharacterUnplayable`) by `1.1 ^ (enemyLevel - killerLevel)` — a level advantage/disadvantage is a constant percentage per level, rather than a raw ratio that flattens out at high levels. The killer receives `max(scaledReward, flat skill-use amount)`, and every other party member gets half of that same amount for free. `ActionSkill.TryPerformAction` separately grants a flat skill-use amount (`GrantSkillUseExperience`) on any successful skill use — but only when the action *didn't* kill anyone, since a kill's own reward already takes the max against that same flat amount (granting both would double-count it).
- **Leveling**: `HeroBattleCharacter.MaxExperience => Level * 1000`. The `Experience` property's setter checks this on every write, so any XP source triggers it automatically; on reaching the cap, XP resets to exactly 0 (excess discarded, not carried over) and `Level` increments once (both the battle-local value and the persistent `Character.Level`) — multiple thresholds crossed in one grant only level up once. `OnExperienceGained`/`OnLeveledUp` events fire from the same setter and drive `PlayerCharacterUI`'s floating "+X EXP"/"LEVEL UP!" text.
- **Victory/defeat**: victory syncs battle HP/MP back to the persistent party and plays a fanfare that depends on `Encounter.Type` (Standard/Grand); defeat plays a game-over jingle and reloads the last save file.

## Notable Gaps

- **Enemy AI** falls back to uniform-random skill/target selection (`BattleAI.cs` is an unused, unimplemented stub).
- **Passive skills** (`PassiveSkill.cs`) are a stub (`NotImplementedException`) and aren't wired into the action pipeline at all.
- **Confusion, Berserk, and the "attack allies" half of Zombification** all need a way to auto-perform "an attack" without player input, but there's no character-level concept of a basic attack to auto-perform yet — `AttackButton` just holds its own serialized `ActionSkill` in the UI layer, disconnected from `BattleCharacter`/`CharacterProfile`. Deferred until that's resolved (see the `status-effects-basic-attack-gap` memory note).
- **Haste** isn't wired to anything yet (unlike the above, it isn't blocked on anything — it just hasn't been done).
- **Crit** doesn't exist as a system — `CoreStats.CritRate` and `ReactionModifiers.CritRateBonus` are both stored but never read.
- **Reaction skill selection** — a hero with multiple eligible reaction skills has all of them fire together; there's no UI for picking one.
- **`TargetScope.Multiselect`** and **AOE melee** are unimplemented — the enum case and the plumbing exist, but nothing authors or exercises them yet.

## Key Files

| Area | Path |
|---|---|
| Battle loop / turns | `Scripts/MARDEK/Battle/BattleManager.cs`, `TurnManager.cs` |
| Actions & effects | `Scripts/MARDEK/Battle/BattleAction.cs`, `Scripts/MARDEK/Character/Skills/Action Effects/Action Effects.cs` |
| Battle characters | `Scripts/MARDEK/Battle/BattleCharacter.cs`, `HeroBattleCharacter.cs`, `EnemyBattleCharacter.cs` |
| Reactions | `Scripts/MARDEK/Character/Skills/ReactionSkill.cs`, `Scripts/MARDEK/Battle/ReactionModifiers.cs`, `Scripts/MARDEK/UI/Battle/ReactionBar.cs` |
| Visuals | `Scripts/MARDEK/Battle/BattleModelAnimator.cs` |
| Encounters | `Scripts/MARDEK/Battle/Encounter.cs`, `EncounterSet.cs`, `RandomEncounterGenerator.cs` |
| Character data | `Scripts/MARDEK/Character/Character.cs`, `CharacterPlayable.cs`, `CharacterUnplayable.cs`, `PartySO.cs`, `BattleAI.cs` |
| Skills | `Scripts/MARDEK/Character/Skills/Skill.cs`, `ActionSkill.cs`, `PassiveSkill.cs`, `ReactionSkill.cs` |
| Skillsets | `Scripts/MARDEK/Character/Skillsets/` |
| Stats | `Scripts/MARDEK/Stats/CoreStats.cs`, `Element.cs`, `MaxStats/` |
| UI | `Scripts/MARDEK/UI/Battle/BattleUIManager.cs`, `AttackButton.cs`, `BattleActionUI.cs`, `ReactionBar.cs`, `Loot/BattleLoot.cs`, `Scripts/MARDEK/UI/HealthBar.cs`, `ManaBar.cs`, `ExperienceBar.cs`, `PlayerCharacterUI.cs` |
| Data assets | `ScriptableObjects/Characters/Skills/**`, `ScriptableObjects/Stats/**` |
