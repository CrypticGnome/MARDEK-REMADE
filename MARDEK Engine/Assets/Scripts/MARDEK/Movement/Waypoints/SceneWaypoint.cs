using UnityEngine;
using System.Collections.Generic;
using MARDEK.Core;
using MARDEK.Progress;

namespace MARDEK.Movement
{
    [RequireComponent(typeof(GridObject))]
    public class SceneWaypoint : MonoBehaviour
    {
        [SerializeField] WaypointEnum thisWaypoint = null;
        [SerializeField] CharacterPositions characterPositions;
        private void Awake()
        {
               if (SceneTransitionCommand.usedWaypoint == null)
                    return;
               
               if (thisWaypoint == SceneTransitionCommand.usedWaypoint)
               {
                   MapParty.OverrideAfterTransition(transform.position, SceneTransitionCommand.transitionFacingDirection);
                   SceneTransitionCommand.ClearUsedWaypoint();
               }
               
        }
    } 
}