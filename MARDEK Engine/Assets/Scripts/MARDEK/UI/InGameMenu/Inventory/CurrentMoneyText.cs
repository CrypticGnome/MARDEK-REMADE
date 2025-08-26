using MARDEK.CharacterSystem;
using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
     using MARDEK.Inventory;
     using Progress;

    public class CurrentMoneyText : MonoBehaviour
    {
         [SerializeField] TMP_Text text;
          [SerializeField] InventorySO inventory;
        void FixedUpdate()
        {
               text.text = inventory.Money.ToString();
        }
    }
}
