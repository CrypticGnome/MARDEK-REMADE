using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using Core;
    using CharacterSystem;
    using MARDEK.Stats;
     using static Codice.CM.Common.CmCallContext;
     using UnityEngine.Profiling;

     public class HeroBattleCharacter : BattleCharacter
     {
          public HeroBattleCharacter(Character character, Vector3 position) : base(character, position)
          {
          }
     }
}