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

          [Header("Event Triggers")]
          [SerializeField] bool onStart = false;
          [SerializeField] bool onInteractionKey = false;
          [SerializeField] bool onTriggerEnter = false;
          [SerializeField] bool onTriggerExit = false;
          [SerializeField] string tagName = "Player";

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
               if (!string.IsNullOrEmpty(tagName) && tagName != string.Empty)   // Don't know why I need to see if it's empty twice, but it doesn't work with null or empty
               {
                    if (collision.gameObject.CompareTag(tagName))
                    {
                         Trigger();
                    }
                    return;
               }
               Trigger();

          }
               private void OnTriggerExit2D(Collider2D collision)
          {
               if (onTriggerExit)
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
                    Debug.Log($"Running {command} on {command.transform.parent.name}");

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
               else yield return new WaitUntil(() => ongoingCommand.IsOngoing() == false);
          }

     }
}