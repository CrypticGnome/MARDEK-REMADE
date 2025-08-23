using MARDEK.Core;
using MARDEK.Event;
using MARDEK.Save;
using UnityEngine;
using System.Collections;
using MARDEK.Core.LevelDesign;
using System;

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

          if (branchCondition.GetValue(this))
          {
               if (OnTrue is null)
               {
                    Debug.LogError($"Null exception in {name} of type Command Branch");
                    return;
               }
               OnTrue.Trigger();
          }
          else
          {
               if (OnFalse is null)
               {
                    Debug.LogError($"Null exception in {name} of type Command Branch");
                    return;
               }
               OnFalse.Trigger();
          }
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

