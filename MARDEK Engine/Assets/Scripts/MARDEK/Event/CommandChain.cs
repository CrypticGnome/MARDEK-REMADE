using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MARDEK.Core;

namespace MARDEK.Event
{
     public class CommandChain : Command
     {
          [SerializeField] Command[] commands;

          [Header("Event Triggers")]
          [SerializeField] bool onStart = false;
          [SerializeField] bool onInteractionKey = false;
          [SerializeField] bool onTriggerEnter = false;


#if UNITY_EDITOR
          [Header("Debugging")]
          [SerializeField] bool raiseEvent;
#endif
          public bool IsOngoing { get; private set; } = false;

          void Start()
          {
               if (onStart) 
                    Trigger();
          }

          private void OnTriggerEnter2D(Collider2D collision)
          {
               if (onTriggerEnter)
                    if (collision.gameObject.CompareTag("Player"))
                         Trigger();
          }
          public void Interact()
          {
               if (onInteractionKey)
                    Trigger();
          }


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

                    ///GetAndTriggerNextCommand()
                    command.Trigger();
                    // Lock shit up and UpdateCurrentCommand()
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
}