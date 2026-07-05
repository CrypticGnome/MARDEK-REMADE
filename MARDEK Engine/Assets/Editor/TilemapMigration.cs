using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

// Workaround for Unity 6000.1+/6000.4 scrambling tilemap data saved by 6000.0 when it
// upgrades the serialized format on load.
//
// Usage:
//   1. In 6000.0.18f1 (where tilemaps load correctly): Custom > Tilemap Migration > 1 - Export All Tilemaps
//      Writes JSON to <project root>/TilemapExport/.
//   2. Upgrade Unity (the maps will look scrambled - that's expected, ignore it).
//   3. In the new version: Custom > Tilemap Migration > 2 - Restore All Tilemaps
//      Clears every tilemap and repaints it from the JSON, so the new editor serializes
//      the data natively instead of upgrading the old format.
public static class TilemapMigration
{
     static string ExportRoot => Path.Combine(Path.GetDirectoryName(Application.dataPath), "TilemapExport");

     [System.Serializable]
     class CellData
     {
          public int x, y, z;
          public string tileId;
          public float[] matrix; // null when identity
          public float[] color;  // null when white
     }

     [System.Serializable]
     class TilemapEntry
     {
          public string indexPath; // sibling-index path from root, e.g. "2/0/1"
          public string namePath;  // human-readable, for logs only
          public List<CellData> cells = new List<CellData>();
     }

     [System.Serializable]
     class AssetEntry
     {
          public string assetPath;
          public bool isPrefab;
          public List<TilemapEntry> tilemaps = new List<TilemapEntry>();
     }

     [MenuItem("Custom/Tilemap Migration/1 - Export All Tilemaps")]
     static void ExportAll()
     {
          if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
               return;
          Directory.CreateDirectory(ExportRoot);
          int assetCount = 0, tilemapCount = 0, cellCount = 0;
          try
          {
               foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
               {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab == null || prefab.GetComponentInChildren<Tilemap>(true) == null)
                         continue;
                    EditorUtility.DisplayProgressBar("Exporting tilemaps", path, 0.5f);
                    var entry = new AssetEntry { assetPath = path, isPrefab = true };
                    foreach (var tm in prefab.GetComponentsInChildren<Tilemap>(true))
                         entry.tilemaps.Add(ExportTilemap(tm, prefab.transform, ref cellCount));
                    WriteEntry(entry);
                    assetCount++;
                    tilemapCount += entry.tilemaps.Count;
               }

               foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }))
               {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    EditorUtility.DisplayProgressBar("Exporting tilemaps", path, 0.5f);
                    var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                    var entry = new AssetEntry { assetPath = path, isPrefab = false };
                    foreach (var root in scene.GetRootGameObjects())
                    {
                         foreach (var tm in root.GetComponentsInChildren<Tilemap>(true))
                         {
                              if (PrefabUtility.IsPartOfPrefabInstance(tm))
                              {
                                   Debug.LogWarning($"Skipping prefab-instance tilemap in scene (handled via its prefab): {path} / {NamePath(tm.transform, null)}");
                                   continue;
                              }
                              entry.tilemaps.Add(ExportTilemap(tm, null, ref cellCount));
                         }
                    }
                    if (entry.tilemaps.Count > 0)
                    {
                         WriteEntry(entry);
                         assetCount++;
                         tilemapCount += entry.tilemaps.Count;
                    }
               }
          }
          finally
          {
               EditorUtility.ClearProgressBar();
          }
          Debug.Log($"Tilemap export complete: {tilemapCount} tilemaps / {cellCount} cells across {assetCount} assets -> {ExportRoot}");
     }

     [MenuItem("Custom/Tilemap Migration/2 - Restore All Tilemaps")]
     static void RestoreAll()
     {
          if (!Directory.Exists(ExportRoot))
          {
               Debug.LogError($"No export found at {ExportRoot}. Run the export in the old Unity version first.");
               return;
          }
          if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
               return;
          int assetCount = 0, tilemapCount = 0, missingTiles = 0;
          try
          {
               foreach (string file in Directory.GetFiles(ExportRoot, "*.json"))
               {
                    var entry = JsonUtility.FromJson<AssetEntry>(File.ReadAllText(file));
                    EditorUtility.DisplayProgressBar("Restoring tilemaps", entry.assetPath, 0.5f);
                    if (entry.isPrefab)
                    {
                         var root = PrefabUtility.LoadPrefabContents(entry.assetPath);
                         try
                         {
                              foreach (var tmEntry in entry.tilemaps)
                                   tilemapCount += RestoreTilemap(tmEntry, ResolveIndexPath(root.transform, tmEntry.indexPath), entry.assetPath, ref missingTiles);
                              PrefabUtility.SaveAsPrefabAsset(root, entry.assetPath);
                         }
                         finally
                         {
                              PrefabUtility.UnloadPrefabContents(root);
                         }
                    }
                    else
                    {
                         var scene = EditorSceneManager.OpenScene(entry.assetPath, OpenSceneMode.Single);
                         var roots = scene.GetRootGameObjects();
                         foreach (var tmEntry in entry.tilemaps)
                         {
                              var target = ResolveIndexPath(roots, tmEntry.indexPath);
                              tilemapCount += RestoreTilemap(tmEntry, target, entry.assetPath, ref missingTiles);
                         }
                         EditorSceneManager.MarkSceneDirty(scene);
                         EditorSceneManager.SaveScene(scene);
                    }
                    assetCount++;
               }
          }
          finally
          {
               EditorUtility.ClearProgressBar();
          }
          string missing = missingTiles > 0 ? $" WARNING: {missingTiles} cells referenced tile assets that could not be resolved." : "";
          Debug.Log($"Tilemap restore complete: {tilemapCount} tilemaps across {assetCount} assets.{missing}");
     }

     static TilemapEntry ExportTilemap(Tilemap tm, Transform relativeTo, ref int cellCount)
     {
          var entry = new TilemapEntry
          {
               indexPath = IndexPath(tm.transform, relativeTo),
               namePath = NamePath(tm.transform, relativeTo),
          };
          foreach (var pos in tm.cellBounds.allPositionsWithin)
          {
               if (!tm.HasTile(pos))
                    continue;
               var tile = tm.GetTile(pos);
               var cell = new CellData
               {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z,
                    tileId = GlobalObjectId.GetGlobalObjectIdSlow(tile).ToString(),
               };
               Matrix4x4 m = tm.GetTransformMatrix(pos);
               if (m != Matrix4x4.identity)
               {
                    cell.matrix = new float[16];
                    for (int i = 0; i < 16; i++)
                         cell.matrix[i] = m[i];
               }
               Color c = tm.GetColor(pos);
               if (c != Color.white)
                    cell.color = new[] { c.r, c.g, c.b, c.a };
               entry.cells.Add(cell);
               cellCount++;
          }
          return entry;
     }

     static int RestoreTilemap(TilemapEntry entry, Transform target, string assetPath, ref int missingTiles)
     {
          if (target == null)
          {
               Debug.LogError($"Could not find tilemap '{entry.namePath}' (index path {entry.indexPath}) in {assetPath} - hierarchy changed since export?");
               return 0;
          }
          var tm = target.GetComponent<Tilemap>();
          if (tm == null)
          {
               Debug.LogError($"Object at '{entry.namePath}' in {assetPath} has no Tilemap component.");
               return 0;
          }
          tm.ClearAllTiles();
          foreach (var cell in entry.cells)
          {
               if (!GlobalObjectId.TryParse(cell.tileId, out var id))
               {
                    missingTiles++;
                    continue;
               }
               var tile = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as TileBase;
               if (tile == null)
               {
                    missingTiles++;
                    continue;
               }
               var pos = new Vector3Int(cell.x, cell.y, cell.z);
               tm.SetTile(pos, tile);
               if (cell.matrix != null && cell.matrix.Length == 16)
               {
                    var m = new Matrix4x4();
                    for (int i = 0; i < 16; i++)
                         m[i] = cell.matrix[i];
                    tm.SetTransformMatrix(pos, m);
               }
               if (cell.color != null && cell.color.Length == 4)
               {
                    tm.SetTileFlags(pos, TileFlags.None);
                    tm.SetColor(pos, new Color(cell.color[0], cell.color[1], cell.color[2], cell.color[3]));
               }
          }
          EditorUtility.SetDirty(tm);
          return 1;
     }

     static string IndexPath(Transform t, Transform relativeTo)
     {
          var parts = new List<string>();
          while (t != null && t != relativeTo)
          {
               parts.Insert(0, t.GetSiblingIndex().ToString());
               t = t.parent;
          }
          return string.Join("/", parts);
     }

     static string NamePath(Transform t, Transform relativeTo)
     {
          var parts = new List<string>();
          while (t != null && t != relativeTo)
          {
               parts.Insert(0, t.name);
               t = t.parent;
          }
          return string.Join("/", parts);
     }

     static Transform ResolveIndexPath(Transform root, string indexPath)
     {
          if (string.IsNullOrEmpty(indexPath))
               return root;
          Transform current = root;
          foreach (string part in indexPath.Split('/'))
          {
               int index = int.Parse(part);
               if (index >= current.childCount)
                    return null;
               current = current.GetChild(index);
          }
          return current;
     }

     static Transform ResolveIndexPath(GameObject[] sceneRoots, string indexPath)
     {
          string[] parts = indexPath.Split('/');
          int rootIndex = int.Parse(parts[0]);
          if (rootIndex >= sceneRoots.Length)
               return null;
          Transform current = sceneRoots[rootIndex].transform;
          for (int i = 1; i < parts.Length; i++)
          {
               int index = int.Parse(parts[i]);
               if (index >= current.childCount)
                    return null;
               current = current.GetChild(index);
          }
          return current;
     }

     static void WriteEntry(AssetEntry entry)
     {
          string fileName = entry.assetPath.Replace('/', '_').Replace('\\', '_').Replace(':', '_') + ".json";
          File.WriteAllText(Path.Combine(ExportRoot, fileName), JsonUtility.ToJson(entry));
     }
}
