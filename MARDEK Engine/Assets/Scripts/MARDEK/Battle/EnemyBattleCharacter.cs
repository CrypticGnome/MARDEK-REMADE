
using UnityEngine;

namespace MARDEK.Battle
{
    using CharacterSystem;
     using Stats;
     using UnityEngine.TextCore.Text;

     public class EnemyBattleCharacter : BattleCharacter
     {
          public EnemyBattleCharacter(CharacterProfile character, Vector3 position)
          {
               Profile = character;
               VolatileStats = character.Stats;
               Skillset = character.LearnableSkillset;
               GameObject prefabInstance = Object.Instantiate(Profile.BattleModelPrefab);

               if (prefabInstance.TryGetComponent<BattleModelComponent>(out battleModel))
                    battleModel.SetBattlePosition(position);

               VolatileStats = new CoreStats(character.Stats);
               BaseStats.CalculateMaxValues(this);

               VolatileStats.CalculateMaxValues(this);
               CurrentHP = VolatileStats.MaxHP;
               CurrentMP = VolatileStats.MaxMP;
          }
          
     }
}