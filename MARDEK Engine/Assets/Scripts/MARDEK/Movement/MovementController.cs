using System.Collections.Generic;
using UnityEngine;
using MARDEK.Core;

namespace MARDEK.Movement
{
    public abstract class MovementController : MonoBehaviour
    {
        [SerializeField] protected Movable movement = null;
        [SerializeField] protected List<MoveDirection> allowedDirections = new List<MoveDirection>();

        public void SendDirection(MoveDirection direction)
        {
            if (movement) movement.MoveInDirectionOnce(direction);
        }

        public MoveDirection ApproximanteDirectionByVector2(Vector2 vector)
        {
            if(Utilities2D.AreCloseEnough(vector, Vector2.zero))
                return null;
            if (allowedDirections.Count == 0)
               return null;

            
            MoveDirection result = allowedDirections[0];
            foreach (MoveDirection dir in allowedDirections)
            {
                if (Vector2.Distance(result.value, vector) > Vector2.Distance(dir.value, vector))
                {
                    result = dir;
                }
            }
            return result;
        }
    }
}