using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.Core;
using UnityEngine.Rendering;

namespace MARDEK.Event
{
    [System.Serializable]
    public class CommandQueue
    {
          [SerializeField] GameObject commandsGameObject;
          [SerializeField] Command[] commands;
        public bool isOngoing { get; private set; } = false;
        Queue<Command> commandQueue = new Queue<Command>();
        Command currentCommand = null;
          public void TriggerSecond()
          {
               if (isOngoing)
               {
                    Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                    return;
               }
               isOngoing = true;

               
               isOngoing = false;

          }
          void Trigger()
          {
               if (isOngoing)
               {
                    Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                    return;
               }
               //StartCoroutine(PerformCommandChain());
          }
          IEnumerator PerformCommandChain()
          {
               isOngoing = true;

               for (int i = 0; i < commands.Length; i++)
               {
                    Command command = commands[i];
                    if (command is null)
                         continue;

                    ///GetAndTriggerNextCommand()
                    command.Trigger();
                    // Lock shit up and UpdateCurrentCommand()
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
               if (ongoingCommand.waitForExecutionEnd)
               { 
                    PlayerLocks.EventSystemLock++;
                    yield return new WaitUntil(() => ongoingCommand.IsOngoing() == false);
                    PlayerLocks.EventSystemLock--;
               }
          }

          public void TriggerFirst()
        {
               if (isOngoing)
               {
                   Debug.LogWarning("Trying to trigger event, but this event is already ongoing");
                   return;
               }
               if(commandsGameObject == null)
               {
                   return;
               }

               commandQueue = new Queue<Command>(commandsGameObject.GetComponents<Command>());
               if (commandQueue.Count < 1)
               {
                    Debug.LogWarning("Trying to trigger event with no commands");
               }

               isOngoing = true;
               GetAndTriggerNextCommand();
        }
        
        public void TryAdvanceQueue()
        {
            if (isOngoing)
            {
                UpdateCurrentCommand();
            }
        }

        void UpdateCurrentCommand()
        {
               if (currentCommand is not OngoingCommand)
               {
                    GetAndTriggerNextCommand();
                    return;
               }
               // check ongoing (current event is non-null and can be converted to OngoingCommand)
               OngoingCommand command = currentCommand as OngoingCommand;
               if (command.IsOngoing() || command.waitForExecutionEnd)
               {
                    command.UpdateCommand();
                    return;
               }

               SetPlayerLockByCurrentEventType(false);
               GetAndTriggerNextCommand();

        }

        void GetAndTriggerNextCommand()
        {
            if (commandQueue.TryDequeue(out currentCommand))
            {
                currentCommand.Trigger();
                SetPlayerLockByCurrentEventType(true);
                TryAdvanceQueue();
            }
            else
            {
                isOngoing = false;
            }
        }

        void SetPlayerLockByCurrentEventType(bool setValue)
        {
            OngoingCommand cmd = currentCommand as OngoingCommand;
            if (cmd != null && cmd.waitForExecutionEnd)
            {
                if (setValue == true)
                    PlayerLocks.EventSystemLock++;
                else
                    PlayerLocks.EventSystemLock--;
            }
        }
    }
}