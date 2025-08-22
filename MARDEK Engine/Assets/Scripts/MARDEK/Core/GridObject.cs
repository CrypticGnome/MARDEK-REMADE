using MARDEK.Core;
using UnityEngine;

namespace MARDEK
{
     #if UNITY_EDITOR
    [ExecuteInEditMode, SelectionBase]
    public class GridObject : MonoBehaviour
    {
          void Update()
          {
               if (Application.isPlaying) return;

               float z = transform.position.z;
               Vector3 pos = transform.position.ToGridCentre();
               pos.z = z;
               if (transform.position != pos)
                    transform.position = pos;
          }


        private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, Vector3.one);

    }
    #endif
}