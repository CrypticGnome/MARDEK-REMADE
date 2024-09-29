using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using UnityEditor;
using UnityEngine;

namespace MARDEK
{
    public class BattleModelAnimatorTestor : MonoBehaviour
    {
     [SerializeField] UnityEngine.Animation Animation;
          [SerializeField] Material spriteMaterial;
          [SerializeField] Vector3 spriteScale;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        [ContextMenu("Play Animation")]
          public void RunAnimation()
          {
               StartCoroutine(PlayAllAnimation());
               
          }
          #if UNITY_EDITOR
          IEnumerator PlayAllAnimation()
          {
               foreach (AnimationState anim in Animation)
               {
                    Animation.clip = anim.clip;
                    Animation.Play();
                    yield return new WaitUntil(() => Animation.isPlaying == false);
               }
          }
          [ContextMenu("Set Sprite Material")]
          public void SetSpriteMaterial()
          {
               SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
               foreach (SpriteRenderer spriteRenderer in spriteRenderers)
               {
                    spriteRenderer.material = spriteMaterial;
               }
          }

          [ContextMenu("Delete Redundant GOs")]
          public void Bob()
          {
               Transform[] goTransforms = GetComponentsInChildren<Transform>(includeInactive: true);
               long size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(gameObject);
               Debug.Log($"Object is {size} bytes");
               HashSet<Transform> transformsUsedInAnimations = new HashSet<Transform>();
               foreach (AnimationState anim in Animation)
               {
                    AnimationClip clip = anim.clip;
                    EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
                    foreach (EditorCurveBinding binding in bindings)
                    {
                         string gameObjectPath = binding.path;

                         Transform target = transform.Find(gameObjectPath);
                         transformsUsedInAnimations.Add(target);
                    }
               }

               for (int i = 0; i < goTransforms.Length; i++) 
               {
                    Transform goTransform = goTransforms[i];
                    if (goTransform is null)
                         continue;

                    if (goTransform.GetComponentsInChildren<SpriteRenderer>().Length == 0)
                         Destroy(goTransform.gameObject);
                    bool usedInAnimation = false;
                    usedInAnimation = SearchChildren(goTransform, usedInAnimation);
                    if (!usedInAnimation)
                         Destroy(goTransform.gameObject);

               }
               size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(gameObject);
               Debug.Log($"Object is {size} bytes");

               bool SearchChildren(Transform transform, bool used)
               {
                    if (transformsUsedInAnimations.Contains(transform))
                         return true;
                    foreach (Transform children in transform.GetComponentsInChildren<Transform>())
                    {
                         return used || SearchChildren(children, used);
                    }
                    return used;
               }
          }

          [ContextMenu("Set All Sprites' Scale")]
          public void SetSpriteSize()
          {
               SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
               foreach (SpriteRenderer spriteRenderer in spriteRenderers)
               {
                    spriteRenderer.transform.localScale = spriteScale;
               }
          }
#endif
     }
}
