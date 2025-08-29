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
          [field: SerializeField] public int Cost { get; private set; }
          [SerializeField] Battle.BattleAction action;
          public Battle.BattleAction Action { get { return action; } }
     }
}