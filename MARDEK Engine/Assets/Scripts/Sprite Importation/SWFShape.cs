using UnityEditor;
using UnityEngine;

public class SWFShape : MonoBehaviour
{
     private void OnValidate()
     {

#if UNITY_EDITOR
          // Option 2 (editor-only): load by GUID/Asset path
           Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Sprites/Unlit_VectorGradient.mat");
#endif
          GetComponent<SpriteRenderer>().material = mat;


     }
     public void Create(Sprite sprite)
     {
          gameObject.name = sprite.name;

          var sr = gameObject.AddComponent<SpriteRenderer>();
          sr.sprite = sprite;
     }
     [ContextMenu("CalculateSortingOrder")]
     public void CalculateSortingOrderAndColor()
     {
          int order = 0;
          var upperObject = GetComponentInParent<SWFPlacedObject>(includeInactive: true);
          Color color = Color.white;
          while (upperObject != null)
          {
               color = upperObject.GetColor();
               order += upperObject.Depth;
               var parent = upperObject.transform.parent;
               if (parent == null)
                    upperObject = null;
               else
                    upperObject = parent.GetComponentInParent<SWFPlacedObject>(includeInactive: true);
          }
          var r = gameObject.GetComponent<SpriteRenderer>();
          r.sortingOrder = order;
          r.color = color;
     }
}