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
          public new int Level { get { return Character.Level; } private set { Character.Level = value; } }
          public int Experience { get { return Character.Experience; } private set { Character.Experience = value; } }
          public new CoreStats BaseStats { get { return Character.BaseStats; } }
          public HeroBattleCharacter(Character character, Vector3 position) 
          {
               Character = character;
               Profile = character.Profile;
               Skillset = character.ActionSkillset;
               battleModel = Object.Instantiate(Profile.BattleModelPrefab).GetComponent<BattleModel>();
               battleModel.SetBattlePosition(position);
               VolatileStats = new CoreStats(BaseStats);

               CurrentHP = character.MaxHP;
               CurrentMP = character.MaxMP;
          }
     }
}