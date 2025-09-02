using UnityEngine;
using MARDEK.Progress;

namespace MARDEK.Movement
{
     #if UNITY_EDITOR
    [RequireComponent(typeof(GridObject))]
    #endif
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