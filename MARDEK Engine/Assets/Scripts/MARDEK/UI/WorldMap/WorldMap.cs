using MARDEK.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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