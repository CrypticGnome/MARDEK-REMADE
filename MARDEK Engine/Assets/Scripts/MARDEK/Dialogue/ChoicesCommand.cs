using System.Collections.Generic;
using UnityEngine;
using MARDEK.Event;
using MARDEK.Core;
using System.Collections;
using System;

namespace MARDEK.DialogueSystem
{
    public class ChoicesCommand : OngoingCommand
     {
        [SerializeField] Dialogue choicesDialogue = null;
          [SerializeField] CommandChain[] commandsByChoice;

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
               ChoicesManager.TriggerChoices(choicesDialogue);
               StartCoroutine(WaitForChoice());

          }
          IEnumerator WaitForChoice()
          {
               // Currently, the choosing is done through the choice manager and we wait until it gives a value not equal to the default.
               // This is dogshit, but currently no reason to rework it.
               int index = -1;
               isOngoing = true;
               yield return new WaitUntil(() =>
               {
                    index = ChoicesManager.GetChosenIndex();
                    return index != -1;
               });
               isOngoing = false;

               if (index >= commandsByChoice.Length)
                    throw new IndexOutOfRangeException();
               CommandChain commands = commandsByChoice[index];
               commands.Trigger();
          }
     }
}