using MARDEK.Battle;
using MARDEK.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
    public class AttackButton : MonoBehaviour
    {
          [SerializeField] ActionSkill attack;
          [SerializeField] BattleCharacterPicker targetPicker;
          private void OnEnable()
          {
               // Set sprite to character specific attack sprite
          }
          public void SelectAction()
        {
               targetPicker.EnableWithAction(attack.Action.Apply);
        }
    }
}