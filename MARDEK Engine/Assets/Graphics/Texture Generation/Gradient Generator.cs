using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace MARDEK.Editor
{
     [CreateAssetMenu(fileName = "GradientAsset", menuName = "Custom/Gradient Generator")]
     public class GradientAsset : ScriptableObject
     {
          public Gradient gradient;
          public int width = 256;
          public int height = 1;

          [Tooltip("Input the saving path here. Changing the saving path is recommended incase of updates require deletion of the plugin.")]
          public string savingPath;
          public string name;

          public enum TextureFormat { Png, Jpg };
          [Header("Texture Baker")]
          public TextureFormat textureFormat;

          public Texture2D GenerateTexture()
          {
               Texture2D tex = new Texture2D(width, height);
               for (int x = 0; x < width; x++)
               {
                    Color c = gradient.Evaluate((float)x / (width - 1));
                    for (int y = 0; y < height; y++)
                    {
                         tex.SetPixel(x, y, c);
                    }
               }
               tex.wrapMode = TextureWrapMode.Clamp;
               tex.Apply();
               return tex;
          }

          [ContextMenu("Generate Texture")]
          public void BakeGradientTexture()
          {
               string saveFormat = ".png";

               switch (textureFormat)
               {
                    case TextureFormat.Png:
                         saveFormat = ".png";
                         break;
                    case TextureFormat.Jpg:
                         saveFormat = ".jpg";
                         break;
               }

               var _gradientTexture = GenerateTexture();
               byte[] _bytes = _gradientTexture.EncodeToPNG();

               string relativePath = $"{savingPath}{name}{saveFormat}";
               string fullPath = Path.GetFullPath(relativePath);

               // Ensure directory exists
               string dir = Path.GetDirectoryName(fullPath);
               if (!Directory.Exists(dir))
               {
                    Debug.LogAssertion($"Directory {dir} does not exist");
                    return;
               }

               // Write file
               File.WriteAllBytes(fullPath, _bytes);

               // Refresh asset database so Unity picks it up
               AssetDatabase.Refresh();

               Debug.Log("<color=#00FF00><b> GradientTexture_" + name + saveFormat + " baked sucessfully. Saved in the following path: " + savingPath + "</b></color>");
          }
     }
}
