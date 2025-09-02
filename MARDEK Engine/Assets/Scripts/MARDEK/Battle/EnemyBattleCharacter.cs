
using UnityEngine;

namespace MARDEK.Battle
{
    using CharacterSystem;
     using Stats;

     public class EnemyBattleCharacter : BattleCharacter
     {
          public EnemyBattleCharacter(Character character, Transform parent)
          {
               Profile = character.Profile;
               VolatileStats = Profile.Stats;
               Skillset = Profile.LearnableSkillset;
               GameObject prefabInstance = Object.Instantiate(Profile.BattleModelPrefab, parent);
               battleModel = prefabInstance.GetComponent<BattleModelComponent>();
               Level = character.Level;
               VolatileStats = new CoreStats(Profile.Stats);
               BaseStats.CalculateMaxValues(this);

               VolatileStats.CalculateMaxValues(this);
               CurrentHP = VolatileStats.MaxHP;
               CurrentMP = VolatileStats.MaxMP;
          }
          
     }
}