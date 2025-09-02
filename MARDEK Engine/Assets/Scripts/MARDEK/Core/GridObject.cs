using MARDEK.Core;
using UnityEngine;

namespace MARDEK
{
     #if UNITY_EDITOR
    [ExecuteInEditMode, SelectionBase]
    public class GridObject : MonoBehaviour
    {
          private void OnValidate()
          {
               Vector2 centredGridPos = transform.position.ToGridCentre();
               if (centredGridPos != (Vector2)transform.position)
                    transform.Set2DPosition(centredGridPos);

          }
          
        private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, Vector3.one);

    }
    #endif
}