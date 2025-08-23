using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MARDEK.Core;
using MARDEK.Event;
using MARDEK.Animation;

namespace MARDEK.Movement
{
    public class PlayerController : MovementController
    {
        static PlayerController instance;
        MoveDirection desiredDirection = null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                GetComponentInChildren<SpriteAnimator>().SetAsPlayerAnimator();
            }
            else
                Debug.LogError("there was already a PlayerController instance when a new PlayerController awoke");
        }

        public static Movable GetPlayerMovement()
        {
            if (instance)
                return instance.movement;
            else
                return null;
        }

        public void OnInteraction(InputAction.CallbackContext ctx)
        {
            if (PlayerLocks.isPlayerLocked)
                return;

            if (ctx.performed == false)
                return;
            if(movement == null || movement.isMoving || movement.currentDirection == null)
                return;

               var collidersHit = Physics2D.OverlapBoxAll((Vector2)transform.position + movement.currentDirection.value, Vector2.one / 2, 0);
               foreach (Collider2D c in collidersHit)
               {
                    if (c.TryGetComponent(out CommandTrigger trigger))
                         trigger.Interact();
               }
        }
        
        public void OnMovementInput(InputAction.CallbackContext ctx)
        {
            Vector2 direction = ctx.ReadValue<Vector2>();
            if (direction.x == 0 || direction.y == 0)
                desiredDirection = GetMoveDirection(direction);
            else
                desiredDirection = null;
        }

        private void Update()
        {
            if (PlayerLocks.isPlayerLocked)
                return;
            SendDirection(desiredDirection);
        }
    }
}