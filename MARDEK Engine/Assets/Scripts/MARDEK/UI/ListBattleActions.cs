using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.UI
{
    using Core;
    using MARDEK.CharacterSystem;
     public abstract class ListBattleActions : MonoBehaviour
     {
        [SerializeField] SelectableLayout listUI;
        List<BattleActionUI> actionSlotUIs = new();
        int index = 0;

        private void Awake()
        {
            actionSlotUIs = new List<BattleActionUI>(listUI.GetComponentsInChildren<BattleActionUI>());
        }

        protected void ClearSlots()
        {
            foreach (var slot in actionSlotUIs)
                slot.SetSlot(null);
            index = 0;
        }

        protected void SetNextSlot(BattleActionSlot action)
        {
            actionSlotUIs[index].SetSlot(action);
            index++;
        }

        public void UpdateLayout()
        {
            listUI.UpdateSelectionAtIndex(false);
        }
    }
}
