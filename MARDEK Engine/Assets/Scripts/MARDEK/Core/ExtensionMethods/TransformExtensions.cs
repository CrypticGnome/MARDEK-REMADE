using System.Runtime.CompilerServices;
using UnityEngine;

namespace MARDEK.Core
{
     public static class TransformExtensions
     {
          public static void Set2DPosition(this Transform transform, Vector2 position)
          {
               transform.position = new Vector3(position.x, position.y, transform.position.z);
          }
     }
}