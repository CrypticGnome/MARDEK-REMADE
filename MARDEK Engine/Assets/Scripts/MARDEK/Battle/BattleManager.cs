using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MARDEK.CharacterSystem;
using MARDEK.Save;
using UnityEngine;

namespace MARDEK.Battle
{
	using MARDEK.Skill;
	using MARDEK.UI;
	using Progress;


	public class BattleManager : MonoBehaviour
	{
		[SerializeField] PartySO playerParty;
		[SerializeField] GameObject characterActionUI = null;
		[SerializeField] List<GameObject> enemyPartyPositions = new();
		[SerializeField] List<GameObject> playerPartyPositions = new();
		[SerializeField] EncounterSet dummyEncounter;
		[SerializeField] BattleCharacterPicker characterPicker;
		[SerializeField] ActionDisplay actionDisplay;
		public static Encounter Encounter;
		public static EncounterSet encounter { private get; set; }
		public static BattleCharacter characterActing { get; private set; }
		public static BattleAction ActionToPerform;
		static public List<BattleCharacter> EnemyBattleParty { get; private set; } = new();
		static public List<BattleCharacter> PlayerBattleParty { get; private set; } = new();
		public static BattleManager instance;
		public delegate void TurnEnd();
		public static event TurnEnd OnTurnEnd;
		public delegate void TurnStart();
		public static event TurnEnd OnTurnStart;
		readonly BattleStateMachine stateMachine = new();
		public BattleState state => stateMachine.CurrentState;


		private void Awake()
		{
			instance = this;
			if (!encounter) encounter = dummyEncounter;
			InstantiateEncounter();
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
		void InstantiateEncounter()
		{
			List<Character> enemyCharacters = encounter.InstantiateEncounter(out Encounter);

			EnemyBattleParty.Clear();
			for (int i = 0; i < enemyCharacters.Count; i++)
			{
				EnemyBattleCharacter enemyCharacter = new EnemyBattleCharacter(enemyCharacters[i], enemyPartyPositions[i].transform);
				EnemyBattleParty.Add(enemyCharacter);
			}
			PlayerBattleParty.Clear();
			for (int i = 0; i < playerParty.Count; i++)
			{
				HeroBattleCharacter playerCharacter = new HeroBattleCharacter(playerParty[i], playerPartyPositions[i].transform);
				PlayerBattleParty.Add(playerCharacter);
			}

			foreach (BattleCharacter enemy in EnemyBattleParty)
				if (enemy is EnemyBattleCharacter enemyCharacter)
					enemyCharacter.ChooseNextAction(PlayerBattleParty);
		}
		void SetInitialACT()
		{
			// Maybe have it so that if you press interact quickly your party gets an initial act bonus?
			// Sort of like how currently if you press x currentyl you can just skip the battle
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
			CompressListToCap(tempACT);

			listIndex = 0;
			allCharacters.ForEach(character => character.ACT = tempACT[listIndex++]);


			void AddCharacterTime(BattleCharacter character, bool surprised)
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
					float temp_act = minTime / timeToTurn * TurnManager.ActResolution;
					temp_act += Random.Range(-167, 167);
					tempACT.Add(temp_act);
				}
			}
			void NormalizeBottomToZero(List<float> input)
			{
				float minValue = input.Min();

				for (int i = 0; i < allCharacters.Count; i++) tempACT[i] -= minValue;
			}
			void CompressListToCap(List<float> input)
			{
				float maxValue = input.Max();
				float compressionFactor = TurnManager.ActResolution / maxValue;

				for (int i = 0; i < allCharacters.Count; i++) tempACT[i] *= compressionFactor;
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
				if (instance.state == BattleState.Concluding)
				{
					yield break;
				}


				characterActing = nextActor;
				characterActing.ACT -= TurnManager.ActResolution;


				OnTurnStart?.Invoke();
				if (characterActing.stunned)
				{
					Debug.Log($"{characterActing.Name} is stunned");
					characterActing.TickStatusEffects();
					instance.EndTurn();
					yield break;
				}

				if (EnemyBattleParty.Contains(characterActing))
				{
					PerformEnemyMove();
					yield break;
				}

				characterActionUI.SetActive(true);
				stateMachine.TrySetState(BattleState.ChoosingAction);
				characterActing.TickStatusEffects();
			}

			void PerformEnemyMove()
			{
				characterActing.TickStatusEffects();

				if (characterActing is not EnemyBattleCharacter enemyCharacter)
				{
					Debug.LogError($"{characterActing.Name} is acting as an enemy but isn't an EnemyBattleCharacter");
					characterActing = null;
					instance.characterActionUI.SetActive(false);
					instance.EndTurn();
					return;
				}

				ActionSkill skill = enemyCharacter.NextAction;
				// The queued target may have died since it was chosen, so re-pick if needed.
				BattleCharacter target = enemyCharacter.NextTarget != null && enemyCharacter.NextTarget.CurrentHP > 0
					? enemyCharacter.NextTarget
					: PlayerBattleParty.Where(hero => hero.CurrentHP > 0).OrderBy(_ => Random.value).FirstOrDefault();

				if (skill is null || target is null)
				{
					characterActing = null;
					instance.characterActionUI.SetActive(false);
					instance.EndTurn();
					return;
				}

				Debug.Log($"{characterActing.Name} uses {skill.DisplayName}");
				PerformActionToTarget(skill, target);
				instance.actionDisplay.DisplayAction(skill);

				// Immediately queue up this enemy's following attack so one is always ready to show ahead of time.
				enemyCharacter.ChooseNextAction(PlayerBattleParty);
			}
		}
		public static void PerformActionToTarget(IBattleAction action, BattleCharacter target)
		{
			if (action is null)
			{
				Debug.LogAssertion("Attempted action was null");
				instance.EndTurn();
				return;
			}

			instance.stateMachine.TrySetState(BattleState.ActionPerforming);
			var attacker = characterActing;
			instance.StartCoroutine(PlayAttack());

			IEnumerator PlayAttack()
			{
				var attackerModel = attacker.battleModel;
				var targetModel = target.battleModel;

				void ApplyAction() => action.TryPerformAction(attacker, target);

				if (action is ActionSkill skill && attackerModel != null)
					yield return attackerModel.PlayAction(skill, targetModel, ApplyAction);
				else
				{
					ApplyAction();
					yield return new WaitForSeconds(1.5f);
				}
				instance.EndTurn();
			}
		}

		void EndTurn() => StartCoroutine(EndTurnRoutine());

		IEnumerator EndTurnRoutine()
		{
			var deadEnemies = EnemyBattleParty.Where(enemy => enemy.CurrentHP <= 0).ToList();
			foreach (var enemy in deadEnemies)
				EnemyBattleParty.Remove(enemy);

			var deathRoutines = deadEnemies.Select(enemy => StartCoroutine(PlayDeathThenDestroy(enemy))).ToList();
			foreach (var deathRoutine in deathRoutines)
				yield return deathRoutine;

			for (int i = PlayerBattleParty.Count - 1; i >= 0; i--)
			{
				BattleCharacter hero = PlayerBattleParty[i];
				int health = hero.CurrentHP;
				if (health <= 0)
				{
					//die
				}
			}
			characterActing = null;
			instance.stateMachine.TrySetState(BattleState.Idle);
			OnTurnEnd?.Invoke();
			instance.characterActionUI.SetActive(false);
			instance.CheckBattleEnd();
		}

		IEnumerator PlayDeathThenDestroy(BattleCharacter enemy)
		{
			yield return enemy.battleModel.PlayDeathSequence();
			Destroy(enemy.battleModel.gameObject);
		}
		public void SkipCurrentCharacterTurn() => EndTurn();

		void CheckBattleEnd()
		{
			// heroes stay in PlayerBattleParty at 0 HP (unlike dead enemies, which are removed), so
			// defeat is "everyone down" rather than an empty list
			bool defeat = PlayerBattleParty.Count > 0 && PlayerBattleParty.All(hero => hero.CurrentHP <= 0);
			instance.characterActionUI.SetActive(false);
			if (defeat)
			{
				instance.stateMachine.TrySetState(BattleState.Concluding);
				StartCoroutine(Defeat());
			}
			var victory = EnemyBattleParty.Count == 0;
			if (victory)
			{
				instance.stateMachine.TrySetState(BattleState.Concluding);
				StartCoroutine(Victory());
			}
		}

		IEnumerator Defeat()
		{
			print("defeat!!");
			yield return new WaitForSeconds(1);
			BattleUIManager.Instance.OnDefeat();

			string lastSavedFile = PlayerPrefs.GetString("lastSavedFile", string.Empty);
			if (string.IsNullOrEmpty(lastSavedFile))
			{
				Debug.LogWarning("Party was defeated but no save file exists to reload");
				yield break;
			}
			SaveSystem.CallGameFileLoaderScene(lastSavedFile);
		}

		IEnumerator Victory()
		{
			print("victory!!");
			yield return new WaitForSeconds(1);
			BattleUIManager.Instance.OnVictory();

			for (int i = 0; i < playerParty.Count; i++)
			{
				if (playerParty[i] == null) continue;

				playerParty[i].CurrentHP = PlayerBattleParty[i].CurrentHP;
				playerParty[i].CurrentMP = PlayerBattleParty[i].CurrentMP;
			}
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
}
