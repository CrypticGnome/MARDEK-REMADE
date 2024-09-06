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
          public new CoreStats BaseStats { get { return Character.BaseStats; } }
          public HeroBattleCharacter(Character character, Vector3 position) 
          {
               Character = character;
               Level = character.Level;
               Profile = character.Profile;
               Skillset = character.ActionSkillset;
               battleModel = Object.Instantiate(Profile.BattleModelPrefab).GetComponent<BattleModel>();
               battleModel.SetBattlePosition(position);

               VolatileStats = new CoreStats(BaseStats);

               int maxHP = VolatileStats.MaxHPCalc.GetMaxHP(this);
               int bob = VolatileStats.MaxHPCalc.GetMaxHP(this);
               VolatileStats.MaxHP = maxHP;
               VolatileStats.MaxMP = VolatileStats.MaxMPCalc.GetMaxMP(this); ;
               CurrentHP = character.MaxHP;
               CurrentMP = character.MaxMP;

               VolatileStats.Attack = character.Attack;
               VolatileStats.Defense = character.Defense;
               VolatileStats.MagicDefense = character.MagicDefense;
          }
     }
}