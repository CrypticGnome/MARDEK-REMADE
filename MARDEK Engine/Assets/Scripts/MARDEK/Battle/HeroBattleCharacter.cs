using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using Core;
    using CharacterSystem;
    using MARDEK.Stats;

     public class HeroBattleCharacter : BattleCharacter
     {
          public Character Character { get; private set; }
          public int Experience { get { return Character.Experience; } private set { Character.Experience = value; } }
          public HeroBattleCharacter(Character character, Transform parent) 
          {
               Character = character;
               Level = character.Level;
               Profile = character.Profile;
               Skillset = character.ActionSkillset;
               GameObject prefabInstance = Object.Instantiate(Profile.BattleModelPrefab, parent);
               battleModel = prefabInstance.GetComponent<BattleModelComponent>();


               VolatileStats = new CoreStats(BaseStats);
               BaseStats.CalculateMaxValues(this);
               VolatileStats.CalculateMaxValues(this);

               CurrentHP = VolatileStats.MaxHP;
               CurrentMP = VolatileStats.MaxMP;

               VolatileStats.Attack = character.Attack;
               VolatileStats.Defense = character.Defense;
               VolatileStats.MagicDefense = character.MagicDefense;
          }
     }
}