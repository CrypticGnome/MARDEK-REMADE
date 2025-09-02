using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MARDEK.UI
{
     public class PathGenerator : MonoBehaviour
    {
          [SerializeField] Transform waypointFolder;
          [SerializeField] Transform pathFolder;
          List<WaypointPair> path = new List<WaypointPair>();
          private void Awake()
          {
               GeneratePaths();
               Debug.Log("Map has " + path.Count + " paths");
          }
          public void GeneratePaths()
          {
               DestroyPreviousPaths();
               GetWaypointConnections();
          }
          void DestroyPreviousPaths()
          {
               int pathCount = pathFolder.childCount;

               for (int pathIndex = pathCount - 1; pathIndex <= 0; pathIndex++)
               {
                    Destroy(transform.GetChild(pathIndex).gameObject);
               }
          }
          void GetWaypointConnections()
          {
               MapWaypoint[] waypoints = waypointFolder.GetComponentsInChildren<MapWaypoint>();
               List<WaypointPair> connections = new List<WaypointPair>();

               foreach (MapWaypoint waypoint in waypoints) 
               {

                    if (waypoint.UpperLocation != null)
                         connections.Add(new WaypointPair(waypoint,waypoint.UpperLocation.Waypoint));
                    if (waypoint.LowerLocation != null)
                         connections.Add(new WaypointPair(waypoint, waypoint.LowerLocation.Waypoint));
                    if (waypoint.RightLocation != null)
                         connections.Add(new WaypointPair(waypoint, waypoint.RightLocation.Waypoint));
                    if (waypoint.LeftLocation != null)
                         connections.Add(new WaypointPair(waypoint, waypoint.LeftLocation.Waypoint));
               }
               foreach (WaypointPair connection in connections)
               {
                    bool pathAlreadyAdded = false;
                    foreach (WaypointPair path in path)
                    {
                         if (path.Equals(connection))
                         {
                              pathAlreadyAdded = true;
                              break;
                         }
                    }
                    if (!pathAlreadyAdded)
                         path.Add(connection);
               }
          }

          class WaypointPair
          {
               public MapWaypoint[] waypoints = new MapWaypoint[2];

               public WaypointPair (MapWaypoint waypoint1, MapWaypoint waypoint2)
               {
                    waypoints[0] = waypoint1;
                    waypoints[1] = waypoint2;
               }
               public bool Equals(WaypointPair other) 
               {
                    return waypoints.All( waypoint => other.waypoints.Contains(waypoint) );
               }
          }
     }
}
