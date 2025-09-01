using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Skill
{
     using Codice.CM.SEIDInfo;
     using MARDEK.Battle;
     using MARDEK.CharacterSystem;
     using MARDEK.Core;
     using MARDEK.Stats;
     using System;
     using UnityEditorInternal;

     [CreateAssetMenu(menuName = "MARDEK/Skill/ActionSkill")]
    public class ActionSkill : Skill, IBattleAction
     {
          [field: SerializeField] public int Cost { get; private set; }
          [SerializeField] BattleAction action;
          public BattleAction Action { get { return action; } }

          public Sprite ActionIcon => Action.Element.thickSprite;

          public bool TryPerformAction(BattleCharacter user, BattleCharacter target)
          {
               if (user.CurrentMP < Cost) return false;
               action.Apply(user, target);
               user.CurrentMP -= Cost;
               return true;
          }
     }
}