using System;
using UnityEngine;

namespace MARDEK
{
    public class Utilities2D
    {

        public static Vector3 SnapPositionToGrid(Vector3 pos)
        {
               pos = Vector3Int.FloorToInt(pos);
               pos += Vector3.one / 2;

               return pos;
        }

        public static void SetTransformPosition(Transform transform, Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        public static bool AreCloseEnough(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude < 4 * Vector2.kEpsilon;
        }
    }
}