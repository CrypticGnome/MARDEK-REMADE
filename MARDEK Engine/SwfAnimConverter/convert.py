"""SWF -> Unity legacy .anim converter for MARDEK battle models.

Usage:  python convert.py <Character> <outdir>
        python convert.py --list

Reads timeline<spriteId>.json (produced by parse_sprite.py) plus the character
config below, and writes one .anim per battle animation slot.

Method (see README.md): decompose each bone's Flash matrix per frame (chained
for angle continuity), then anchor to the Unity rig's bind pose:
    theta_out = theta_anchor + theta(f) - theta(idle_ref)
    pos_out   = pos_anchor   + pos(f)   - pos(idle_ref)
    scale_out = scale_anchor * scale(f) / scale(idle_ref)
Anchors come from the character's existing hand-made Idle.anim t=0 pose where
one exists, otherwise from the prefab bind pose.
"""
import json, math, os, re, sys
from swfanim import unity_linear, unity_pos, decompose, mat_mul, mat_inv

FPS = 30.0

# ---------------------------------------------------------------------------
# Character configs
#
# mapping: Unity bone path -> (flash depths tried in order, parent depth, required charId)
#   - required charId hides the bone (scale 0) when absent (weapon sheathed etc.)
#   - depths [] = bone has no Flash counterpart; emitted static at anchor pose
# anchors_from_anim: existing Idle .anim whose t=0 pose anchors those bones
# prefab_anchors: bind pose for bones not covered by the idle anim
#   {'pos': (x,y), 'theta_deg': z, 'scale': (x,y,z)}
# emit: which slots to generate (labels: idle/moveto/strike/jumpback/hit/die/
#       dead/spellcast/useitem/victory)
# ---------------------------------------------------------------------------

ANIMROOT = r"U:/MARDEK-REMADE/MARDEK Engine/Assets/Animations/Model Animations/Battle Model Animations/Humanoid"

ALL_SLOTS = ['idle', 'moveto', 'strike', 'jumpback', 'hit', 'die', 'dead', 'spellcast', 'useitem', 'victory']
MISSING_8 = ['moveto', 'strike', 'jumpback', 'die', 'dead', 'spellcast', 'useitem', 'victory']

CONFIG = {
 'MardekSoldier': {
   'timeline': 'timeline2364.json', 'sprite': 2364,
   'clip_prefix': 'Mardek Soldier',
   'anchors_from_anim': f'{ANIMROOT}/Mardek/Mardek Soldier Idle.anim',
   'emit': MISSING_8,
   'mapping': {
     'Torso': ([23], None, None), 'Torso/Head': ([26], 23, None),
     'Right Shoulder': ([13], None, None), 'Left Shoulder': ([30], None, None),
     'Right Forearm': ([10], None, None), 'Left Forearm': ([40], None, None),
     'Right Thigh': ([17], None, None), 'Left Thigh': ([34], None, None),
     'Right Calf': ([21], None, None), 'Left Calf': ([38], None, None),
     'Right Foot': ([19], None, None), 'Left Foot': ([36], None, None),
     'Right Forearm/Weapon': ([3, 2], 10, '2261'),
     'Left Forearm/Shield': ([43], 40, '2289'),
   },
   'prefab_anchors': {
     'Right Foot': {'pos': (-14.5, 6.999996), 'theta_deg': -7.548, 'scale': (1, 1, 0)},
     'Left Foot': {'pos': (3.3, 4.1), 'theta_deg': 22.715, 'scale': (1, 1, 0)},
     'Torso/Head': {'pos': (-0.5, 11.6), 'theta_deg': -1.341, 'scale': (0.9, 0.9, 0)},
   },
 },
 'Emela': {
   'timeline': 'timeline2551.json', 'sprite': 2551,
   'clip_prefix': 'Emela',
   'anchors_from_anim': f'{ANIMROOT}/Emela/Emela_Idle.anim',
   'emit': MISSING_8,
   'mapping': {
     'Torso': ([19], None, None), 'Torso/Head': ([21], 19, None),
     'Right Shoulder': ([9], None, None), 'Left Shoulder': ([25], None, None),
     'Right Forearm': ([7], None, None), 'Left Forearm': ([35], None, None),
     'Right Thigh': ([12], None, None), 'Left Thigh': ([28], None, None),
     'Right Calf': ([16], None, None), 'Left Calf': ([32], None, None),
     'Right Foot': ([14], None, None), 'Left Foot': ([30], None, None),
     'Right Forearm/Weapon': ([3, 2], 7, '2509'),
     'Left Forearm/Shield': ([37], 35, '2289'),
   },
   'prefab_anchors': {
     'Right Foot': {'pos': (-10.7, 8.2), 'theta_deg': -7.548, 'scale': (1, 1, 0)},
     'Left Foot': {'pos': (9.5, 5.9), 'theta_deg': 22.715, 'scale': (1, 1, 0)},
     'Torso/Head': {'pos': (1.0, 11.0), 'theta_deg': -8.1, 'scale': (1, 1, 0)},
   },
 },
 'Deugan': {
   'timeline': 'timeline2427.json', 'sprite': 2427,
   'clip_prefix': 'Deugan',
   'anchors_from_anim': f'{ANIMROOT}/Deugan/Deugan Idle.anim',
   'emit': MISSING_8,
   'mapping': {
     # depth 5 is Deugan's cape - no Unity bone, not converted
     'Torso': ([21], None, None), 'Torso/Head': ([24], 21, None),
     'Right Shoulder': ([13], None, None), 'Left Shoulder': ([28], None, None),
     'Right Forearm': ([11], None, None), 'Left Forearm': ([36], None, None),
     'Right Thigh': ([15], None, None), 'Left Thigh': ([30], None, None),
     'Right Calf': ([19], None, None), 'Left Calf': ([34], None, None),
     'Right Foot': ([17], None, None), 'Left Foot': ([32], None, None),
     'Right Forearm/Weapon': ([3, 38], 11, '2367'),
     'Left Forearm/Shield': ([], 36, None),   # Deugan has no shield in Flash
   },
   'prefab_anchors': {
     'Right Foot': {'pos': (-13.6, 6.5), 'theta_deg': -7.548, 'scale': (1, 1, 0)},
     'Left Foot': {'pos': (4.2, 3.6), 'theta_deg': 22.715, 'scale': (1, 1, 0)},
     'Torso/Head': {'pos': (-0.5, 11.6), 'theta_deg': -1.341, 'scale': (0.9, 0.9, 0)},
   },
 },
 'Vehrn': {
   'timeline': 'timeline2651.json', 'sprite': 2651,
   'clip_prefix': 'Vehrn',
   'anchors_from_anim': None,   # no hand-made clips: anchor everything to the prefab
   'emit': ALL_SLOTS,
   'mapping': {
     'Torso': ([23], None, None), 'Torso/Head': ([26], 23, None),
     'Right Shoulder': ([14], None, None), 'Left Shoulder': ([30], None, None),
     'Right Forearm': ([11], None, None), 'Left Forearm': ([39], None, None),
     'Right Thigh': ([16], None, None), 'Left Thigh': ([32], None, None),
     'Right Calf': ([20], None, None), 'Left Calf': ([36], None, None),
     'Right Foot': ([18], None, None), 'Left Foot': ([34], None, None),
     'Right Forearm/Weapon': ([3, 2], 11, '2261'),
     'Left Forearm/Shield': ([42], 39, '2289'),
   },
   'prefab_anchors': {
     'Torso': {'pos': (-3.3, 47.0), 'theta_deg': 0.0, 'scale': (1, 1, 0)},
     'Torso/Head': {'pos': (-0.5, 11.6), 'theta_deg': -1.341, 'scale': (0.9, 0.9, 0)},
     'Right Shoulder': {'pos': (-2.3, 45.1), 'theta_deg': 71.36, 'scale': (-1, 1, 0)},
     'Left Shoulder': {'pos': (8.6, 45.9), 'theta_deg': -55.63, 'scale': (1, 1, 0)},
     'Right Forearm': {'pos': (-3.4, 36.4), 'theta_deg': 33.59, 'scale': (-1, 1, 0)},
     'Left Forearm': {'pos': (14.5, 36.92), 'theta_deg': -78.87, 'scale': (1, 1, 0)},
     'Right Thigh': {'pos': (-0.9, 33.7), 'theta_deg': -34.2, 'scale': (0.926, 1.087, 0)},
     'Left Thigh': {'pos': (1.0, 32.2), 'theta_deg': 8.72, 'scale': (1.222, 0.852, 0)},
     'Right Calf': {'pos': (-9.0, 21.3), 'theta_deg': -7.548, 'scale': (1, 1, 0)},
     'Left Calf': {'pos': (2.2, 19.9), 'theta_deg': 25.18, 'scale': (0.978, 1.023, 0)},
     'Right Foot': {'pos': (-14.5, 7.0), 'theta_deg': -7.548, 'scale': (1, 1, 0)},
     'Left Foot': {'pos': (3.3, 4.1), 'theta_deg': 22.715, 'scale': (1, 1, 0)},
     'Right Forearm/Weapon': {'pos': (10.7, 0.8), 'theta_deg': 0.0, 'scale': (1, 1, 0)},
     'Left Forearm/Shield': {'pos': (12.98, -7.68), 'theta_deg': -57.65, 'scale': (0.998, 0.84, 0.162)},
   },
 },
}

SLOT_NAMES = {'idle': 'Idle', 'moveto': 'MoveTo', 'strike': 'Strike', 'jumpback': 'JumpBack',
              'hit': 'Hurt', 'die': 'Die', 'dead': 'Dead', 'spellcast': 'Spellcast',
              'useitem': 'UseItem', 'victory': 'Victory'}

# keyframe reduction tolerances (max deviation from the dense 30fps bake)
TOL_ROT_DEG = 0.6
TOL_POS_PX = 0.12
TOL_SCALE = 0.008


# ---------------------------------------------------------------------------
# section derivation from frame labels + DoAction bytecode
# ---------------------------------------------------------------------------
def derive_sections(labels, actions, nframes):
    """Section rules (validated against Mardek/Deugan/Emela/Vehrn sprites):
    - a section starts at its frame label
    - it ends the frame before an Animate("idle") callback, or at the next
      label, whichever comes first
    - 'die' runs through the 'dead' label frame so it lands on the dead pose
    - 'dead' is a single frozen frame (the timeline stop()s there)
    - 'victory' ends ON a bare stop() action frame and holds
    - 'idle' and 'moveto' are loops (a wrap key is appended)
    """
    idle_returns = sorted(f for f, s in actions if 'Animate' in s and 'idle' in s)
    stops = sorted(f for f, s in actions if s == [])
    ordered = sorted(labels.items(), key=lambda kv: kv[1])
    sections = {}
    for i, (name, start) in enumerate(ordered):
        nxt = ordered[i + 1][1] if i + 1 < len(ordered) else nframes + 1
        end = nxt - 1
        for f in idle_returns:
            if start < f <= end + 1:
                end = f - 1
                break
        if name == 'die' and 'dead' in labels:
            end = labels['dead']            # include the dead pose
        elif name == 'dead':
            sections[name] = {'frames': [start, start], 'times': [0.0, 1.0]}
            continue
        elif name == 'victory':
            for f in stops:
                if f > start:
                    end = min(end + 1, f)   # stop frame itself displays and holds
                    break
        fr = list(range(start, end + 1))
        if name in ('idle', 'moveto'):
            fr = fr + [start]               # wrap key for clean looping
            sections[name] = {'frames': fr, 'loop_wrap': True}
        else:
            sections[name] = {'frames': fr}
    return sections


# ---------------------------------------------------------------------------
# anchors
# ---------------------------------------------------------------------------
def parse_idle_anchors(path):
    """t=0 rotation/position/scale per bone path from an existing .anim."""
    txt = open(path, encoding='utf-8').read()
    def section(a, b):
        return txt[txt.index(a):txt.index(b)]
    def first_keys(block, has_w):
        out = {}
        for part in re.split(r'\n  - curve:', block)[1:]:
            p = re.search(r'\n    path: (.*)', part).group(1).strip()
            m = re.search(r'value: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)(?:, w: ([-\d.e]+))?\}', part)
            out[p] = [float(v) for v in m.groups() if v is not None]
        return out
    rot = first_keys(section('m_RotationCurves:', 'm_CompressedRotationCurves:'), True)
    pos = first_keys(section('m_PositionCurves:', 'm_ScaleCurves:'), False)
    scl = first_keys(section('m_ScaleCurves:', 'm_FloatCurves:'), False)
    anchors = {}
    for p in rot:
        theta = 2 * math.atan2(rot[p][2], rot[p][3])
        anchors[p] = {'pos': tuple(pos[p][:2]), 'theta': theta, 'scale': tuple(scl[p])}
    return anchors


# ---------------------------------------------------------------------------
# per-bone component series
# ---------------------------------------------------------------------------
def build_series(frames, mapping):
    nframes = len(frames)
    ser = {}
    for path, (depths, parent, char) in mapping.items():
        comps = [None] * (nframes + 1)
        prev = None
        for f in range(1, nframes + 1):
            st = frames[f - 1]
            m = None
            for d in depths:
                cand = st.get(str(d))
                if cand is not None and (char is None or cand['charId'] == char):
                    m = cand
                    break
            if m is None:
                continue
            if parent is not None:
                p = st.get(str(parent))
                if p is None:
                    continue
                m = mat_mul(mat_inv(p), m)
            t, sx, sy = decompose(unity_linear(m), prev)
            prev = (t, sx, sy)
            px, py = unity_pos(m)
            comps[f] = (t, sx, sy, px, py)
        ser[path] = comps
    return ser


def norm_pi(a):
    while a > math.pi: a -= 2 * math.pi
    while a < -math.pi: a += 2 * math.pi
    return a


def build_anchors(cfg, ser, idle_frame):
    idle_anchors = parse_idle_anchors(cfg['anchors_from_anim']) if cfg['anchors_from_anim'] else {}
    anchors = {}
    for path in cfg['mapping']:
        ref = ser[path][idle_frame]
        static = ref is None                      # bone with no Flash data at all
        if static:
            ref = (0.0, 1.0, 1.0, 0.0, 0.0)
        t_ref, sx_ref, sy_ref, px_ref, py_ref = ref
        if path in idle_anchors and cfg['anchors_from_anim']:
            a = idle_anchors[path]
            th, pos, sc = a['theta'], a['pos'], a['scale']
        else:
            a = cfg['prefab_anchors'][path]
            th, pos, sc = math.radians(a['theta_deg']), a['pos'], a['scale']
        anchors[path] = {
            'static': static,
            'dtheta': norm_pi(th - t_ref),
            'dpos': (pos[0] - px_ref, pos[1] - py_ref),
            'srx': sc[0] / sx_ref, 'sry': sc[1] / sy_ref, 'sz': sc[2],
            'theta0': th, 'pos0': pos, 'scale0': sc,
        }
    return anchors


def make_bone_at(cfg, ser, anchors):
    nframes = len(ser[next(iter(ser))]) - 1
    hideable = {p for p, args in cfg['mapping'].items() if args[2] is not None}
    def bone_at(path, f):
        a = anchors[path]
        if a['static']:
            return {'theta': a['theta0'], 'px': a['pos0'][0], 'py': a['pos0'][1],
                    'sx': a['scale0'][0], 'sy': a['scale0'][1], 'sz': a['scale0'][2]}
        s = ser[path]
        hidden = s[f] is None
        c, ff, step = s[f], f, -1
        while c is None:
            ff += step
            if ff < 1:
                ff, step = f + 1, 1
            if ff > nframes:
                raise RuntimeError(f"no data for {path}")
            c = s[ff]
        if hidden and path not in hideable:
            raise RuntimeError(f"unexpected gap for {path} at frame {f}")
        t, sx, sy, px, py = c
        out = {'theta': t + a['dtheta'],
               'sx': sx * a['srx'], 'sy': sy * a['sry'], 'sz': a['sz'],
               'px': px + a['dpos'][0], 'py': py + a['dpos'][1]}
        if hidden:
            out['sx'] = out['sy'] = out['sz'] = 0.0
        return out
    return bone_at


# ---------------------------------------------------------------------------
# keyframe reduction
# ---------------------------------------------------------------------------
def hermite(t, t0, v0, m0, t1, v1, m1):
    dt = t1 - t0
    if dt <= 0:
        return v0
    u = (t - t0) / dt
    u2, u3 = u * u, u ** 3
    return ((2*u3 - 3*u2 + 1) * v0 + (u3 - 2*u2 + u) * dt * m0
            + (-2*u3 + 3*u2) * v1 + (u3 - u2) * dt * m1)

def eval_reduced(times, vals, keep, t):
    ks = keep
    def slope(j):
        if len(ks) == 1:
            return 0.0
        if j == 0:
            a, b = ks[0], ks[1]
        elif j == len(ks) - 1:
            a, b = ks[-2], ks[-1]
        else:
            a, b = ks[j-1], ks[j+1]
        return (vals[b] - vals[a]) / (times[b] - times[a])
    for j in range(len(ks) - 1):
        if times[ks[j]] <= t <= times[ks[j+1]]:
            return hermite(t, times[ks[j]], vals[ks[j]], slope(j),
                           times[ks[j+1]], vals[ks[j+1]], slope(j+1))
    return vals[ks[-1]]

def reduce_keys(times, channels, err_fn, tol):
    n = len(times)
    if n <= 2:
        return list(range(n))
    keep = [0, n - 1]
    while True:
        worst_i, worst_e = -1, tol
        for i in range(n):
            approx = [eval_reduced(times, ch, keep, times[i]) for ch in channels]
            e = err_fn(i, approx)
            if e > worst_e:
                worst_i, worst_e = i, e
        if worst_i < 0:
            return sorted(keep)
        keep.append(worst_i)
        keep.sort()

def reduce_quat(times, quats):
    zs = [q[2] for q in quats]
    ws = [q[3] for q in quats]
    def err(i, approx):
        z, w = approx
        norm = math.hypot(z, w)
        if norm < 1e-6:
            return math.inf
        dot = max(-1.0, min(1.0, (z * zs[i] + w * ws[i]) / norm))
        return 2 * math.acos(abs(dot))
    return reduce_keys(times, [zs, ws], err, math.radians(TOL_ROT_DEG))

def reduce_vec(times, vecs, tol):
    chans = [[v[k] for v in vecs] for k in range(len(vecs[0]))]
    def err(i, approx):
        return max(abs(approx[k] - chans[k][i]) for k in range(len(chans)))
    return reduce_keys(times, chans, err, tol)

def pick(seq, idx):
    return [seq[i] for i in idx]


# ---------------------------------------------------------------------------
# YAML emission
# ---------------------------------------------------------------------------
def ff(v):
    if v == int(v) and abs(v) < 1e7:
        return str(int(v))
    return repr(round(v, 8))

def slopes(times, vals):
    n = len(vals)
    out = []
    for i in range(n):
        if n == 1:
            out.append(0.0)
        elif i == 0:
            out.append((vals[1] - vals[0]) / (times[1] - times[0]))
        elif i == n - 1:
            out.append((vals[i] - vals[i-1]) / (times[i] - times[i-1]))
        else:
            out.append((vals[i+1] - vals[i-1]) / (times[i+1] - times[i-1]))
    return out

def vec_curve_yaml(times, vecs, path, has_w):
    comps = list(zip(*vecs))
    sl = [slopes(times, list(c)) for c in comps]
    names = 'xyzw' if has_w else 'xyz'
    L = ['  - curve:', '      serializedVersion: 2', '      m_Curve:']
    for i, t in enumerate(times):
        val = ', '.join(f'{n}: {ff(comps[j][i])}' for j, n in enumerate(names))
        slp = ', '.join(f'{n}: {ff(sl[j][i])}' for j, n in enumerate(names))
        wgt = ', '.join(f'{n}: 0.33333334' for n in names)
        L += ['      - serializedVersion: 3', f'        time: {ff(t)}',
              f'        value: {{{val}}}', f'        inSlope: {{{slp}}}',
              f'        outSlope: {{{slp}}}', '        tangentMode: 0',
              '        weightedMode: 0', f'        inWeight: {{{wgt}}}',
              f'        outWeight: {{{wgt}}}']
    L += ['      m_PreInfinity: 2', '      m_PostInfinity: 2',
          '      m_RotationOrder: 4', f'    path: {path}']
    return L

def float_curve_yaml(times, vals, attribute, path, tangent=136):
    sl = slopes(times, vals)
    L = ['  - serializedVersion: 2', '    curve:', '      serializedVersion: 2', '      m_Curve:']
    for i, t in enumerate(times):
        L += ['      - serializedVersion: 3', f'        time: {ff(t)}',
              f'        value: {ff(vals[i])}', f'        inSlope: {ff(sl[i])}',
              f'        outSlope: {ff(sl[i])}', f'        tangentMode: {tangent}',
              '        weightedMode: 0', '        inWeight: 0.33333334',
              '        outWeight: 0.33333334']
    L += ['      m_PreInfinity: 2', '      m_PostInfinity: 2', '      m_RotationOrder: 4',
          f'    attribute: {attribute}', f'    path: {path}', '    classID: 4',
          '    script: {fileID: 0}', '    flags: 0']
    return L


def build_clip(clip_name, spec, bone_at, bone_order):
    fr = spec['frames']
    times = spec.get('times') or [i / FPS for i in range(len(fr))]
    if spec.get('times') or spec.get('loop_wrap'):
        stop = times[-1]
    else:
        stop = len(fr) / FPS
    data = {p: [bone_at(p, f) for f in fr] for p in bone_order}

    rot_blocks, pos_blocks, scl_blocks, ed_blocks, eu_blocks = [], [], [], [], []
    dense_total = reduced_total = 0
    for p in bone_order:
        d = data[p]
        thetas = [b['theta'] for b in d]
        quats = [(0.0, 0.0, math.sin(t/2), math.cos(t/2)) for t in thetas]
        poss = [(b['px'], b['py'], 0.0) for b in d]
        scls = [(b['sx'], b['sy'], b['sz']) for b in d]

        ri = reduce_quat(times, quats)
        pi = reduce_vec(times, poss, TOL_POS_PX)
        si = reduce_vec(times, scls, TOL_SCALE)
        dense_total += 3 * len(times)
        reduced_total += len(ri) + len(pi) + len(si)

        rot_blocks += vec_curve_yaml(pick(times, ri), pick(quats, ri), p, True)
        pos_blocks += vec_curve_yaml(pick(times, pi), pick(poss, pi), p, False)
        scl_blocks += vec_curve_yaml(pick(times, si), pick(scls, si), p, False)
        for attr, vals, idx in [('m_LocalPosition.x', [v[0] for v in poss], pi),
                                ('m_LocalPosition.y', [v[1] for v in poss], pi),
                                ('m_LocalPosition.z', [v[2] for v in poss], pi),
                                ('m_LocalScale.x', [v[0] for v in scls], si),
                                ('m_LocalScale.y', [v[1] for v in scls], si),
                                ('m_LocalScale.z', [v[2] for v in scls], si)]:
            ed_blocks += float_curve_yaml(pick(times, idx), pick(vals, idx), attr, p)
        degs = [math.degrees(t) for t in thetas]
        two = [0, len(times) - 1] if len(times) > 1 else [0]
        for attr, vals, idx in [('localEulerAnglesBaked.x', [0.0]*len(times), two),
                                ('localEulerAnglesBaked.y', [0.0]*len(times), two),
                                ('localEulerAnglesBaked.z', degs, ri)]:
            eu_blocks += float_curve_yaml(pick(times, idx), pick(vals, idx), attr, p)

    L = ['%YAML 1.1', '%TAG !u! tag:unity3d.com,2011:', '--- !u!74 &7400000', 'AnimationClip:',
         '  m_ObjectHideFlags: 0', '  m_CorrespondingSourceObject: {fileID: 0}',
         '  m_PrefabInstance: {fileID: 0}', '  m_PrefabAsset: {fileID: 0}',
         f'  m_Name: {clip_name}', '  serializedVersion: 7', '  m_Legacy: 1',
         '  m_Compressed: 0', '  m_UseHighQualityCurve: 1', '  m_RotationCurves:']
    L += rot_blocks
    L += ['  m_CompressedRotationCurves: []', '  m_EulerCurves: []', '  m_PositionCurves:']
    L += pos_blocks
    L += ['  m_ScaleCurves:']
    L += scl_blocks
    L += ['  m_FloatCurves: []', '  m_PPtrCurves: []', '  m_SampleRate: 60', '  m_WrapMode: 0',
          '  m_Bounds:', '    m_Center: {x: 0, y: 0, z: 0}', '    m_Extent: {x: 0, y: 0, z: 0}',
          '  m_ClipBindingConstant:', '    genericBindings: []', '    pptrCurveMapping: []',
          '  m_AnimationClipSettings:', '    serializedVersion: 2',
          '    m_AdditiveReferencePoseClip: {fileID: 0}', '    m_AdditiveReferencePoseTime: 0',
          '    m_StartTime: 0', f'    m_StopTime: {ff(stop)}', '    m_OrientationOffsetY: 0',
          '    m_Level: 0', '    m_CycleOffset: 0', '    m_HasAdditiveReferencePose: 0',
          '    m_LoopTime: 1', '    m_LoopBlend: 0', '    m_LoopBlendOrientation: 0',
          '    m_LoopBlendPositionY: 0', '    m_LoopBlendPositionXZ: 0',
          '    m_KeepOriginalOrientation: 0', '    m_KeepOriginalPositionY: 1',
          '    m_KeepOriginalPositionXZ: 0', '    m_HeightFromFeet: 0', '    m_Mirror: 0',
          '  m_EditorCurves:']
    L += ed_blocks
    L += ['  m_EulerEditorCurves:']
    L += eu_blocks
    L += ['  m_HasGenericRootTransform: 0', '  m_HasMotionFloatCurves: 0', '  m_Events: []', '']
    return '\n'.join(L), dense_total, reduced_total


def convert(char, outdir):
    cfg = CONFIG[char]
    tl = json.load(open(cfg['timeline']))
    frames, labels = tl['frames'], tl['labels']
    actions = [(f, s) for f, s in tl['actions']]
    sections = derive_sections(labels, actions, len(frames))
    idle_frame = labels['idle']

    ser = build_series(frames, cfg['mapping'])
    anchors = build_anchors(cfg, ser, idle_frame)
    bone_at = make_bone_at(cfg, ser, anchors)
    bone_order = sorted(cfg['mapping'])

    os.makedirs(outdir, exist_ok=True)
    for slot in cfg['emit']:
        spec = sections[slot]
        clip_name = f"{cfg['clip_prefix']} {SLOT_NAMES[slot]}"
        text, dense, red = build_clip(clip_name, spec, bone_at, bone_order)
        with open(os.path.join(outdir, clip_name + '.anim'), 'w', encoding='utf-8', newline='\n') as f:
            f.write(text)
        dur = spec['times'][-1] if spec.get('times') else (
            spec['frames'][-1] is not None and (len(spec['frames']) - (1 if not spec.get('loop_wrap') else 1)) / FPS)
        nfr = len(spec['frames'])
        print(f"  {clip_name}.anim  frames={nfr} keys {dense}->{red} ({100*red/max(dense,1):.0f}%)")


if __name__ == '__main__':
    if '--list' in sys.argv:
        for c in CONFIG:
            print(c)
        sys.exit(0)
    char = sys.argv[1]
    outdir = sys.argv[2] if len(sys.argv) > 2 else f'out_{char}'
    print(f"== {char} ==")
    convert(char, outdir)
