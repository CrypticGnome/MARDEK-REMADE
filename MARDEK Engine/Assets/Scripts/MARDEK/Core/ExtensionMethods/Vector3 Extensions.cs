using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace MARDEK.Core
{
     public static class Vector3Extensions
     {
          public static Vector3 ToGridCentre(this Vector3 position)
          {
               position = Vector3Int.FloorToInt(position);
               position += Vector3.one / 2;

               return position;
          }
     }

     public static class Vector2Extensions
     {
          public static Vector2 SnapToGrid(this Vector2 position)
          {
               position = Vector2Int.FloorToInt(position);
               position += Vector2.one / 2;

               return position;
          }

          public static bool IsApproximately(this Vector2 a, Vector2 b)
          {
               return (a - b).sqrMagnitude < 0.001f;
          }
     }
}