using MARDEK.Core;
using MARDEK.Movement;
using MARDEK.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MARDEK.UI
{
    public class MapPlayerIcon : MonoBehaviour
    {
          public MapWaypoint CurrentWaypoint;
          [SerializeField] LastUsedWaypoint lastUsedWaypoint;
          public LastUsedWaypoint LastUsedWaypoint { get { return lastUsedWaypoint; } private set { lastUsedWaypoint = value; } }
          [SerializeField] MapWaypoint defaultWaypoint;
          [SerializeField] InputReader inputReader;
          [SerializeField] SceneTransitionCommand sceneTransitionCommand;
          public delegate void PlayerLeave(MapWaypoint playerWapoint);
          public event PlayerLeave OnPlayerLeave;
          public delegate void PlayerArrived(MapConnection playerWapoint);
          public event PlayerArrived OnPlayerArrived;
          bool atWaypoint { get { return CurrentWaypoint != null; } }
          Vector2Int desiredDirection;
          private void OnEnable()
          {
               if (LastUsedWaypoint.Location is null)
               {
                    Debug.LogWarning("Current waypoint is null, setting it to " + defaultWaypoint.name);
                    CurrentWaypoint = defaultWaypoint;
               }
               transform.position = CurrentWaypoint.transform.position;
          }
          // Start is called before the first frame update
          void Start()
          {
        
          }

          // Update is called once per frame
          void Update()
          { 
        
          }
          public void OnMovementInput(InputAction.CallbackContext ctx)
          {
               Vector2 direction = ctx.ReadValue<Vector2>();
               desiredDirection = GetMoveDirection(direction);

               if (desiredDirection == Vector2.up)
                    UseConnection(CurrentWaypoint.UpperLocation);
               else if (desiredDirection == Vector2.down)
                    UseConnection(CurrentWaypoint.LowerLocation);
               else if (desiredDirection == Vector2.left)
                    UseConnection(CurrentWaypoint.LeftLocation);
               else if (desiredDirection == Vector2.right)
                    UseConnection(CurrentWaypoint.RightLocation);
          }

          Vector2Int GetMoveDirection(Vector2 inputDirection)
          {
               if (Utilities2D.AreCloseEnough(inputDirection, Vector2.zero))
                    return Vector2Int.zero;

               Vector2Int[] allowedDirections = GetAllowedDirections();
               if (allowedDirections.Length == 0)
                    return Vector2Int.zero;


               Vector2Int result = Vector2Int.zero;
               foreach (Vector2Int direction in allowedDirections)
               {
                    if (Vector2.Distance(result, inputDirection) > Vector2.Distance(direction, inputDirection))
                    {
                         result = direction;
                    }
               }
               return result;
          }

          Vector2Int[] GetAllowedDirections()
          {
               if (CurrentWaypoint is null)
                    return null;
               List<Vector2Int> allowedDirections = new List<Vector2Int>();

               if (CurrentWaypoint.UpperLocation.Waypoint != null)
                    allowedDirections.Add(Vector2Int.up);
               if (CurrentWaypoint.LowerLocation.Waypoint != null)
                    allowedDirections.Add(Vector2Int.down);
               if (CurrentWaypoint.LeftLocation.Waypoint != null)
                    allowedDirections.Add(Vector2Int.left);
               if (CurrentWaypoint.RightLocation.Waypoint != null)
                    allowedDirections.Add(Vector2Int.right);

               return allowedDirections.ToArray();
          }

          void UseConnection(MapConnection connection)
          {
               OnPlayerLeave?.Invoke(CurrentWaypoint);
                    
               CurrentWaypoint = connection.Waypoint;
               LastUsedWaypoint.Location = CurrentWaypoint.Location;
               LastUsedWaypoint.EnterWaypoint = connection.EnterWaypoint;
               LastUsedWaypoint.OverrideFacingDirection = connection.OverrideFacingDirection;
               transform.position = connection.Waypoint.transform.position;
               OnPlayerArrived?.Invoke(connection);
          }
          public void LoadArea()
          {
               CurrentWaypoint.Enter();
          }

     }
}
