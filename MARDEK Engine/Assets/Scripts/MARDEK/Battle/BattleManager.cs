using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MARDEK.CharacterSystem;
using MARDEK.Stats;

namespace MARDEK.Battle
{
    using Core;
     using MARDEK.Skill;
     using Progress;
    using UnityEngine.Events;

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
          public static ApplyAction ActionToPerform;
          const float actResolution = 1000;
          static public List<BattleCharacter> EnemyBattleParty { get; private set; } = new();
          static public List<BattleCharacter> PlayerBattleParty { get; private set; } = new();
          static BattleManager instance;
          BattleState state;


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
                    EnemyBattleParty.Add(new BattleCharacter(c, position));
               }
               void SpawnPlayerBattleCharacter(Character c)
               {
                    var position = playerPartyPositions[PlayerBattleParty.Count].transform.position;
                    PlayerBattleParty.Add(new BattleCharacter(c, position));
               }
          }
          private void Update()
          {
               if (state == BattleState.Idle)
               {
                    WaitForNextCharacter();
                    return;
               }

               void WaitForNextCharacter()
               {    
                    characterActing = StepActCycleTryGetNextCharacter();
                    if (characterActing == null)
                         return;
                    
                    bool characterIsEnemy = EnemyBattleParty.Contains(characterActing);
                    if (characterIsEnemy)
                    {
                         PerformEnemyMove();
                         return;
                    }

                    characterActionUI.SetActive(true);
                    state = BattleState.ChoosingAction;

               }
               void PerformEnemyMove()
               {
                    ActionSkillset enemyMoveset = characterActing.Character.ActionSkillset;
                    if (enemyMoveset is null)
                    {
                         Debug.LogWarning("Enemy moveset is null");
                         characterActing = null;
                         instance.characterActionUI.SetActive(false);
                         return;
                    }
                    Action move = enemyMoveset.Skills[Random.Range(0, enemyMoveset.Skills.Count)].Action;
                    PerformAction(move.Apply);
               }
               BattleCharacter StepActCycleTryGetNextCharacter()
               {
                    var charactersInBattle = GetCharactersInOrder();
                    AddTickRateToACT(ref charactersInBattle, Time.deltaTime);
                    var readyToAct = GetNextCharacterReadyToAct(charactersInBattle);
                    if (readyToAct != null)
                         readyToAct.Character.ACT -= actResolution;
                    return readyToAct;

                    
               }

               List<BattleCharacter> GetCharactersInOrder()
               {
                    // order by Position (p1 e1 p2 e2 p3 e3 p4 e4)
                    List<BattleCharacter> returnList = new List<BattleCharacter>();
                    for (int i = 0; i < 4; i++)
                    {
                         if (PlayerBattleParty.Count > i)
                              returnList.Add(PlayerBattleParty[i]);
                         if (EnemyBattleParty.Count > i)
                              returnList.Add(EnemyBattleParty[i]);
                    }
                    return returnList;
               }

               BattleCharacter GetNextCharacterReadyToAct(List<BattleCharacter> characters)
               {
                    float maxAct = 0;
                    foreach (var c in characters)
                    {
                         var act = c.Character.ACT;
                         if (act > maxAct)
                              maxAct = act;
                    }
                    if (maxAct < actResolution)
                         return null;

                    foreach (var c in characters)
                         if (c.Character.ACT == maxAct)
                              return c;
                    throw new System.Exception("A character had enough ACT to take a turn but wasn't returned by this method");
               }

               void AddTickRateToACT(ref List<BattleCharacter> characters, float deltatime)
               {
                    foreach (var character in characters)
                    {
                         CoreStats stats = character.Character.Profile.Stats;
                         var tickRate = 1 + 0.05f * stats.Agility;
                         tickRate *= 1000 * deltatime;
                         character.Character.ACT += (int)tickRate;
                    }
               }
          }
          public static void PerformAction(ApplyAction action)
          {

               BattleCharacter target;
               if (EnemyBattleParty.Contains(characterActing))
                    target = PlayerBattleParty[Random.Range(0, PlayerBattleParty.Count - 1)];
               else
                    target = EnemyBattleParty[Random.Range(0, EnemyBattleParty.Count - 1)];

               Debug.Log($"{characterActing.Name} targets {target.Name}");
               if (action is null)
               {
                    Debug.LogAssertion("Attempted action was null");
                    instance.EndTurn();
                    return;
               }

               instance.state = BattleState.ActionPerforming;
               action.Invoke(characterActing.Character, target.Character);

               instance.EndTurn();

               
          }
          void EndTurn()
          {
               for (int i = EnemyBattleParty.Count - 1; i >= 0; i--)
               {
                    var enemy = EnemyBattleParty[i];
                    var health = enemy.Character.CurrentHP;
                    if (health > 0)
                         continue;

                    EnemyBattleParty.Remove(enemy);
                    Destroy(enemy.battleModel.gameObject);
               }

               for (int i = PlayerBattleParty.Count - 1; i >= 0; i--)
               {
                    var hero = PlayerBattleParty[i];
                    var health = hero.Character.CurrentHP;
                    if (health <= 0)
                         PlayerBattleParty.Remove(hero);
               }
               characterActing = null;
               instance.state = BattleState.Idle;
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

          enum BattleState
          {
               Idle,
               ChoosingAction,
               ActionPerforming,
               Concluding
          }
    }
     public delegate void ApplyAction(Character user, Character target);

}