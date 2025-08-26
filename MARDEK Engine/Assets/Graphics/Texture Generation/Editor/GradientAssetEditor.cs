using MARDEK.Editor;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace MARDEK
{
     [CustomEditor(typeof(GradientAsset))]

     public class GradientAssetEditor : UnityEditor.Editor
    {
          public override void OnInspectorGUI()
          {
               base.OnInspectorGUI();

               GradientAsset gradAsset = (GradientAsset)target;
               if (GUILayout.Button("Bake Gradient Texture"))
               {
                    gradAsset.BakeGradientTexture();

               }
          }
     }
}
