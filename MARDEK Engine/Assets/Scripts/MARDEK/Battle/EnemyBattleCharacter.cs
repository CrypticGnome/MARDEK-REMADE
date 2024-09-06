
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
               battleModel = Object.Instantiate(Profile.BattleModelPrefab).GetComponent<BattleModel>();
               battleModel.SetBattlePosition(position);
               VolatileStats = new CoreStats(character.Stats);
               BaseStats.CalculateMaxValues(this);

               VolatileStats.MaxHP = VolatileStats.MaxHPCalc.GetMaxHP(this);
               VolatileStats.MaxMP = VolatileStats.MaxMPCalc.GetMaxMP(this); ;
               CurrentHP = VolatileStats.MaxHP;
               CurrentMP = VolatileStats.MaxMP;
          }
          
     }
}