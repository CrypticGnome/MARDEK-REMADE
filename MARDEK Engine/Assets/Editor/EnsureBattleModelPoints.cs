using System.Linq;
using UnityEditor;
using UnityEngine;
using MARDEK.Battle;

// Ensures every battle-model prefab has a "Receive Melee Strike Point" and a "Strike Point"
// child transform wired into its BattleModelAnimator, matching the convention set by
// "mardek soldier" (both are plain Transform-only GameObjects parented directly under the
// model's root transform).
//
// - Any existing GameObject named "Hit Point" is renamed to "Receive Melee Strike Point"
//   (same object, same wiring - this is a rename only).
// - Prefabs missing either point get both created from scratch, positioned from the
//   character's sprite bounds: Strike Point at the vertical-center/left edge of the
//   bounding box, Receive Melee Strike Point at the bounds center nudged slightly toward
//   that same side (the "front" of the torso, mirroring the small offset already used by
//   "mardek soldier").
public static class EnsureBattleModelPoints
{
     const string PrefabRoot = "Assets/Prefabs/Battle Models";
     const string OldHitPointName = "Hit Point";
     const string HitPointName = "Receive Melee Strike Point";
     const string StrikePointName = "Strike Point";

     [MenuItem("Custom/Ensure Hit And Strike Points On Battle Models")]
     static void EnsureAll()
     {
          string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabRoot });
          int renamed = 0, created = 0, skipped = 0, unchanged = 0;
          try
          {
               for (int i = 0; i < guids.Length; i++)
               {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    EditorUtility.DisplayProgressBar("Ensuring hit/strike points", path, (float)i / guids.Length);
                    switch (EnsureOnPrefab(path))
                    {
                         case Result.Created: created++; break;
                         case Result.RenamedOnly: renamed++; break;
                         case Result.Unchanged: unchanged++; break;
                         default: skipped++; break;
                    }
               }
          }
          finally
          {
               EditorUtility.ClearProgressBar();
          }
          Debug.Log($"Ensured battle model points: {created} prefab(s) got new points, {renamed} renamed only, {unchanged} already correct, {skipped} skipped (no BattleModelAnimator)");
     }

     enum Result { Created, RenamedOnly, Unchanged, Skipped }

     static Result EnsureOnPrefab(string path)
     {
          GameObject root = PrefabUtility.LoadPrefabContents(path);
          try
          {
               var model = root.GetComponentInChildren<BattleModelAnimator>();
               if (model == null)
                    return Result.Skipped;

               bool changed = false;

               // Rename any legacy "Hit Point" object in place - same transform, same wiring.
               var legacyHitPoint = root.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.gameObject.name == OldHitPointName);
               if (legacyHitPoint != null)
               {
                    legacyHitPoint.gameObject.name = HitPointName;
                    changed = true;
               }

               var so = new SerializedObject(model);
               var strikePointProp = so.FindProperty("strikePoint");
               var hitPointProp = so.FindProperty("hitPoint");
               var parentProp = so.FindProperty("transform");

               bool needsStrikePoint = strikePointProp.objectReferenceValue == null;
               bool needsHitPoint = hitPointProp.objectReferenceValue == null;

               if (!needsStrikePoint && !needsHitPoint)
               {
                    if (!changed)
                         return Result.Unchanged;

                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    Debug.Log($"Renamed '{OldHitPointName}' to '{HitPointName}' in {path}");
                    return Result.RenamedOnly;
               }

               Transform parent = parentProp.objectReferenceValue as Transform ?? model.transform;

               var renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
               if (renderers.Length == 0)
               {
                    Debug.LogWarning($"{path}: no SpriteRenderers found, cannot compute bounds - skipping point creation");
                    return changed ? Result.RenamedOnly : Result.Skipped;
               }

               Bounds bounds = renderers[0].bounds;
               for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

               Vector3 centerLocal = parent.InverseTransformPoint(bounds.center);
               Vector3 minLocal = parent.InverseTransformPoint(new Vector3(bounds.min.x, bounds.center.y, bounds.center.z));

               // "Slightly in front" of centre of mass: a small nudge toward the same side
               // as the Strike Point (the torso's facing side), mirroring the ~3% of
               // half-width offset already used on "mardek soldier".
               float forwardOffset = Mathf.Sign(minLocal.x - centerLocal.x) * bounds.extents.x * 0.03f;
               Vector3 hitPointLocal = centerLocal + new Vector3(forwardOffset, 0, 0);

               if (needsStrikePoint)
               {
                    var go = new GameObject(StrikePointName);
                    go.transform.SetParent(parent, false);
                    go.transform.localPosition = minLocal;
                    strikePointProp.objectReferenceValue = go.transform;
                    changed = true;
               }

               if (needsHitPoint)
               {
                    var go = new GameObject(HitPointName);
                    go.transform.SetParent(parent, false);
                    go.transform.localPosition = hitPointLocal;
                    hitPointProp.objectReferenceValue = go.transform;
                    changed = true;
               }

               so.ApplyModifiedPropertiesWithoutUndo();

               PrefabUtility.SaveAsPrefabAsset(root, path);
               Debug.Log($"Created missing hit/strike point(s) on {path}");
               return Result.Created;
          }
          finally
          {
               PrefabUtility.UnloadPrefabContents(root);
          }
     }
}
