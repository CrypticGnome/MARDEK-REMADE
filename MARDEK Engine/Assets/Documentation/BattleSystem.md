# Battle System Overview

High-level architecture of MARDEK Engine's turn-based battle system. This is a big-picture reference, not an exhaustive API reference — see the listed files for implementation details.

## Core Flow

Battles are driven by a central state machine, **`BattleManager`** (`Scripts/MARDEK/Battle/BattleManager.cs`), which cycles through states: `Idle → ChoosingAction → ActionPerforming → Concluding`. On start it spins up an `Encounter`, builds `HeroBattleParty`/`EnemyBattleParty` from the active roster, and hands turn order over to the **`TurnManager`**.

Turn order uses an **ATB (Active Time Battle) system**: each combatant accumulates an `ACT` meter (0–1000) at a rate based on their Agility stat. Whoever fills the meter first acts next. Player turns wait for input through the action UI; enemy turns currently pick a random skill (real enemy AI is a stubbed/unimplemented placeholder).

Performing an action flows: **UI selection → `IBattleAction.TryPerformAction` → battle model animation (melee/cast) → effect resolution → turn cleanup**, which removes defeated combatants and checks for battle end.

## Characters & Stats

- **`Character`** (ScriptableObject) is persistent character data, subclassed into playable heroes and unplayable enemies (enemies carry loot tables).
- In battle, characters are wrapped as **`BattleCharacter`** (hero/enemy subclasses), holding a runtime copy of stats, current HP/MP, ACT meter, and a link to the visual battle model.
- **`CoreStats`** defines the stat block: STR/VIT/SPI/AGI, Attack/Defense/Magic Defense, elemental absorption, and status resistances. Max HP/MP are computed via pluggable calculator assets.
- **Status effects** (Poison, Sleep, Paralysis, Blindness, Silence, Confusion, Bleed, etc.) use a buildup-vs-resistance model, ticked each turn.

## Actions

Skills are **data-driven via ScriptableObjects** rather than hardcoded per-character logic. "Actions" is the umbrella for anything a combatant can do on their turn — skills and items both plug into the same pipeline.

### The `IBattleAction` pipeline

`IBattleAction` (icon, display name, `TryPerformAction(user, target)`) is the shared contract. Two things implement it:

- **`ActionSkill : Skill, IBattleAction`** — has an MP `Cost` and a `BattleAction`. `TryPerformAction` checks `user.CurrentMP >= Cost`, applies the action, then deducts the cost:
  ```csharp
  if (user.CurrentMP < Cost) return false;
  action.Apply(user, target);
  user.CurrentMP -= Cost;
  return true;
  ```
- **`ExpendableItem : Item, IBattleAction`** — mirrors the same shape (own `BattleAction`, delegates to `action.Apply`) but has no MP gate. Basic **"Attack" is not a special case** — `AttackButton` just holds a serialized `ActionSkill` (with `Cost = 0`), reusing the exact same skill machinery as spells.

`BattleAction` itself is the reusable effect-executor both of the above wrap: it holds an `Element`, a list of `SoundEffect`s, and a polymorphic array of `ActionEffects` (`[SerializeReference, SubclassSelector]`, so designers compose new skills from effect building blocks directly in the inspector without code). `Apply(user, target)` runs every effect in order, then plays the sound effects. It also exposes `IsMeleeAttack`, which scans for a `DealMeleeDamageStandard` effect — this flag later decides whether melee approach/strike animation plays.

### Action Effects catalog

Each `ActionEffects` subclass implements `ApplyEffect(user, target, element)`:

| Effect | Behavior |
|---|---|
| `DealMeleeDamageStandard` | Rolls an accuracy check; `attack = motionValue * user.Attack * elementalVulnerability`, run through the shared damage formula against `target.Defense` and `user.Strength` |
| `DealMagicDamageStandard` | Same shape, but vs. `target.MagicDefense` using `user.Spirit` as the power stat |
| `DefaultHeal` | `heal = (numerator + target.MagicDefense) * user.Spirit * user.Level / denominator`; inverts into damage against Undead targets |
| `ManaHeal` | Flat MP restore |
| `ConstHeal` | Flat HP restore, no stat scaling |
| `DealStatusDamage` | Adds to `target.StatusBuildup` for a given status effect |

The shared damage formula (`CalculateDamageStandard`): defense is halved and subtracted from attack, scaled by an attack/(attack+defense) ratio, then multiplied by a power term derived from the relevant stat and level, and finally randomized by a variance window (e.g. ±10%). Elemental vulnerability is `1 - absorption/100` from the target's `CoreStats`.

### Targeting

Targeting is handled entirely in the UI layer, not in the action data. `BattleCharacterPicker` is a cursor that walks the hero/enemy party lists (sorted by screen position); `EnableWithAction(action)` arms it with the chosen `IBattleAction`, and confirming a target calls `BattleManager.PerformActionToTarget(action, target)`. There's currently no AOE support and no built-in ally/enemy restriction in the picker — both party lists are freely navigable, so any targeting rules are left to the caller/UI context rather than enforced by the action system itself.

### Call chain & ordering

1. Player picks an action in the UI → target picker arms with that `IBattleAction`.
2. On confirm: `BattleManager.PerformActionToTarget` sets state to `ActionPerforming` and calls `action.TryPerformAction(actor, target)` **synchronously** — cost is checked, `BattleAction.Apply` runs, and every effect (damage/heal, HP/MP changes, floating damage numbers, "Hurt" reaction animation) is fully resolved *before* any attack animation plays.
3. Only afterward does a `PlayAttack()` coroutine run: if `BattleAction.IsMeleeAttack`, it plays the full melee approach → strike → jump-back sequence; otherwise it just waits ~1.5s. In other words the strike/cast animation is cosmetic and sequenced after the numbers are already applied, rather than driving the effect timing.
4. The coroutine finishes → `BattleManager.EndTurn()` clears dead characters, checks win/lose, and advances turn order.

### Enemy actions

Enemies use the **identical** pipeline — there's no separate resolution path. `PerformEnemyMove` (inside `BattleManager`'s turn loop) just substitutes random selection for player input: it picks a random `ActionSkill` from the acting enemy's skillset and a random hero target, then calls the same `PerformActionToTarget`. This is also where the earlier-noted "no real AI" gap lives — selection is uniform-random, not strategic.

### Turn economy

There's no action-point or extra-turn system beyond MP: one `IBattleAction` always consumes exactly one turn, `EndTurn()` runs unconditionally after every action regardless of type, and ACT (the turn-order meter) only resets for the acting character once their turn concludes. Skills are grouped per-character into skillsets (action/passive/reaction), stored as data assets — passive and reaction skills exist as types but aren't yet wired into the action pipeline (see Notable Gaps).

## UI Integration

- **`BattleUIManager`** coordinates the action-selection menu and character inspection, and fires victory events.
- **`BattleCharacterPicker`** handles targeting — cycling between valid hero/enemy targets and invoking the chosen action.
- Supporting UI: action menus/tabs, HP/MP bars, floating damage numbers, and a post-battle loot screen.

## Other Systems

- **Encounters** are defined as data assets listing enemies, level ranges, and rewards; a generator selects them for random battles.
- **Animation/VFX** are driven by a battle model component that plays idle, movement, attack, cast, hurt, death, and victory clips.
- **Victory/defeat**: victory syncs battle HP/MP back to the persistent party; defeat handling is not yet implemented.
- **Rewards**: item loot is functional; experience-point granting is not yet wired into the battle flow (UI for it exists but is unused).

## Notable Gaps

A few systems are present as scaffolding but not yet functional: enemy AI (falls back to random skill selection), passive/reaction skills, and defeat/XP handling. Worth keeping in mind when extending the battle system.

## Key Files

| Area | Path |
|---|---|
| Battle loop / turns | `Scripts/MARDEK/Battle/BattleManager.cs`, `TurnManager.cs` |
| Actions & effects | `Scripts/MARDEK/Battle/BattleAction.cs`, `Scripts/MARDEK/Character/Skills/Action Effects/Action Effects.cs` |
| Battle characters | `Scripts/MARDEK/Battle/BattleCharacter.cs`, `HeroBattleCharacter.cs`, `EnemyBattleCharacter.cs` |
| Visuals | `Scripts/MARDEK/Battle/BattleModelComponent.cs` |
| Encounters | `Scripts/MARDEK/Battle/Encounter.cs`, `EncounterSet.cs`, `RandomEncounterGenerator.cs` |
| Character data | `Scripts/MARDEK/Character/Character.cs`, `CharacterPlayable.cs`, `CharacterUnplayable.cs`, `PartySO.cs`, `BattleAI.cs` |
| Skills | `Scripts/MARDEK/Character/Skills/Skill.cs`, `ActionSkill.cs`, `PassiveSkill.cs`, `ReactionSkill.cs` |
| Skillsets | `Scripts/MARDEK/Character/Skillsets/` |
| Stats | `Scripts/MARDEK/Stats/CoreStats.cs`, `Element.cs`, `MaxStats/` |
| UI | `Scripts/MARDEK/UI/Battle/BattleUIManager.cs`, `AttackButton.cs`, `BattleActionUI.cs`, `Loot/BattleLoot.cs`, `Scripts/MARDEK/UI/HealthBar.cs`, `ManaBar.cs`, `ExperienceBar.cs` |
| Data assets | `ScriptableObjects/Characters/Skills/**`, `ScriptableObjects/Stats/**` |
