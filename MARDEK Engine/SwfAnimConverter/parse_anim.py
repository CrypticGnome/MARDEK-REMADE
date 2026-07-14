"""Parse a Unity .anim (legacy, machine-generated) into JSON curve data."""
import re, json, sys

src, out = sys.argv[1], sys.argv[2]
txt = open(src, encoding='utf-8').read()

def section(name, nxt):
    a = txt.index(name)
    b = txt.index(nxt)
    return txt[a:b]

def parse_vec_curves(block):
    """Curves with vector values: value: {x: .., y: .., z: ..[, w: ..]}"""
    curves = {}
    # split on '  - curve:' or '  - serializedVersion: 2\n    curve:' boundaries
    parts = re.split(r'\n  - (?:curve:|serializedVersion: 2\n    curve:)', block)
    for p in parts[1:]:
        pm = re.search(r'\n    path: (.*)', p)
        if not pm:
            continue
        path = pm.group(1).strip()
        keys = []
        for km in re.finditer(
            r'time: ([-\d.e]+)\s+value: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)(?:, w: ([-\d.e]+))?\}', p):
            g = km.groups()
            keys.append([float(v) for v in g if v is not None])
        curves[path] = keys
    return curves

def parse_float_curves(block):
    """m_EditorCurves / m_EulerEditorCurves style: scalar values, attribute+path."""
    curves = {}
    parts = re.split(r'\n  - serializedVersion: 2\n    curve:', block)
    for p in parts[1:]:
        am = re.search(r'\n    attribute: (.*)', p)
        pm = re.search(r'\n    path: (.*)', p)
        if not (am and pm):
            continue
        key = (pm.group(1).strip(), am.group(1).strip())
        keys = []
        for km in re.finditer(r'time: ([-\d.e]+)\s+value: ([-\d.e]+)\s', p):
            keys.append([float(km.group(1)), float(km.group(2))])
        curves['|'.join(key)] = keys
    return curves

rot = parse_vec_curves(section('m_RotationCurves:', 'm_CompressedRotationCurves:'))
pos = parse_vec_curves(section('m_PositionCurves:', 'm_ScaleCurves:'))
scl = parse_vec_curves(section('m_ScaleCurves:', 'm_FloatCurves:'))
ed = parse_float_curves(section('m_EditorCurves:', 'm_EulerEditorCurves:'))
eu = parse_float_curves(section('m_EulerEditorCurves:', 'm_HasGenericRootTransform:'))

json.dump({'rot': rot, 'pos': pos, 'scale': scl, 'editor': ed, 'euler': eu}, open(out, 'w'))
print("rot paths:", sorted(rot))
print("pos paths:", sorted(pos))
print("scale paths:", sorted(scl))
print("euler attrs sample:", sorted(eu)[:6])
for p in sorted(pos):
    print(f"pos[{p}] nkeys={len(pos[p])} t0={pos[p][0]}")
for p in sorted(scl):
    print(f"scale[{p}] nkeys={len(scl[p])} t0={scl[p][0]}")
