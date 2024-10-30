using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Movement
{
     [RequireComponent(typeof(Collider2D))]
     public class ColliderHelper : MonoBehaviour
     {
          Collider2D _collider = null;
          ContactFilter2D filter = default;
          List<Collider2D> results = new List<Collider2D>();
          Collider2D[] colliders = new Collider2D[8];

          private void Awake()
          {
               InitializeFields();
          }

          void InitializeFields()
          {
               _collider = GetComponent<Collider2D>();
               if (_collider)
               {
                    filter.useLayerMask = true;
                    LayerMask mask = Physics2D.GetLayerCollisionMask(gameObject.layer);
                    filter.layerMask = mask;
               }
          }

          public void OffsetCollider(Vector2 offset)
          {
               if (_collider)
                    _collider.offset = offset;
          }

          public List<Collider2D> Overlaping(Vector2 offset)
          {
               results.Clear();
               if (_collider == null)
                    return results;
               _collider.offset = offset;
               System.Array.Clear(colliders, 0, colliders.Length);
               _collider.Overlap(filter, colliders);
               foreach (Collider2D c in colliders)
                    if (c != null && c.isActiveAndEnabled)
                         results.Add(c);
               return results;
          }

          public List<Collider2D> LineCast(Vector2 lineDelta)
          {
               this.results.Clear();
               RaycastHit2D[] results = new RaycastHit2D[8];
               _collider.Raycast(lineDelta, filter, results, lineDelta.magnitude);
               foreach (RaycastHit2D result in results)
                    if (result.collider != null && result.collider.isActiveAndEnabled)
                         this.results.Add(result.collider);
               return this.results;
          }
     }
}