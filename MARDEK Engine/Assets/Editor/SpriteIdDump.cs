using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Dumps the (texture GUID, sprite fileID, sprite name) of every sprite in the project
// to <project root>/SpriteIdDump/<unity version>.csv.
//
// Run this once in 6000.0 and once in the upgraded version. Comparing the two files
// shows whether sprite fileIDs changed between versions (which would silently retarget
// every sprite reference in the project).
public static class SpriteIdDump
{
     [MenuItem("Custom/Dump Sprite IDs")]
     static void Dump()
     {
          string dir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SpriteIdDump");
          Directory.CreateDirectory(dir);
          string outPath = Path.Combine(dir, Application.unityVersion + ".csv");
          var sb = new StringBuilder();
          sb.AppendLine("textureGuid,spriteFileId,spriteName,texturePath");
          int spriteCount = 0, textureCount = 0;
          string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
          try
          {
               for (int i = 0; i < textureGuids.Length; i++)
               {
                    string path = AssetDatabase.GUIDToAssetPath(textureGuids[i]);
                    if (i % 50 == 0)
                         EditorUtility.DisplayProgressBar("Dumping sprite IDs", path, (float)i / textureGuids.Length);
                    bool any = false;
                    foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                    {
                         var sprite = obj as Sprite;
                         if (sprite == null)
                              continue;
                         if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localId))
                         {
                              sb.AppendLine($"{guid},{localId},{sprite.name},{path}");
                              spriteCount++;
                              any = true;
                         }
                    }
                    if (any)
                         textureCount++;
               }
          }
          finally
          {
               EditorUtility.ClearProgressBar();
          }
          File.WriteAllText(outPath, sb.ToString());
          Debug.Log($"Dumped {spriteCount} sprites from {textureCount} textures to {outPath}");
     }
}
