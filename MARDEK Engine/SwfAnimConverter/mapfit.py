"""Propose Flash-depth -> Unity-bone mapping by matching idle-frame positions
(converted with a fitted global offset) against prefab bone world positions."""
import json, math, sys
from itertools import permutations
from swfanim import unity_pos

def fit(timeline_file, bones, limb_depths, idle_frame=5):
    tl = json.load(open(timeline_file))
    st = tl['frames'][idle_frame - 1]
    dpos = {}
    for d in limb_depths:
        m = st.get(str(d))
        if m is None:
            print(f"  depth {d}: MISSING at idle frame")
            continue
        dpos[d] = unity_pos(m)
    # initial offset: mean difference
    bn = list(bones)
    dn = list(dpos)
    for _ in range(4):
        mx = sum(bones[b][0] for b in bn)/len(bn) - sum(dpos[d][0] for d in dn)/len(dn)
        my = sum(bones[b][1] for b in bn)/len(bn) - sum(dpos[d][1] for d in dn)/len(dn)
        # greedy assignment by nearest
        pairs = []
        used_b, used_d = set(), set()
        cands = sorted(((math.hypot(dpos[d][0]+mx-bones[b][0], dpos[d][1]+my-bones[b][1]), d, b)
                        for d in dn for b in bn))
        for dist, d, b in cands:
            if d in used_d or b in used_b:
                continue
            pairs.append((d, b, dist)); used_d.add(d); used_b.add(b)
        # refine offset from matched pairs
        mx = sum(bones[b][0] - dpos[d][0] for d, b, _ in pairs)/len(pairs)
        my = sum(bones[b][1] - dpos[d][1] for d, b, _ in pairs)/len(pairs)
    print(f"  offset ({mx:.2f},{my:.2f})")
    for d, b, _ in sorted(pairs, key=lambda p: int(p[0])):
        cx, cy = dpos[d][0]+mx, dpos[d][1]+my
        px, py = bones[b]
        print(f"  depth {d:>3} -> {b:16s}  conv ({cx:7.2f},{cy:7.2f})  prefab ({px:7.2f},{py:7.2f})  err {math.hypot(cx-px,cy-py):5.2f}")
    return {d: b for d, b, _ in pairs}

# prefab world positions (root bones; Head = Torso + local since torso rot=0)
CHARS = {
 'Emela': {
   'timeline': 'timeline2551.json',
   'limb_depths': [7,9,12,14,16,19,21,25,28,30,32,35],
   'bones': {
     'Torso': (-5.23,50.11), 'Head': (-4.23,61.11),
     'Right Shoulder': (-5.53,48.61), 'Left Shoulder': (5.57,48.51),
     'Right Forearm': (-8.34,40.36), 'Left Forearm': (10.86,40.25),
     'Right Thigh': (0.90,33.80), 'Left Thigh': (2.80,32.30),
     'Right Calf': (-7.20,21.40), 'Left Calf': (4.00,20.00),
     'Right Foot': (-10.70,8.20), 'Left Foot': (9.50,5.90)},
 },
 'Deugan': {
   'timeline': 'timeline2427.json',
   'limb_depths': [5,11,13,15,17,19,21,24,28,30,32,34,36],
   'bones': {
     'Torso': (3.40,40.90), 'Head': (2.90,52.50),
     'Right Shoulder': (-0.80,44.70), 'Left Shoulder': (10.10,46.30),
     'Right Forearm': (-1.90,36.00), 'Left Forearm': (15.47,36.52),
     'Right Thigh': (0.60,33.30), 'Left Thigh': (2.50,31.80),
     'Right Calf': (-7.50,20.90), 'Left Calf': (3.70,19.50),
     'Right Foot': (-13.60,6.50), 'Left Foot': (4.20,3.60)},
 },
 'Vehrn': {
   'timeline': 'timeline2651.json',
   'limb_depths': [11,14,16,18,20,23,26,30,32,34,36,39],
   'bones': {
     'Torso': (-3.30,47.00), 'Head': (-3.80,58.60),
     'Right Shoulder': (-2.30,45.10), 'Left Shoulder': (8.60,45.90),
     'Right Forearm': (-3.40,36.40), 'Left Forearm': (14.50,36.92),
     'Right Thigh': (-0.90,33.70), 'Left Thigh': (1.00,32.20),
     'Right Calf': (-9.00,21.30), 'Left Calf': (2.20,19.90),
     'Right Foot': (-14.50,7.00), 'Left Foot': (3.30,4.10)},
 },
}

for name, cfg in CHARS.items():
    print(f"== {name} ==")
    fit(cfg['timeline'], cfg['bones'], cfg['limb_depths'])
