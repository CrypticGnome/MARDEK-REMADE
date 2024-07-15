using MARDEK.Core;
using MARDEK.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MARDEK.UI
{
     [RequireComponent(typeof(SpriteRenderer), typeof (SceneTransitionCommand))]
     public class MapWaypoint : MonoBehaviour
     {

          [SerializeField] SpriteRenderer spriteRenderer;
          [SerializeField] SceneTransitionCommand sceneTransitionCommand;
          [SerializeField] MapPlayerIcon player;

          [SerializeField] MapConnection upperLocation, lowerLocation, rightLocation, leftLocation;


          [SerializeField] MapLocation location;
          static MapConnection LastUsedConnection = null;

          public MapLocation Location { get { return location; } }
          public MapConnection UpperLocation { get { return upperLocation; } }
          public MapConnection LowerLocation { get { return lowerLocation; } }
          public MapConnection RightLocation { get { return rightLocation; } }
          public MapConnection LeftLocation { get { return leftLocation; } }

          public bool Enabled { get { return location.Discovered; } }
          private void Awake()
          {
               player.OnPlayerArrived += Arrive;
               player.OnPlayerLeave += Leave;
               if (player.CurrentWaypoint is not null)
                    return;
               if (player.LastUsedWaypoint is null)
                    return;
               if (player.LastUsedWaypoint.Location.Equals(location))
                    player.CurrentWaypoint = this;
          }
          private void OnEnable()
          {
               if (Enabled)
                    spriteRenderer.enabled = true;
               else
                    spriteRenderer.enabled = false;
          }

          public bool HasBeenDiscovered()
          {
               return location.Discovered;
          }



          public void Leave(MapWaypoint waypoint)
          {
               if (waypoint != this)
                    return;
               spriteRenderer.sprite = location.DefaultIcon;

          }

          public void Arrive(MapConnection connection)
          {
               if (connection.Waypoint != this)
                    return;
               spriteRenderer.sprite = location.ActiveIcon;
               LastUsedConnection = connection;
          }
          public void Enter()
          {
               sceneTransitionCommand.EnterCustom(location.Scene, LastUsedConnection.EnterWaypoint, LastUsedConnection.OverrideFacingDirection);
          }
          
     }
     [Serializable]
     public class MapConnection
     {
          public MapWaypoint Waypoint;
          public WaypointEnum EnterWaypoint;
          public MoveDirection OverrideFacingDirection = null;
     }
}