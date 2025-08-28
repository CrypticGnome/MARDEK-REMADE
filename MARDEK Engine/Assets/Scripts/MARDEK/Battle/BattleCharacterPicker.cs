using MARDEK.Battle;
using MARDEK.Skill;
using MARDEK.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MARDEK.UI
{
    public class BattleCharacterPicker : MonoBehaviour
    {
          [SerializeField] Transform crystalPointerTransform;
          [SerializeField] SpriteRenderer crystalPointerRenderer;
          static List<BattleCharacter> Heroes => BattleManager.PlayerBattleParty;
          static List<BattleCharacter> Enemies => BattleManager.EnemyBattleParty;

          static int index = 0;

          public static int SelectedCharacterIndex
          {
               get { return index; }
               private set
               {
                    index = value;
                    SelectedCharacter = EnemiesSelected ? Enemies[SelectedCharacterIndex] : Heroes[SelectedCharacterIndex];
               }
          }
          public static bool EnemiesSelected;
          public static BattleCharacter SelectedCharacter { get; private set; }
          ApplyBattleAction action;


          public void HandleSelection(InputAction.CallbackContext context)
          {
               if (!enabled)
                    return;
               var value = context.ReadValue<Vector2>();
               if (value.Equals(Vector2.zero)) return;

               // Handle horizontal input
               if (value.x == -1 && !EnemiesSelected)
               {
                    EnemiesSelected = true;
                    SelectedCharacterIndex = ClampEnemiesIndex(SelectedCharacterIndex);
               }
               else if (value.x == 1 && EnemiesSelected)
               {
                    EnemiesSelected = false;
                    SelectedCharacterIndex = ClampHeroesIndex(SelectedCharacterIndex);
               }

               if (value.y == 0) return;


               // Handle verticle input
               if (value.y == 1)
                    SelectedCharacterIndex = EnemiesSelected ? ClampEnemiesIndex(SelectedCharacterIndex + 1) : ClampHeroesIndex(SelectedCharacterIndex + 1);
               else
                    SelectedCharacterIndex = EnemiesSelected ? ClampEnemiesIndex(SelectedCharacterIndex - 1) : ClampHeroesIndex(SelectedCharacterIndex - 1);
          }
          public void SelectTarget()
          {
               BattleManager.PerformActionToTarget(action, SelectedCharacter);
               enabled = false;
          }
          public void EnableWithAction(ApplyBattleAction action)
          {
               enabled = true;
               this.action = action;
          }
          private void OnDisable()
          {
               action = null;
          }
          int ClampHeroesIndex(int index) => Mathf.Clamp(index, 0, Heroes.Count - 1);
          int ClampEnemiesIndex(int index) => Mathf.Clamp(index, 0, Heroes.Count - 1);

     }
}
