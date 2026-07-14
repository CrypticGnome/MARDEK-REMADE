using UnityEngine;

namespace MARDEK.UI
{
	public class WorldMap : MonoBehaviour
	{
		[SerializeField] Transform waypointsFolder;

		Path[] allPaths;
		MapWaypoint[] allWaypoints;

		MapWaypoint currentWaypoint;
		MapWaypoint nextWaypoint;
		float startMoveTime;
		float stopMoveTime;


	}
}