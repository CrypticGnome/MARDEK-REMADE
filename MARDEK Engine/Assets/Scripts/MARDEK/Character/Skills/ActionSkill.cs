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
    public class ActionSkill : Skill
     {
          [SerializeField] Battle.BattleAction action;
          [SerializeField] public ActionType TypeOfAction;
          public Battle.BattleAction Action { get { return action; } }
          public enum ActionType
          {
               Melee,
               Spell,
               ItemUsage,
          }
     }
}