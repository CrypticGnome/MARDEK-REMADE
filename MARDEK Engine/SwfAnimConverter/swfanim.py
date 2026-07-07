"""Shared conversion helpers: Flash matrix -> Unity transform components."""
import math

TWIP = 20.0

def unity_linear(m):
    """Flash linear part -> Unity linear part (y-flip conjugation)."""
    # Flash: x' = a*x + b*y ; y' = c*x + d*y   (a=scaleX, b=rotateSkew1, c=rotateSkew0, d=scaleY)
    # Unity basis: (x, -y) => U = F conjugated by diag(1,-1)
    return (m['a'], -m['b'], -m['c'], m['d'])  # U00, U01, U10, U11

def unity_pos(m):
    return (m['tx'] / TWIP, -m['ty'] / TWIP)

def decompose(U, prev=None):
    """U = (U00,U01,U10,U11) -> (theta_rad, sx, sy) with R(theta)*S convention.
    col1 = (-sy sin, sy cos); col0 = (sx cos, sx sin). sy kept positive unless
    continuity with prev (theta, sx, sy) prefers the flipped branch."""
    U00, U01, U10, U11 = U
    sy = math.hypot(U01, U11)
    if sy < 1e-9:
        theta = math.atan2(U10, U00)
        sx = math.hypot(U00, U10)
        return (theta, sx, 0.0)
    theta = math.atan2(-U01, U11)
    sx = U00 * math.cos(theta) + U10 * math.sin(theta)
    cands = [(theta, sx, sy), (theta + math.pi, -sx, -sy)]
    best = None
    for (t, x, y) in cands:
        if prev is not None:
            # unwrap t near prev theta
            while t - prev[0] > math.pi: t -= 2 * math.pi
            while t - prev[0] < -math.pi: t += 2 * math.pi
            cost = abs(t - prev[0])
        else:
            cost = 0 if y >= 0 else 1e9
        if best is None or cost < best[0]:
            best = (cost, (t, x, y))
    return best[1]

def mat_mul(A, B):
    """affine 2x3 as dict a,b,c,d,tx,ty (flash-layout: x'=a x + b y + tx; y'=c x + d y + ty)"""
    return {
        'a': A['a']*B['a'] + A['b']*B['c'],
        'b': A['a']*B['b'] + A['b']*B['d'],
        'c': A['c']*B['a'] + A['d']*B['c'],
        'd': A['c']*B['b'] + A['d']*B['d'],
        'tx': A['a']*B['tx'] + A['b']*B['ty'] + A['tx'],
        'ty': A['c']*B['tx'] + A['d']*B['ty'] + A['ty'],
    }

def mat_inv(A):
    det = A['a']*A['d'] - A['b']*A['c']
    ia, ib, ic, idd = A['d']/det, -A['b']/det, -A['c']/det, A['a']/det
    return {'a': ia, 'b': ib, 'c': ic, 'd': idd,
            'tx': -(ia*A['tx'] + ib*A['ty']), 'ty': -(ic*A['tx'] + idd*A['ty'])}

def lerp_mat(m0, m1, t):
    return {k: m0[k] + (m1[k] - m0[k]) * t for k in ('a','b','c','d','tx','ty')}

def sample_series(frames, depth, f_start, f_count, nkeys, child_of=None):
    """Sample nkeys evenly over [f_start, f_start+f_count] (1-based frames, inclusive
    start; position f_start + u*f_count). Linear interp of raw matrices between frames.
    Returns list of flash matrices; if child_of given (another depth), returns local
    matrix inv(parent)*child."""
    out = []
    last = len(frames)
    for k in range(nkeys):
        u = k / (nkeys - 1) if nkeys > 1 else 0.0
        fpos = f_start + u * f_count
        f0 = int(math.floor(fpos))
        t = fpos - f0
        f1 = min(f0 + 1, last)
        f0 = min(f0, last)
        def get(fi, d):
            st = frames[fi - 1]
            return st.get(str(d))
        def mat_at(d):
            m0, m1 = get(f0, d), get(f1, d)
            if m0 is None: return None
            if m1 is None or t == 0: return dict(m0)
            return lerp_mat(m0, m1, t)
        m = mat_at(depth)
        if m is None:
            out.append(None)
            continue
        if child_of is not None:
            p = mat_at(child_of)
            m = mat_mul(mat_inv(p), m)
        out.append(m)
    return out

def to_components(mats):
    """List of flash mats -> list of (theta, sx, sy, px, py) with angle continuity."""
    comps = []
    prev = None
    for m in mats:
        if m is None:
            comps.append(None)
            continue
        U = unity_linear(m)
        t, sx, sy = decompose(U, prev)
        prev = (t, sx, sy)
        px, py = unity_pos(m)
        comps.append((t, sx, sy, px, py))
    return comps
