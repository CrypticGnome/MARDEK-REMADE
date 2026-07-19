using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MARDEK.Battle;

// Poses every battle-model prefab to the first frame of its assigned Idle animation.
//
// The rigs were hand-built and their saved bind poses drift a few pixels/degrees from
// where the Idle clip actually starts. Sampling the idle clip at t=0 and saving that
// pose back into the prefab makes the prefab in the editor match what the game shows,
// which matters when placing sprite pivots, Strike/Hit Points, etc.
//
// Bones the idle clip does not animate (e.g. feet/head on the hand-made clips) are
// left untouched.
public static class ApplyIdlePoseToPrefabs
{
     const string PrefabRoot = "Assets/Prefabs/Battle Models";

     [MenuItem("Custom/Apply Idle Pose To Battle Model Prefabs")]
     static void ApplyAll()
     {
          string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabRoot });
          int posed = 0, skipped = 0, unchanged = 0;
          try
          {
               for (int i = 0; i < guids.Length; i++)
               {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    EditorUtility.DisplayProgressBar("Applying idle poses", path, (float)i / guids.Length);
                    switch (ApplyToPrefab(path))
                    {
                         case Result.Posed: posed++; break;
                         case Result.Unchanged: unchanged++; break;
                         default: skipped++; break;
                    }
               }
          }
          finally
          {
               EditorUtility.ClearProgressBar();
          }
          Debug.Log($"Idle pose applied to {posed} prefab(s), {unchanged} already matched, {skipped} skipped (no model/idle clip)");
     }

     enum Result { Posed, Unchanged, Skipped }

     static Result ApplyToPrefab(string path)
     {
          GameObject root = PrefabUtility.LoadPrefabContents(path);
          try
          {
               var model = root.GetComponentInChildren<BattleModelAnimator>();
               if (model == null)
                    return Result.Skipped;

               // idle clip and Animation component are private serialized fields
               var so = new SerializedObject(model);
               var clip = so.FindProperty("idle").objectReferenceValue as AnimationClip;
               var anim = so.FindProperty("animation").objectReferenceValue as Animation;
               if (clip == null || anim == null)
               {
                    Debug.LogWarning($"{path}: no idle clip or Animation component assigned, skipping");
                    return Result.Skipped;
               }

               var before = SnapshotPose(anim.transform);
               clip.SampleAnimation(anim.gameObject, 0f);
               if (!PoseChanged(anim.transform, before))
                    return Result.Unchanged;

               PrefabUtility.SaveAsPrefabAsset(root, path);
               Debug.Log($"Applied first frame of '{clip.name}' to {path}");
               return Result.Posed;
          }
          finally
          {
               PrefabUtility.UnloadPrefabContents(root);
          }
     }

     static List<(Vector3 pos, Quaternion rot, Vector3 scale)> SnapshotPose(Transform troot)
     {
          var snapshot = new List<(Vector3, Quaternion, Vector3)>();
          foreach (var t in troot.GetComponentsInChildren<Transform>(true))
               snapshot.Add((t.localPosition, t.localRotation, t.localScale));
          return snapshot;
     }

     static bool PoseChanged(Transform troot, List<(Vector3 pos, Quaternion rot, Vector3 scale)> before)
     {
          int i = 0;
          foreach (var t in troot.GetComponentsInChildren<Transform>(true))
          {
               var (pos, rot, scale) = before[i++];
               if ((t.localPosition - pos).sqrMagnitude > 1e-10f) return true;
               if (Quaternion.Angle(t.localRotation, rot) > 0.001f) return true;
               if ((t.localScale - scale).sqrMagnitude > 1e-10f) return true;
          }
          return false;
     }
}
