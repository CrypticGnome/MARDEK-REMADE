using MARDEK.Animation;
using MARDEK.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MARDEK.Movement
{
    public class Movable : MonoBehaviour
    {
        [SerializeField] float movementSpeed = 1f;

        [SerializeField] ColliderHelper _colliderHelper = null;
        [SerializeField] SpriteAnimator animator = null;

        public ColliderHelper colliderHelper { get { return _colliderHelper; } }
        public bool isMoving { get; private set; }
        public MoveDirection currentDirection { get; private set; }

        Vector2 _lastPosition = Vector2.zero;

        public Vector2 lastPosition
        {
            get
            {
                return _lastPosition;
            }
            private set
            {
                _lastPosition = value;
            }
        }
        Vector2 targetPosition = Vector2.zero;
        Queue<MoveDirection> queuedMoves = new Queue<MoveDirection>();

        public delegate void MoveDelegate();

        public event MoveDelegate OnStartMove = delegate { };
        public event MoveDelegate OnEndMove = delegate { };

        private void Start()
        {
            targetPosition = transform.position;            
        }

        public void EnqueueMoves(ICollection<MoveDirection> directions)
        {
            foreach (MoveDirection direction in directions)
                queuedMoves.Enqueue(direction);
            UpdateMoveStatus();
        }
          public void EnqueueMoves(ICollection<Moves> moves)
          {
               foreach (Moves move in moves)
                    for (int i = 0; i < move.Count;i++)
                         queuedMoves.Enqueue(move.Direction);
               UpdateMoveStatus();
          }

          public void MoveInDirectionOnce(MoveDirection direction)
        {
            if (isMoving)
                return;
            queuedMoves.Clear();
            if(direction != null)
            {
                queuedMoves.Enqueue(direction);
                UpdateMoveStatus();
            }
        }

        public void FaceDirection(MoveDirection direction)
        {
            currentDirection = direction;
            UpdateAnimatorWithCurrentDirection();
            StopAnimator();
        }

        private void Update()
        {
               if (!isMoving)
                    return;
               
               isMoving = !MoveToPosition(transform, targetPosition, movementSpeed, Time.deltaTime);
               if(isMoving == false)
               {
                   transform.Set2DPosition(targetPosition.SnapToGrid());
                   OnEndMove.Invoke();
               }
               UpdateMoveStatus();
        }

        void UpdateMoveStatus()
        {            
               if (isMoving)
               {
                    if (colliderHelper)
                         colliderHelper.OffsetCollider(targetPosition - (Vector2)transform.position);
                    return;
               }
            
               bool shouldMove = ShouldMove();
               if (shouldMove)
               {
                   lastPosition = transform.position;
                   isMoving = true;
                   OnStartMove.Invoke();
               }
               else
               {
                   if (colliderHelper) colliderHelper.OffsetCollider(Vector2.zero);
                   FaceDirection(currentDirection);
               }   
        }

        bool ShouldMove()
        {
            Vector2 previousTargetPosition = targetPosition;
            bool hasNextMove = GetNextTargetPosition();
            if (hasNextMove)
            {
                UpdateAnimatorWithCurrentDirection();
                if (colliderHelper == null)
                    return true;
                Vector2 offset = targetPosition - (Vector2)transform.position;
                if (colliderHelper.LineCast(offset).Count == 0)
                {
                    return true;
                }
            }
            targetPosition = previousTargetPosition;
            return false;
        }

        bool GetNextTargetPosition()
        {
               if(queuedMoves.Count <= 0)
                    return false;

               
               currentDirection = queuedMoves.Dequeue();
               if (currentDirection == null)
                   return false;

               targetPosition = (Vector2)transform.position + currentDirection.value;
               return true;
        }

        private void UpdateAnimatorWithCurrentDirection()
        {
            if (animator) animator.PlayClipByMoveDirectionReference(currentDirection);
        }

        void StopAnimator()
        {
            if (animator) animator.StopCurrentAnimation(1); //end with last sprite
        }

        bool MoveToPosition(Transform transform, Vector2 targetPosition, float movementSpeed, float deltaTime)
        {
            // returns true if movement has reached the target
            if (targetPosition.IsApproximately(transform.position)) return true;


               Vector2 positionDifferece = targetPosition - (Vector2)transform.position;
               Vector2 increment = positionDifferece.normalized * deltaTime * movementSpeed;
               if (increment.sqrMagnitude < positionDifferece.sqrMagnitude)
               {
                   transform.Set2DPosition((Vector2)transform.position + increment);
                   return targetPosition.IsApproximately(transform.position);
               }

               return true;
        }
    }
}