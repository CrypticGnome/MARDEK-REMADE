using MARDEK.Core;
using MARDEK.Event;
using MARDEK.Save;
using UnityEngine;
using System.Collections;

public class CommandBranch : OngoingCommand
{
     [SerializeField] Command[] OnTrue;
     [SerializeField] Command[] OnFalse;
     [SerializeField] LocalSwitchBool boolean;

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

          if (boolean.GetBoolValue())
               StartCoroutine(PerformCommandChain(OnTrue));
          else
               StartCoroutine(PerformCommandChain(OnFalse));
     }



     IEnumerator PerformCommandChain(Command[] commands)
     {
          isOngoing = true;

          for (int i = 0; i < commands.Length; i++)
          {
               Command command = commands[i];
               if (command is null)
                    continue;

               command.Trigger();

               if (command is OngoingCommand)
               {
                    IEnumerator waitForCommandToFinish = WaitForOnGoingCommand(command as OngoingCommand);
                    yield return command.StartCoroutine(waitForCommandToFinish);
               }
          }
          isOngoing = false;
     }

     IEnumerator WaitForOnGoingCommand(OngoingCommand ongoingCommand)
     {
          // Lock player actions until command has finished
          if (ongoingCommand.LockPlayerActions)
          {
               PlayerLocks.EventSystemLock++;
               yield return new WaitUntil(() => ongoingCommand.IsOngoing() == false);
               PlayerLocks.EventSystemLock--;
          }
     }
}
