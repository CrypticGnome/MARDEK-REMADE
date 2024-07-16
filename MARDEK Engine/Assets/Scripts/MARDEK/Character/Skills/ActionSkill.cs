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
          [SerializeField] Battle.Action action;
          public Battle.Action Action { get { return action; } }
     }
}