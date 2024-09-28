using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

namespace MARDEK
{
    public class BattleModelSeperator : MonoBehaviour
    {
          [SerializeField] Animator animator;
          [SerializeField] UnityEngine.Animation animation;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

          [MenuItem("Tools/Remove Missing Animations")]
          [ContextMenu("RemoveAnimations")]
          public void RemoveMissingAnimations()
          {
               Debug.Log("BOB");
              // foreach (AnimationClip clip in animation)
               //AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
               //if (controller == null) return;
               //
               //foreach (AnimationClip clip in controller.animationClips)
               //{
               //     SerializedObject serializedClip = new SerializedObject(clip);
               //     SerializedProperty curves = serializedClip.FindProperty("m_Curves");
               //
               //     List<int> curvesToRemove = new List<int>();
               //
               //     // Check all curves in the animation clip
               //     for (int i = 0; i < curves.arraySize; i++)
               //     {
               //          SerializedProperty curve = curves.GetArrayElementAtIndex(i);
               //          string path = curve.FindPropertyRelative("path").stringValue;
               //          GameObject targetGO = gameObject.transform.Find(path)?.gameObject;
               //
               //          // If the GameObject is missing, mark this curve for removal
               //          if (targetGO == null)
               //          {
               //               curvesToRemove.Add(i);
               //          }
               //     }
               //
               //     // Remove the marked curves (from the end to avoid reindexing issues)
               //     for (int i = curvesToRemove.Count - 1; i >= 0; i--)
               //     {
               //          curves.DeleteArrayElementAtIndex(curvesToRemove[i]);
               //     }
               //
               //     serializedClip.ApplyModifiedProperties();
               //}
               //
               //Debug.Log($"Processed {gameObject.name} and removed missing animation references.");


               AssetDatabase.SaveAssets();
          }
     }
}
