using MARDEK.CharacterSystem;
using MARDEK.Inventory;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace MARDEK.UI
{
     using MARDEK.Battle;
     using Progress;
    public class BattleLoot : MonoBehaviour
    {
          List<Item> currentItems = new List<Item>(4);
          List<int> currentAmounts = new List<int>(4);

        // TODO Remove after testing

        [SerializeField] List<BattleLootEntry> entries;
        [SerializeField] List<InventorySpace> inventorySpaces;

          void Start()
          {
               currentItems.Clear();
               currentAmounts.Clear();
               SetLoot(currentItems, currentAmounts);
          }

          void Reset()
          {
               currentItems.Clear();
               currentAmounts.Clear();
          }
          public void SetLoot(List<Item> items, List<int> amounts)
          {
               Encounter encounter = BattleManager.instance.Encounter;
               items.AddRange(encounter.UniqueRewards);
               for (int index = 0; index < encounter.Enemies.Length; index++)
               {
                    Character character = encounter.Enemies[index].enemy;
                    if (!(character is CharacterUnplayable))
                         continue;
                    CharacterUnplayable.ItemDrop[] itemDrops = ((CharacterUnplayable)character).Drops;
                    for (int drop = 0; drop < itemDrops.Length; drop++) 
                    {
                         if (Random.value > itemDrops[drop].Chance)
                              continue;
                         if (items.Contains(itemDrops[drop].Item))
                         {
                              int itemIndex = items.IndexOf(itemDrops[drop].Item);
                              amounts[itemIndex]++;
                              continue;
                         }
                         items.Add(itemDrops[drop].Item);
                         amounts.Add(1);
                    }
               }

               for (int index = 0; index < entries.Count; index++)
               {
                    entries[index].SetItem(index, currentItems.ToArray(), currentAmounts.ToArray());
               }

               UpdateInventorySpaceGrids();
          }
          public void SetLoot0(List<Item> items, List<int> amounts)
          {
               if (items.Count != amounts.Count) throw new System.ArgumentException("#items must be equal to #amounts");
               Reset();

               for (int index = 0; index < items.Count; index++) {
                    currentItems[index] = items[index];
                    currentAmounts[index] = amounts[index];
                    entries[index].gameObject.SetActive(true);
               }

               for (int index = 0; index < entries.Count; index++) {
                    entries[index].SetItem(index, currentItems.ToArray(), currentAmounts.ToArray()) ;
               }

               UpdateInventorySpaceGrids();
          }

        void UpdateInventorySpaceGrids()
        {
            for (int index = 0; index < inventorySpaces.Count; index++)
            {
                if (index < Party.Instance.Characters.Count)
                {
                    inventorySpaces[index].gameObject.SetActive(true);
                    inventorySpaces[index].UpdateImage(Party.Instance.Characters[index].Inventory);
                }
                else inventorySpaces[index].gameObject.SetActive(false);
            }
        }

        void UpdateLoot()
        {
            List<Item> newItems = new List<Item>();
            List<int> newAmounts = new List<int>();

            for (int index = 0; index < currentItems.Count; index++)
            {
                if (currentAmounts[index] != 0)
                {
                    newItems.Add(currentItems[index]);
                    newAmounts.Add(currentAmounts[index]);
                }
            }

            SetLoot(newItems, newAmounts);
        }

        public void Interact()
        {
            BattleLootSelectable.currentlySelected.Interact(currentItems.ToArray(), currentAmounts.ToArray());
            UpdateLoot();
        }
    }
}
