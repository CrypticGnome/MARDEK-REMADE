
using UnityEngine;

namespace MARDEK.Battle
{
    using CharacterSystem;
     using Stats;
     using UnityEngine.TextCore.Text;

     public class EnemyBattleCharacter : BattleCharacter
     {
          public new CoreStats BaseStats { get { return Profile.Stats; } }
          public EnemyBattleCharacter(CharacterProfile character, Vector3 position)
          {
               Profile = character;
               Skillset = character.LearnableSkillset;
               battleModel = Object.Instantiate(Profile.BattleModelPrefab).GetComponent<BattleModel>();
               battleModel.SetBattlePosition(position);

               VolatileStats = new CoreStats(character.Stats);

               VolatileStats.MaxHP = VolatileStats.MaxHPCalc.GetMaxHP(this);
               VolatileStats.MaxMP = VolatileStats.MaxMPCalc.GetMaxMP(this); ;
               CurrentHP = VolatileStats.MaxHP;
               CurrentMP = VolatileStats.MaxMP;
          }
          
     }
}