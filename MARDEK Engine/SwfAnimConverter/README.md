# SwfAnimConverter

Converts battle-model animations from the original MARDEK Flash game into Unity
legacy `.anim` clips, using a JPEXS XML dump of the SWF. The output plugs
straight into the `BattleModelComponent` slots on the battle-model prefabs.

Already converted: **Mardek Soldier** (2364), **Deugan** (2427), **Emela** (2551), **Vehrn** (2651).

## Requirements

- Python 3 (stdlib only — no packages needed)
- A JPEXS free flash decompiler XML export of the game SWF
  (currently at `C:\Users\joe\OneDrive\Documents\swf.xml`)

## How it works

Each battle model is a `DefineSprite` in the SWF whose timeline contains every
animation back-to-back, separated by frame labels (`idle`, `moveto`, `strike`,
`jumpback`, `hit`, `die`, `dead`, `spellcast`, `useitem`, `victory`). Every
frame stores an affine matrix per body part (per Flash *depth*). The pipeline:

1. **`parse_sprite.py`** — extracts one sprite's timeline into a JSON file:
   per-frame matrices per depth, frame labels, and decoded `DoAction`
   bytecode strings (used to find where sections end — `Animate("idle")`
   callbacks — and gameplay events like `ReactionChance`/`CastSpell`/`TossItem`).
2. **`convert.py`** — for a configured character:
   - derives the section boundaries from labels + actions (`derive_sections`),
   - decomposes each bone's Flash matrix per frame into rotation/scale/position
     (chained across the whole timeline so angles unwrap continuously through
     big rotations like the death fall),
   - **anchors** each bone to the Unity rig's bind pose and applies the Flash
     motion as a delta (see below),
   - reduces keyframes (RDP-style, evaluated with the exact Hermite tangents
     Unity will use) within tight tolerances,
   - writes one legacy `.anim` per animation slot.

### Why anchoring?

The Unity rigs and their existing Idle/Hurt clips were built by hand and sit a
few pixels / degrees off the original Flash placements. Emitting absolute
converted values would make bones pop when transitioning between a hand-made
clip and a converted one. Instead, for every bone:

```
theta_out = theta_anchor + theta_flash(f) - theta_flash(idle frame)
pos_out   = pos_anchor   + pos_flash(f)   - pos_flash(idle frame)
scale_out = scale_anchor * scale_flash(f) / scale_flash(idle frame)
```

The anchor is the bone's pose at t=0 of the character's existing hand-made
Idle clip (`anchors_from_anim`), falling back to the prefab bind pose
(`prefab_anchors`) for bones the idle clip doesn't cover (feet, head) or for
characters with no hand-made clips at all (Vehrn). Result: at the idle pose the
model looks exactly like the rig was authored, and all motion on top of it is
faithful to the original game.

## Usage

```sh
# 1. extract the sprite timeline (writes timeline<ID>.json)
python parse_sprite.py "path/to/swf.xml" timeline2651.json 2651

# 2. generate the clips
python convert.py Vehrn out_Vehrn
```

Then copy the `.anim` files into the right folder under
`Assets/Animations/Model Animations/Battle Model Animations/Humanoid/<Char>/`,
create a `.meta` per file (fresh GUID, `NativeFormatImporter`,
`mainObjectFileID: 7400000`), and reference the GUIDs from the prefab's
`BattleModelComponent` slots **and** its `Animation` component's
`m_Animations` list (legacy `Animation.Play(name)` only finds clips in that list).

## Adding a new character

1. **Find the sprite id.** Sprite **5118** is the game's battle-model selector:
   each frame is labeled with a character/monster name and places that model's
   sprite. Look up the name there (e.g. `vehrn -> 2651`, `steele_soldier -> 2589`,
   `zach -> 2619`, `sharla -> 2801`, `Muriance -> 4746`, ...). All battle models
   also carry a `useitem` frame label if you'd rather grep.
2. **Extract the timeline** with `parse_sprite.py`. The summary it prints shows
   the depth/charId structure: paired charIds (same symbol placed twice) are
   left/right limb pairs; singles are torso, head, weapon, shield (and
   occasionally extras like Deugan's cape).
3. **Derive the depth→bone mapping.** Get the prefab's bone positions with
   `dump_prefab.py`, put them plus the candidate depths into `mapfit.py`, and
   run it. It converts each depth's idle-frame position (twips/20, y-flip),
   fits the global offset, and proposes assignments. Sanity-check against the
   pair structure and the depth-order rule (**lower depth = the character's
   right side**, drawn behind). Weapon/shield are matched by charId, not
   position — they can move between depths mid-timeline (draw-order changes).
4. **Add a config entry** in `convert.py` (`CONFIG` dict): timeline file,
   clip name prefix, mapping, anchors, and which slots to emit.
5. **Generate, install, wire the prefab** as above.

## Fidelity notes & caveats

- Timings are the original 30 fps frame timings (idle 1.0 s, hit 0.5 s,
  die 0.6 s, spellcast ~1.73 s ...). Gameplay callbacks in the original
  happen mid-clip (strike impact ≈ frame 8 of strike, spell release ≈ frame 21
  of spellcast, item toss ≈ frame 12 of useitem) — useful when hooking up
  battle logic later.
- `Die` ends exactly on the `Dead` pose; `Dead` is a static 2-key clip;
  `Idle`/`MoveTo` wrap cleanly for looping; `Victory` plays once and holds.
- Keyframe reduction tolerances are at the top of `convert.py`
  (`TOL_ROT_DEG = 0.6`, `TOL_POS_PX = 0.12`, `TOL_SCALE = 0.008`). The output
  is verified to stay within those bounds of the dense bake — all sub-pixel.
- **Hidden parts**: when the original removes a graphic (weapon sheathed during
  spellcast/useitem), the clip collapses that bone to scale 0 and restores it
  via the next clip's scale curves.
- **Sprite swaps are not supported**: the original swaps symbols mid-timeline
  (potion in hand during useitem, spell-hand during spellcast, Deugan's sword
  at a different draw depth during victory — the depth move *is* handled).
  Showing an actual item would need game-side effects.
- **Deugan's cape** (depth 5 in sprite 2427) has no Unity bone and is not
  converted.
- The pre-existing hand-made clips (Mardek/Emela/Deugan Idle + Hurt, plus the
  unslotted `Deugan_Attack`/`Emela_Spellcast`) were left untouched; converted
  clips fill only the empty slots. They could be regenerated from the SWF for
  full fidelity by adding those slots to the `emit` list (note: hand-made Hurt
  clips run 1.0 s vs the original 0.5 s).
- Existing `Idle`/`Hurt` clips animate only 11 bones (no feet/head); converted
  clips animate all 14. If a converted clip ever looks like it "sticks" after
  switching to Idle, that's the hand-made Idle not resetting feet/head — the
  converted clips all end at (or near) the idle pose precisely to avoid this.

## Files

| file | purpose |
|---|---|
| `parse_sprite.py` | SWF XML dump → `timeline<ID>.json` (matrices, labels, actions) |
| `convert.py` | character configs + clip generation (the main tool) |
| `swfanim.py` | matrix decomposition / coordinate conversion helpers |
| `mapfit.py` | proposes depth→bone mappings for new characters |
| `dump_prefab.py` | prints a prefab's bone tree + component slots |
| `parse_anim.py` | parses an existing `.anim` into JSON (debugging/anchors) |
