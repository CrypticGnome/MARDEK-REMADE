# Plan: SWF Character Catalog Script

## Problem

`SwfAnimConverter` (see [its README](../../SwfAnimConverter/README.md)) can convert a battle
model's animations from the original Flash game once you know its `DefineSprite` character id
(a plain integer, e.g. Vehrn = 2651). Today that id is found by hand: open the JPEXS XML dump of
the SWF, find sprite **5118** (the battle-model selector), and read off `frame label -> characterId`
for the monster/character you want.

That's fine for one character at a time, but it means every new import starts with a manual
lookup, and there's no durable record of what's already been identified. We want a script that
does this lookup once, for every character in the selector, and writes the results to a CSV so
future imports (rats included) are a lookup, not a re-investigation.

This plan is intentionally implementation-light: the actual `swf.xml` isn't on this machine right
now, so anything about the SWF's exact structure below is inferred from `parse_sprite.py`/README
and needs confirming against the real file before being trusted.

## Goal

A standalone script, e.g. `SwfAnimConverter/catalog_characters.py`, that:

1. Reads the JPEXS XML dump (same input as `parse_sprite.py`).
2. Parses sprite 5118 (the selector) to get `name -> characterId` for every battle model.
3. For each discovered id, inspects that character's own `DefineSprite` timeline to confirm it's
   a real animated battle model (has the expected frame labels) and to record where its
   animation sections live.
4. Optionally (stretch goal, see below) walks the character's sub-parts (weapon/shield/limbs/etc)
   to catalog everything the character depends on.
5. Writes one row per character to a CSV.

## Grounding: what we already know from `parse_sprite.py`

`parse_sprite.py` already contains all the low-level parsing logic this script needs, just scoped
to one sprite at a time:

- It streams a JPEXS XML dump and recognizes `DefineSpriteTag` (by `spriteId`), `PlaceObject2Tag`
  (`depth`, `characterId`, matrix), `RemoveObject2Tag`, `FrameLabelTag` (`name` -> frame number),
  `DoActionTag` (decoded bytecode strings), and `ShowFrameTag` (advances the frame counter).
- Its debug output already prints, per sprite, `labels` (name -> frame number) and a `depth ->
  {charIds seen at that depth}` map — this is exactly the "what sub-parts does this character use"
  data the stretch goal needs; it just isn't extracted to a name/id table today, only printed.
- Sprite 5118 is structured the same way as any other sprite: it should be parseable by the exact
  same state machine, just interpreting `labels` as monster/character names and matching each
  label's frame to whatever `characterId` was placed at that frame.

This means the new script can mostly **reuse** `parse_sprite.py`'s parsing loop (either by
importing it as a module — it isn't currently written as one, so it would need a light refactor
to expose a `parse(path, sprite_id=None) -> {frames, labels, actions}` function — or by
duplicating the loop). Refactoring `parse_sprite.py` into an importable function is probably the
right first step so both scripts share one parser instead of drifting apart.

## Proposed CSV schema

One row per character/monster found in the selector:

| column | meaning |
|---|---|
| `id` | `DefineSprite` character id (int) |
| `name` | frame label from sprite 5118 (e.g. `vehrn`, `fumerat`) |
| `has_standard_labels` | bool — does this sprite's own timeline contain the expected animation frame labels (`idle`, `moveto`, `strike`, `jumpback`, `hit`, `die`, `dead`, `spellcast`, `useitem`, `victory`)? Lets us tell "real battle model" apart from a selector entry that points somewhere unexpected. |
| `found_labels` | semicolon-separated list of whichever of the standard labels were actually present (rats/simpler enemies may be missing some, e.g. no `spellcast`) |
| `frame_count` | total frames in that character's timeline, for a sanity gut-check |
| `sub_part_ids` | semicolon-separated list of distinct `charId`s referenced inside the timeline (candidate weapon/shield/limb/cape ids) — stretch goal, see below |
| `notes` | free text — anything that looked off during parsing (missing labels, zero frames, id collision, etc) |

`where to find the animations` in the original ask is `id` + `found_labels`: the id says which
sprite to run `parse_sprite.py`/`convert.py` against, and `found_labels` says which of the ten
animation slots actually exist in it before spending time on a full conversion.

## Stretch goal: cataloging "all data pertaining to a character"

`parse_sprite.py`'s existing depth/charId dump is the seed for this, but full fidelity would mean
recursively resolving each referenced `charId` to find out whether it's itself a `DefineSprite`
(walk into it too) or a leaf `DefineShape`/image (stop). That requires knowing what other tag
types the XML dump uses for shapes/images, which we haven't inspected — `parse_sprite.py` only
ever looks for `PlaceObject2Tag`/`RemoveObject2Tag`/`DoActionTag`/`FrameLabelTag`/`ShowFrameTag`
inside a sprite block, not the top-level tag stream where shape/image definitions would live. This
piece can't be scoped further without the actual file in hand — leaving it as a documented
follow-up rather than guessing at a tag vocabulary we can't verify.

## Implementation sketch (steps, not final code)

1. Refactor `parse_sprite.py`'s parsing loop into an importable `parse(lines_or_path, sprite_id=None) -> dict` so it can be reused for both a single-character extraction and the catalog scan, instead of duplicating the regex/state-machine logic in a second script.
2. Write `catalog_characters.py`:
   - Parse sprite 5118 with the shared parser to get `labels` (name -> frame) and per-frame
     `charId` at whatever depth places the model (need to confirm from the real data which depth
     that is — likely there's just one active depth per frame in the selector, but confirm rather
     than assume).
   - For each `(name, id)` pair, stream-extract that character's own sprite block (same technique
     `parse_sprite.py` already uses to jump straight to a `spriteId` without loading the whole
     file) and check which standard labels are present.
   - Collect the distinct `charId`s seen across all depths in that character's frames for the
     `sub_part_ids` column.
   - Write everything to `characters.csv`.
3. Run it once against the real `swf.xml`, eyeball the output for the three rats (and anything
   else that looks off), and only then hand the confirmed ids back for the actual animation
   import.
4. Treat the CSV as a checked-in artifact (or at least the id/name columns) so future character
   additions are a lookup in this file, not a re-run of the manual sprite-5118 process.

## Open questions to settle once the real `swf.xml` is available

- Does sprite 5118 cover every enemy in the game (including the rats), or only a subset (e.g.
  only bosses/named humanoids), meaning some monsters might need a different discovery method?
- Is there ever more than one selector-style sprite (e.g. a separate one for common monsters vs
  named characters)? README only documents 5118.
- Are frame label names consistent in case/format across all characters (README examples are all
  lowercase-with-underscores), or do some (e.g. rats, being simpler/older content) use a different
  labeling convention that `has_standard_labels` needs to account for?
- How large is `swf.xml`? `parse_sprite.py`'s stream-extraction approach (read line-by-line,
  capture only lines between matching `DefineSpriteTag`s) implies it's too big to comfortably load
  wholesale — the catalog script scanning for sprite 5118 plus N character sprites should follow
  the same streaming discipline rather than parsing the whole file into memory per lookup.
