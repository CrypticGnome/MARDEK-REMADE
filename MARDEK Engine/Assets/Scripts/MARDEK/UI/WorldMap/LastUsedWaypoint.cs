using MARDEK.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.Core;


namespace MARDEK.UI
{
     [CreateAssetMenu(menuName = "MARDEK/World Map/Last Used Waypoint ")]
     public class LastUsedWaypoint : ScriptableObject
     {
          public MapLocation Location;
          public WaypointEnum EnterWaypoint;
          public MoveDirection OverrideFacingDirection = null;
     }
}