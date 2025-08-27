using MARDEK.Core;
using MARDEK.Core.LevelDesign;
using MARDEK.Event;
using MARDEK.Save;
using System;
using System.Collections;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.NegotiationCommand;

namespace MARDEK.Event
{ 
     public class CommandBranch : OngoingCommand
     {
          [SerializeField] Command OnTrue;
          [SerializeField] Command OnFalse;
          [SerializeField] CommandBranchCondition branchCondition;
           bool isOngoing = false;

          public override bool IsOngoing()
          {
               return isOngoing;
          }

          [ContextMenu("Trigger")]
          public override void Trigger()
          {
               if (isOngoing)
               {
                    Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                    return;
               }
               Command command = branchCondition.GetValue(this) ? OnTrue : OnFalse;

               if (command is null)
               {
                    Debug.LogError($"Null exception in {name} of type Command Branch");
                    return;
               }
               command.Trigger();

          }

          public override IEnumerator TriggerAsync()
          {
               if (LockPlayerActions) PlayerLocks.EventSystemLock++;
               if (isOngoing)
               {
                    Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                    yield break;
               }
               Command command = branchCondition.GetValue(this) ? OnTrue : OnFalse;
    
               if (command is null)
               {
                    Debug.LogAssertion($"Null exception in {transform.parent} {name} of type Command Branch");
                    isOngoing = false;
                    if (LockPlayerActions) PlayerLocks.EventSystemLock--;
                    yield break;
               }
               isOngoing = true;

               if (command is OngoingCommand ongoing && RunParallel)
               {
                    StartCoroutine(ongoing.TriggerAsync());
                    isOngoing = false;
                    if (LockPlayerActions) PlayerLocks.EventSystemLock--;
                    yield break;
               }
               command.Trigger();
               isOngoing = false;
               if (LockPlayerActions) PlayerLocks.EventSystemLock--;

          }


          [Serializable]
          public class CommandBranchCondition
          {
               public bool UsingSwitchBool = true;
               public BoolComponent LocalSwitchBool;
               public ConditionComponent ConditionComponent;

               public bool GetValue(MonoBehaviour behaviour)
               {
                    if (UsingSwitchBool)
                    {
                         if (LocalSwitchBool is null)
                         {
                              Debug.LogWarning($"Switch bool is null in {behaviour.name}");
                              return false;
                         }
                         return LocalSwitchBool.Value;
                    }

                    if (ConditionComponent is null || ConditionComponent.Condition is null)
                    {
                         Debug.LogWarning($"Null exception in Condition component of {behaviour.name}");
                         return false;
                    }

                    return ConditionComponent.Condition.Value;
               }
          }
     }


}

