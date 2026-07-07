"""Parse JPEXS XML dump of a DefineSprite into a per-frame timeline.

Usage:
    python parse_sprite.py <sprite.xml> <out.json>           # pre-extracted sprite block
    python parse_sprite.py <swf.xml> <out.json> <spriteId>   # extract from full dump

Output: timeline.json
  frames:  list (frame N = index N-1) of {depth(str): {a,b,c,d,tx,ty,charId}}
  labels:  {name: frameNumber}
  actions: [(frameNumber, [decoded strings from DoAction bytecode])]
SWF MATRIX semantics: x' = ScaleX*x + RotateSkew1*y + Tx ; y' = RotateSkew0*x + ScaleY*y + Ty
Stored raw (Flash coords, twips). hasScale=false => scale 1; hasRotate=false => skew 0.
"""
import re, json, sys

src = sys.argv[1]
out = sys.argv[2]
sprite_id = sys.argv[3] if len(sys.argv) > 3 else None

item_re = re.compile(r'<item type="(\w+)"([^>]*)/?>')
attr_re = re.compile(r'(\w+)="([^"]*)"')
matrix_re = re.compile(r'<matrix type="MATRIX"([^>]*)/>')

if sprite_id:
    # stream-extract just the requested DefineSprite block from a full dump
    lines = []
    capturing = False
    with open(src, encoding='utf-8') as f:
        for line in f:
            if not capturing:
                if 'DefineSpriteTag' in line and f'spriteId="{sprite_id}"' in line:
                    capturing = True
                    lines.append(line)
            else:
                if 'DefineSpriteTag' in line and f'spriteId="{sprite_id}"' not in line:
                    break
                lines.append(line)
    if not lines:
        sys.exit(f"spriteId {sprite_id} not found in {src}")
else:
    lines = open(src, encoding='utf-8').read().splitlines()

frames = []          # frames[i] = state dict AT frame i+1
state = {}           # depth -> dict
labels = {}
actions = []         # (frame, [strings]) decoded from DoAction bytecode
cur_frame = 1

i = 0
n = len(lines)
while i < n:
    line = lines[i]
    m = item_re.search(line)
    if not m:
        i += 1
        continue
    kind = m.group(1)
    attrs = dict(attr_re.findall(m.group(2)))
    if kind == 'PlaceObject2Tag':
        depth = attrs['depth']
        has_char = attrs.get('placeFlagHasCharacter') == 'true'
        has_matrix = attrs.get('placeFlagHasMatrix') == 'true'
        is_move = attrs.get('placeFlagMove') == 'true'
        # find matrix on following lines (inside this item) if present
        mat = None
        if has_matrix:
            j = i + 1
            while j < n and '<matrix' not in lines[j] and '</item>' not in lines[j] and '<item' not in lines[j]:
                j += 1
            if j < n and '<matrix' in lines[j]:
                mm = matrix_re.search(lines[j])
                ma = dict(attr_re.findall(mm.group(1)))
                has_scale = ma.get('hasScale') == 'true'
                has_rot = ma.get('hasRotate') == 'true'
                mat = {
                    'a': float(ma['scaleX']) if has_scale else 1.0,
                    'd': float(ma['scaleY']) if has_scale else 1.0,
                    'b': float(ma['rotateSkew1']) if has_rot else 0.0,  # x' coeff of y
                    'c': float(ma['rotateSkew0']) if has_rot else 0.0,  # y' coeff of x
                    'tx': float(ma['translateX']),
                    'ty': float(ma['translateY']),
                }
        prev = state.get(depth)
        ent = dict(prev) if (is_move and prev) else {'a':1.0,'b':0.0,'c':0.0,'d':1.0,'tx':0.0,'ty':0.0,'charId':None}
        if has_char:
            ent['charId'] = attrs.get('characterId')
        if mat is not None:
            ent.update(mat)
        state[depth] = ent
    elif kind == 'RemoveObject2Tag':
        state.pop(attrs['depth'], None)
    elif kind == 'DoActionTag':
        b = bytes.fromhex(attrs.get('actionBytes', ''))
        strings = [s.decode() for s in re.findall(rb'[ -~]{3,}', b)]
        actions.append((cur_frame, strings))
    elif kind == 'FrameLabelTag':
        labels[attrs['name']] = cur_frame
    elif kind == 'ShowFrameTag':
        frames.append({d: dict(v) for d, v in state.items()})
        cur_frame += 1
    i += 1

json.dump({'frames': frames, 'labels': labels, 'actions': actions}, open(out, 'w'), indent=None)
print(f"frames: {len(frames)}, labels: {labels}")
for f, s in actions:
    print(f"  action @f{f}: {s}")
depth_chars = {}
for f in frames:
    for d, v in f.items():
        depth_chars.setdefault(d, set()).add(v['charId'])
for d in sorted(depth_chars, key=int):
    print(f"depth {d}: chars {sorted(depth_chars[d], key=lambda x: (x is None, x))}")
