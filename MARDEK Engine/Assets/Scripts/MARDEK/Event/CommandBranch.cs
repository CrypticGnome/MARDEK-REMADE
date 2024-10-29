using MARDEK.Core;
using MARDEK.Event;
using MARDEK.Save;
using UnityEngine;
using System.Collections;

public class CommandBranch : Command
{
     [SerializeField] Command[] OnTrue;
     [SerializeField] Command[] OnFalse;
     [SerializeField] LocalSwitchBool boolean;

     public bool IsOngoing { get; private set; } = false;

     [ContextMenu("Trigger")]
     public override void Trigger()
     {
          if (IsOngoing)
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
          IsOngoing = true;

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
          IsOngoing = false;
     }

     IEnumerator WaitForOnGoingCommand(OngoingCommand ongoingCommand)
     {
          // Lock player actions until command has finished
          if (ongoingCommand.WaitForExecutionEnd)
          {
               PlayerLocks.EventSystemLock++;
               yield return new WaitUntil(() => ongoingCommand.IsOngoing() == false);
               PlayerLocks.EventSystemLock--;
          }
     }
}
