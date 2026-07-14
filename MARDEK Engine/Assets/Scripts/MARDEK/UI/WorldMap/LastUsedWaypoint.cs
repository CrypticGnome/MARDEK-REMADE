using MARDEK.Core;
using MARDEK.Movement;
using UnityEngine;


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