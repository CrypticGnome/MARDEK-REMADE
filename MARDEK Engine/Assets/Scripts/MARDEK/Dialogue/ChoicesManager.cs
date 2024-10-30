using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MARDEK.DialogueSystem
{
    using EventSystem = UnityEngine.EventSystems.EventSystem;

    public class ChoicesManager : MonoBehaviour
    {
        static ChoicesManager instance;

        [SerializeField] GameObject canvas = null;
        [SerializeField] RectTransform layoutGroup = null;
        [SerializeField] List<GameObject> choicesUIObjects = new List<GameObject>();
        static Action<int> choiceDecision = null;

        private void Awake()
        {
            instance = this;
        }

        public static void TriggerChoices(Dialogue dialogue)
        {
            instance.canvas.SetActive(true);
            instance.StartCoroutine(instance.SetupChoices(dialogue));
        }

        IEnumerator SetupChoices(Dialogue dialogue)
        {
            for(int i = 0; i < dialogue.CharacterLines[0].WrappedLines.Count; i++)
            {
                var text = dialogue.CharacterLines[0].WrappedLines[i].line;
                choicesUIObjects[i].GetComponent<Text>().text = text;
                choicesUIObjects[i].SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            yield return new WaitForEndOfFrame();
            choicesUIObjects[0].GetComponent<Button>().Select();
        }
        
        public void Choose(int index) 
        {
               choiceDecision?.Invoke(index);
               ResetChoiceManager();
        }
          public static void SetChoices(Action<int> action)
          {
               choiceDecision = action;
          }

        public static void ResetChoiceManager()
        {
            foreach (var choice in instance.choicesUIObjects)
                choice.SetActive(false);
            instance.canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}