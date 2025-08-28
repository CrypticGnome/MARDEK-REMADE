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
          public HeroBattleCharacter(Character character, Vector3 position) 
          {
               Character = character;
               Level = character.Level;
               Profile = character.Profile;
               Skillset = character.ActionSkillset;
               GameObject prefabInstance = Object.Instantiate(Profile.BattleModelPrefab);
               if (prefabInstance.TryGetComponent<BattleModel>(out var battleModel))
                    battleModel.SetBattlePosition(position);
              else if (prefabInstance.TryGetComponent<BattleModelComponent>(out var battleModelComponent))
                    battleModelComponent.SetBattlePosition(position);
               else
                    prefabInstance.transform.position = position;

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