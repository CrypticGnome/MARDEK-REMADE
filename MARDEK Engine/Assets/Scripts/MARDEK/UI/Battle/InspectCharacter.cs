using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MARDEK.Battle;
namespace MARDEK.UI
{
    public class InspectCharacter : MonoBehaviour
    {
          [Header("Gneral Data")]
          [SerializeField] TMP_Text characterName;
          [SerializeField] TMP_Text levelText;
          [Header("Stats")]
          [SerializeField] TMP_Text strength;
          [SerializeField] TMP_Text vitality;
          [SerializeField] TMP_Text spirit;
          [SerializeField] TMP_Text agility;

          [SerializeField] TMP_Text attack;
          [SerializeField] TMP_Text defense;
          [SerializeField] TMP_Text magicDefense;
          [SerializeField] TMP_Text evasion;
          private void OnEnable()
          {
               Inspect();
          }
          public void Inspect()
          {
               BattleCharacter character = BattleUIManager.Instance.characterBeingInspected;
               characterName.text = character.Profile.displayName;
               levelText.text = $"Level {character.Level}";
               strength.text = character.Strength.ToString();
               vitality.text = character.Vitality.ToString();
               spirit.text = character.Spirit.ToString();
               agility.text = character.Agility.ToString();
               attack.text = character.Attack.ToString();
               defense.text = character.Defense.ToString();
               magicDefense.text = character.MagicDefense.ToString();
               evasion.text = "Not Implemented";
          }
    }
}