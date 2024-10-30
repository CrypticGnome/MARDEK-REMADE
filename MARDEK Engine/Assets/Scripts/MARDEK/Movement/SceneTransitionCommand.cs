using UnityEngine;
using UnityEngine.SceneManagement;
using MARDEK.Core;
using MARDEK.Event;

namespace MARDEK.Movement
{
    public class SceneTransitionCommand : Command
    {
        public static WaypointEnum usedWaypoint { get; private set; }
        public static MoveDirection transitionFacingDirection { get; private set; }

        public SceneReference scene = null;
        public WaypointEnum waypoint = null;
        public MoveDirection overrideFacingDirection = null;

        public override void Trigger()
        {
            usedWaypoint = waypoint;
            SetupFacingDirection();

            //Command queue won't have the oportunity to reset the lockValue itself cause the scene reload will destroy the object
            PlayerLocks.EventSystemLock = 0;

            SceneManager.LoadScene(scene);
        }

        void SetupFacingDirection()
        {
            if (overrideFacingDirection)
                transitionFacingDirection = overrideFacingDirection;
            else
            {
                var player = PlayerController.GetPlayerMovement();
                if (player)
                    transitionFacingDirection = player.currentDirection;
                else
                    transitionFacingDirection = null;
            }
        }

        public static void ClearUsedWaypoint()
        {
            usedWaypoint = null;
        }
        public void EnterCustom(SceneReference scene, WaypointEnum waypointEnum, MoveDirection overrideFacingDirection)
        {
               this.scene = scene;
               this.waypoint = waypointEnum;
               this.overrideFacingDirection = overrideFacingDirection;
               Trigger();
        }
    }
}