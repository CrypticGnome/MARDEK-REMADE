using MARDEK.Core;
using System.Collections;
using UnityEngine;


namespace MARDEK.Event
{
     public class CommandChain : OngoingCommand
     {
          [SerializeField] Command[] commands;

          public override bool IsOngoing() => ongoing;
          bool ongoing = false;
          [ContextMenu("Trigger")]
          public override void Trigger()
          {
               if (IsOngoing())
               {
                    Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                    return;
               }
               for (int i = 0; i < commands.Length; i++) 
               {
                    commands[i].Trigger();
               }
          }    

          IEnumerator PerformCommandChain()
          {
               if (LockPlayerActions) PlayerLocks.EventSystemLock++;

               for (int i = 0; i < commands.Length; i++)
               {
                    Command command = commands[i];
                    if (command is null)
                         continue;
                    Debug.Log($"Running {command} on {name}");

                    if ((command is OngoingCommand ongoingCommand))
                    {
                         if (runParallel) StartCoroutine(ongoingCommand.TriggerAsync());
                         else yield return ongoingCommand.TriggerAsync();
                    }
                    else command.Trigger();
               }
               if (LockPlayerActions) PlayerLocks.EventSystemLock--;
               yield break;
          }


          public override IEnumerator TriggerAsync()
          {
               yield return PerformCommandChain();
          }
     }
}