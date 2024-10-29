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
          Action OnDecision = null;
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
               isOngoing = true;

               ChoicesManager.TriggerChoices(choicesDialogue);
               ChoicesManager.SetChoices(OnChoice);
          }

          void OnChoice(int index)
          {
               isOngoing = false;

               if (index >= commandsByChoice.Length)
               {
                    Debug.LogWarning("No command given for the chosen index");
                    return;
               }
               CommandChain commands = commandsByChoice[index];
               commands.Trigger();
          }
     }
}