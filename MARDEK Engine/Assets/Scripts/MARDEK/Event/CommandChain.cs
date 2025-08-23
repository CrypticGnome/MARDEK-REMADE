using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MARDEK.Core;
using UnityEditor;
using UnityEditor.UIElements;

namespace MARDEK.Event
{
     public class CommandChain : Command
     {
          [SerializeField] Command[] commands;

          public bool IsOngoing { get; private set; } = false;

          [ContextMenu("Trigger")]
          public override void Trigger()
          {
               if (IsOngoing)
               {
                    Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                    return;
               }
               StartCoroutine(PerformCommandChain());
          }    

          IEnumerator PerformCommandChain()
          {
               IsOngoing = true;

               for (int i = 0; i < commands.Length; i++)
               {
                    Command command = commands[i];
                    if (command is null)
                         continue;
                    Debug.Log($"Running {command} on {name}");

                    command.Trigger();

                    
                    if (command is OngoingCommand ongoingCommand) 
                         yield return WaitForOnGoingCommand(ongoingCommand);
               }
               IsOngoing = false;
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
               else yield return new WaitUntil(() => ongoingCommand.IsOngoing() == false);
          }

     }
}