using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.CharacterSystem;
using MARDEK.Stats;
using System.Linq;

namespace MARDEK.Battle
{
    using Core;
     using MARDEK.Skill;
     using Progress;
    using UnityEngine.Events;
     using static MARDEK.Battle.BattleManager;

    public class BattleManager : MonoBehaviour
    {
          [SerializeField] Party playerParty;
          [SerializeField] List<Character> DummyEnemies;
          [SerializeField] GameObject characterActionUI = null;
          [SerializeField] List<GameObject> enemyPartyPositions = new();
          [SerializeField] List<GameObject> playerPartyPositions = new();
          [SerializeField] UnityEvent OnVictory;
          [SerializeField] EncounterSet dummyEncounter;
          public static EncounterSet encounter { private get; set; }
          public static BattleCharacter characterActing { get; private set; }
          public static BattleAction ActionToPerform;
          const float actResolution = 1000;
          static public List<BattleCharacter> EnemyBattleParty { get; private set; } = new();
          static public List<BattleCharacter> PlayerBattleParty { get; private set; } = new();
          public static BattleManager instance;
          public delegate void TurnEnd();
          public static event TurnEnd OnTurnEnd;
          public delegate void TurnStart();
          public static event TurnEnd OnTurnStart;
          public BattleState state;


          private void Awake()
          {
               instance = this;
               if (!encounter)
               {
                    if (!dummyEncounter)
                    {
                         Debug.LogAssertion("Encounter is null");
                         CheckBattleEnd();
                         return;
                    }
                    encounter = dummyEncounter;
               }
               List<Character> enemyCharacters = encounter.InstantiateEncounter();

               EnemyBattleParty.Clear();
               foreach (var c in enemyCharacters)
                    SpawnEnemyBattleCharacter(c);

               PlayerBattleParty.Clear();
               foreach (var c in playerParty.Characters)
                    SpawnPlayerBattleCharacter(c);

               state = BattleState.Idle;

               void SpawnEnemyBattleCharacter(Character c)
               {
                    var position = enemyPartyPositions[EnemyBattleParty.Count].transform.position;
                    EnemyBattleParty.Add(new EnemyBattleCharacter(c.Profile, position));
               }
               void SpawnPlayerBattleCharacter(Character c)
               {
                    var position = playerPartyPositions[PlayerBattleParty.Count].transform.position;
                    PlayerBattleParty.Add(new HeroBattleCharacter(c, position));
               }
          }
          private void Start()
          {
               OnTurnEnd += WaitForTurn;
               SetInitialACT();
               OnTurnEnd?.Invoke();
          }
          private void OnDisable()
          {
               OnTurnEnd -= WaitForTurn;
          }

          void SetInitialACT()
          {
               bool partySurprised = false;
               List<float> timesToTurn = new List<float>();
               List<BattleCharacter> allCharacters = new List<BattleCharacter>();
               allCharacters.AddRange(EnemyBattleParty);
               allCharacters.AddRange(PlayerBattleParty);

               foreach (BattleCharacter character in EnemyBattleParty)
                    AddCharacterTime(character, !partySurprised);
               foreach (BattleCharacter character in PlayerBattleParty)
                    AddCharacterTime(character, partySurprised);

               float minTime = timesToTurn.Min();
               int listIndex = 0;
               GetTempACT(out List<float> tempACT);
               NormalizeBottomToZero(tempACT);
               CompressListToCap(tempACT, actResolution); 

               listIndex = 0;
               allCharacters.ForEach(character => character.ACT = tempACT[listIndex++]);


               void AddCharacterTime (BattleCharacter character, bool surprised)
               {
                    float speedMultiplier = surprised ? 1 : 2;
                    speedMultiplier *= Random.Range(0.9f, 1.1f);
                    float timeToTurn = TurnManager.TimeToTurn(character, speedMultiplier);
                    timesToTurn.Add(timeToTurn);
               }
               void GetTempACT(out List<float> tempACT)
               {
                    tempACT = new List<float>();

                    foreach (BattleCharacter battleCharacter in allCharacters)
                    {
                         float timeToTurn = timesToTurn[listIndex++];
                         float temp_act = minTime / timeToTurn * actResolution;
                         temp_act += Random.Range(-167, 167);
                         tempACT.Add(temp_act);
                    }
               }
               void NormalizeBottomToZero(List<float> input)
               {
                    float minValue = input.Min();

                    for (int i = 0; i < allCharacters.Count; i++) 
                    {
                         tempACT[i] -= minValue;
                    }
               }
               void CompressListToCap(List<float> input, float cap)
               {
                    float maxValue = input.Max();
                    float compressionFactor = cap / maxValue;

                    for (int i = 0; i < allCharacters.Count; i++)
                    {
                         tempACT[i] *= compressionFactor;
                    }
               }
          }

          void WaitForTurn()
          {
               StartCoroutine(WaitForNextTurn());
               IEnumerator WaitForNextTurn()
               {
                    TurnManager.GetTimeToNextTurn(out float timeToTurn, out BattleCharacter nextActor);
                    TurnManager.GetCharacterACTNextTurn(timeToTurn, out List<float> startACT, out List<float> finalACT);
                    IEnumerator lerpCharacterACT = TurnManager.LerpCharacterACTs(timeToTurn, startACT, finalACT);
                    yield return StartCoroutine(lerpCharacterACT);

                    characterActing = nextActor;
                    characterActing.ACT -= actResolution;
                    OnTurnStart?.Invoke();

                    if (EnemyBattleParty.Contains(characterActing))
                    {
                         PerformEnemyMove();
                         yield break;
                    }

                    characterActionUI.SetActive(true);
                    state = BattleState.ChoosingAction;
                    characterActing.TickStatusEffects();
               }

               void PerformEnemyMove()
               {
                    characterActing.TickStatusEffects();

                    ActionSkillset enemyMoveset = characterActing.Skillset;
                    if (enemyMoveset is null)
                    {
                         Debug.LogWarning("Enemy moveset is null");
                         characterActing = null;
                         instance.characterActionUI.SetActive(false);
                         return;
                    }
                    ActionSkill skill = enemyMoveset.Skills[Random.Range(0, enemyMoveset.Skills.Count)];
                    BattleAction move = skill.Action;
                    Debug.Log($"{characterActing.Name} uses {skill.DisplayName}");
                    PerformAction(move.Apply);
               }
          }
          public static void PerformAction(ApplyBattleAction action)
          {

               BattleCharacter target;
               if (EnemyBattleParty.Contains(characterActing))
                    target = PlayerBattleParty[Random.Range(0, PlayerBattleParty.Count - 1)];
               else
                    target = EnemyBattleParty[Random.Range(0, EnemyBattleParty.Count - 1)];

               if (action is null)
               {
                    Debug.LogAssertion("Attempted action was null");
                    instance.EndTurn();
                    return;
               }

               instance.state = BattleState.ActionPerforming;
               action.Invoke(characterActing, target);

               instance.EndTurn();

               
          }
          void EndTurn()
          {
               for (int i = EnemyBattleParty.Count - 1; i >= 0; i--)
               {
                    var enemy = EnemyBattleParty[i];
                    var health = enemy.CurrentHP;
                    if (health > 0)
                         continue;

                    EnemyBattleParty.Remove(enemy);
                    Destroy(enemy.battleModel.gameObject);
               }

               for (int i = PlayerBattleParty.Count - 1; i >= 0; i--)
               {
                    BattleCharacter hero = PlayerBattleParty[i];
                    int health = hero.CurrentHP;
                    if (health <= 0)
                         PlayerBattleParty.Remove(hero);
               }
               characterActing = null;
               instance.state = BattleState.Idle;
               OnTurnEnd?.Invoke();
               instance.characterActionUI.SetActive(false);
               instance.CheckBattleEnd();
          }
          public void SkipCurrentCharacterTurn()
          {
               EndTurn();
          }
    
          void CheckBattleEnd()
          {
               bool defeat = PlayerBattleParty.Count == 0;
               instance.characterActionUI.SetActive(false);
               if (defeat)
               {
                    print("defeat!!");
                    Debug.LogAssertion("Defeat not implemented yet");
                    instance.state = BattleState.Concluding;
               }
               var victory = EnemyBattleParty.Count == 0;
               if (victory)
               {
                    instance.state = BattleState.Concluding;
                    StartCoroutine(Victory());
               }
          }

          IEnumerator Victory()
          {
               print("victory!!");
               yield return new WaitForSeconds(2);
               instance.OnVictory.Invoke();
               instance.enabled = false;
          }

          public enum BattleState
          {
               Idle,
               ChoosingAction,
               ActionPerforming,
               Concluding
          }
    }
     //public delegate void ApplyAction(Character target);
     public delegate void ApplyBattleAction(BattleCharacter user, BattleCharacter target);

}