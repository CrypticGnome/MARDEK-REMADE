using MARDEK.CharacterSystem;
using MARDEK.Skill;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using static MARDEK.Battle.BattleManager;
namespace MARDEK.Battle
{
     public class TurnManager
     {
          public static float MaxTimeBetweenTurns = 2f;
          public const float ActResolution = 1000f;
           public static IEnumerator LerpCharacterACTs(float timeToTurn, List<float> startACT, List<float> finalACT)
           {
                if (timeToTurn <= 0)
                     yield break;
                float timer = 0;
                float waitCompletion = 0;
                while (waitCompletion < 1)
                {
                    if (instance.state == BattleState.Concluding)
                    {
                         yield break;
                    }
                    int listIndex = 0;
                     waitCompletion = Mathf.Clamp01(timer / timeToTurn);
                     foreach (BattleCharacter character in EnemyBattleParty)
                     {
                          character.ACT = Mathf.Lerp(startACT[listIndex], finalACT[listIndex], waitCompletion);
                          listIndex++;
                     }
                     foreach (BattleCharacter character in PlayerBattleParty)
                     {
                          character.ACT = Mathf.Lerp(startACT[listIndex], finalACT[listIndex], waitCompletion);
                          listIndex++;
                     }
                     yield return null;
                     timer += Time.deltaTime;
                }
           }
           public static void GetTimeToNextTurn(out float timeToTurn, out BattleCharacter nextActor)
           {
                List<float> timeToTurnList = new List<float>();
                List<BattleCharacter> characterList = new List<BattleCharacter>();
                foreach (BattleCharacter character in EnemyBattleParty)
                {
                    float characterTimeToTurn = TimeToTurn(character, 1);
                     timeToTurnList.Add(characterTimeToTurn);
                     characterList.Add(character);
                }
                foreach (BattleCharacter character in PlayerBattleParty)
                {
                    float characterTimeToTurn = TimeToTurn(character, 1);
                    timeToTurnList.Add(characterTimeToTurn);
                     characterList.Add(character);
                }
                timeToTurn = timeToTurnList.Min();
                int minIndex = timeToTurnList.IndexOf(timeToTurn);
                nextActor = characterList[minIndex];
           }
           public static void GetCharacterACTNextTurn(float timeToTurn, out List<float> startACT, out List<float> finalACT)
           {
                startACT = new List<float>();
                finalACT = new List<float>();
                foreach (BattleCharacter character in EnemyBattleParty)
                {
                     startACT.Add(character.ACT);

                    float actRate = character.ActBuildRate();
                    float characterACT = character.ACT + actRate * timeToTurn / MaxTimeBetweenTurns;
                     finalACT.Add(characterACT);
                }
                foreach (BattleCharacter character in PlayerBattleParty)
                {
                     startACT.Add(character.ACT);

                    float actRate = character.ActBuildRate();
                    float characterACT = character.ACT + actRate * timeToTurn / MaxTimeBetweenTurns;
                     finalACT.Add(characterACT);
                }
           }
           public static float TimeToTurn(BattleCharacter character, float speedMultiplier)
           {
               float actRate = character.ActBuildRate();
               actRate *= speedMultiplier;
               float characterTimeToTurn = (ActResolution - character.ACT) / actRate * MaxTimeBetweenTurns;
               return characterTimeToTurn;
          }
      }
     
}