"""Compactly dump a battle-model prefab: bone tree with transforms, component slots."""
import re, sys, math

path = sys.argv[1]
txt = open(path, encoding='utf-8').read()

docs = re.split(r'--- !u!(\d+) &(\d+)( stripped)?\n', txt)
# docs[0] is header; then groups of (classid, fileid, stripped, body)
objs = {}
for i in range(1, len(docs) - 3, 4):
    cls, fid, stripped, body = docs[i], docs[i+1], docs[i+2], docs[i+3]
    objs[fid] = (int(cls), body)

gos = {}        # fileid -> name
comps = {}      # transform fileid -> data
for fid, (cls, body) in objs.items():
    if cls == 1:
        m = re.search(r'm_Name: (.*)', body)
        gos[fid] = m.group(1).strip()

trs = {}
for fid, (cls, body) in objs.items():
    if cls == 4:  # Transform
        go = re.search(r'm_GameObject: \{fileID: (\d+)\}', body)
        father = re.search(r'm_Father: \{fileID: (\d+)\}', body)
        pos = re.search(r'm_LocalPosition: \{x: ([-\d.e]+), y: ([-\d.e]+)', body)
        rot = re.search(r'm_LocalRotation: \{x: [-\d.e]+, y: [-\d.e]+, z: ([-\d.e]+), w: ([-\d.e]+)\}', body)
        scl = re.search(r'm_LocalScale: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)', body)
        trs[fid] = {
            'name': gos.get(go.group(1), '?') if go else '?',
            'father': father.group(1) if father else '0',
            'pos': (float(pos.group(1)), float(pos.group(2))),
            'theta': math.degrees(2*math.atan2(float(rot.group(1)), float(rot.group(2)))) if rot else 0.0,
            'scale': (float(scl.group(1)), float(scl.group(2)), float(scl.group(3))),
            'id': fid,
        }

def walk(fid, depth=0):
    t = trs[fid]
    print(f"{'  '*depth}{t['name']:20s} pos ({t['pos'][0]:7.2f},{t['pos'][1]:7.2f}) rot {t['theta']:8.2f} scale ({t['scale'][0]:.3f},{t['scale'][1]:.3f},{t['scale'][2]:.3f})  [tid {fid}]")
    for cid, ct in trs.items():
        if ct['father'] == fid:
            walk(cid, depth + 1)

roots = [fid for fid, t in trs.items() if t['father'] == '0' or t['father'] not in trs]
for r in roots:
    walk(r)

# BattleModelComponent slots + Animation
for fid, (cls, body) in objs.items():
    if cls == 114 and 'idle:' in body:
        print("\n-- BattleModelComponent --")
        for line in body.splitlines():
            if re.match(r'\s*(idle|moveto|strike|jumpback|hurt|die|dead|spellcast|useItem|victory|animation):', line):
                print(line)
    if cls == 111:
        print("\n-- Animation component (fileID in doc above) --")
        print(body[:body.index('m_WrapMode')])
