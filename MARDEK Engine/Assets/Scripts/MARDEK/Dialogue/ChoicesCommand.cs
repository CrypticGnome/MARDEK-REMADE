using System.Collections.Generic;
using UnityEngine;
using MARDEK.Event;
using MARDEK.Core;
using System.Collections;
using System;

namespace MARDEK.DialogueSystem
{
    public class ChoicesCommand : Command
     {
        [SerializeField] Dialogue choicesDialogue = null;
          [SerializeField] CommandChain[] commandsByChoice;

          public bool IsOngoing { get; private set; } = false;


          [ContextMenu("Trigger")]
          public override void Trigger()
          {
               if (IsOngoing)
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
               yield return new WaitUntil(() =>
               {
                    index = ChoicesManager.GetChosenIndex();
                    return index != -1;
               });

               if (index >= commandsByChoice.Length)
                    throw new IndexOutOfRangeException();
               CommandChain commands = commandsByChoice[index];
               commands.Trigger();
          }
     }
}