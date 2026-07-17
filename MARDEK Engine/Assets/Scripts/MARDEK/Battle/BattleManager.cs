using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MARDEK.Audio;
using MARDEK.CharacterSystem;
using MARDEK.Inventory;
using MARDEK.Save;
using UnityEngine;
using MARDEK.UI;
using MARDEK.Progress;

namespace MARDEK.Battle
{
	public class BattleManager : MonoBehaviour
	{
		[SerializeField] PartySO playerParty;
		[SerializeField] InventorySO inventory;
		[SerializeField] GameObject characterActionUI = null;
		[SerializeField] List<GameObject> enemyPartyPositions = new();
		[SerializeField] List<GameObject> playerPartyPositions = new();
		[SerializeField] EncounterSet dummyEncounter;
		[SerializeField] BattleCharacterPicker characterPicker;
		[SerializeField] ActionDisplay actionDisplay;
		[SerializeField] AudioClip victoryFanfareStandard;
		[SerializeField] AudioClip victoryFanfareGrand;
		[SerializeField] AudioClip gameOverJingle;
		public static Encounter Encounter;
		public static EncounterSet encounter { private get; set; }
		public static BattleCharacter characterActing { get; private set; }
		public static BattleAction ActionToPerform;
		// Gold rewarded by the most recently concluded victory - read by BattleLoot to
		// display the amount alongside item drops.
		public static int GoldGained { get; private set; }
		static public List<EnemyBattleCharacter> EnemyBattleParty { get; private set; } = new();
		static public List<HeroBattleCharacter> PlayerBattleParty { get; private set; } = new();
		// Enemies first, then heroes - TurnManager relies on this order staying consistent
		// across calls when building its parallel ACT lists.
		public static IEnumerable<BattleCharacter> AllCombatants => EnemyBattleParty.Concat<BattleCharacter>(PlayerBattleParty);
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
				EnemyBattleParty.Add(SpawnBattleCharacter<EnemyBattleCharacter>(enemyCharacters[i], enemyPartyPositions[i].transform));

			PlayerBattleParty.Clear();
			for (int i = 0; i < playerParty.Count; i++)
				PlayerBattleParty.Add(SpawnBattleCharacter<HeroBattleCharacter>(playerParty[i], playerPartyPositions[i].transform));

			foreach (var enemy in EnemyBattleParty)
				enemy.ChooseNextAction(PlayerBattleParty);
		}

		// Battle-model prefabs carry their BattleCharacter component (added by the
		// Custom/Ensure Battle Character Components menu item); the runtime AddComponent is
		// only a fallback for prefabs that haven't been set up yet.
		static T SpawnBattleCharacter<T>(Character character, Transform positionSlot) where T : BattleCharacter
		{
			GameObject prefabInstance = Instantiate(character.Profile.BattleModelPrefab, positionSlot);
			if (!prefabInstance.TryGetComponent(out T battleCharacter))
			{
				Debug.LogWarning($"{character.Profile.displayName}'s battle model prefab has no {typeof(T).Name} - adding one at runtime. Run \"Custom/Ensure Battle Character Components On Battle Models\" to fix the prefab.", prefabInstance);
				battleCharacter = prefabInstance.AddComponent<T>();
			}
			battleCharacter.LoadCharacter(character);
			return battleCharacter;
		}
		void SetInitialACT()
		{
			// Maybe have it so that if you press interact quickly your party gets an initial act bonus?
			// Sort of like how currently if you press x currentyl you can just skip the battle
			bool partySurprised = false;
			List<float> timesToTurn = new List<float>();
			List<BattleCharacter> allCharacters = AllCombatants.ToList();

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
				// No living combatant left to act - the battle is concluding
				if (nextActor == null)
					yield break;
				TurnManager.GetCharacterACTNextTurn(timeToTurn, out List<float> startACT, out List<float> finalACT);
				IEnumerator lerpCharacterACT = TurnManager.LerpCharacterACTs(timeToTurn, startACT, finalACT);
				yield return StartCoroutine(lerpCharacterACT);
				if (instance.state == BattleState.Concluding)
				{
					yield break;
				}


				characterActing = nextActor;
				bool canAct = characterActing.BeginTurn();

				OnTurnStart?.Invoke();

				if (!canAct)
				{
					instance.EndTurn();
					yield break;
				}

				yield return characterActing.TakeAction();
			}
		}
		public static void EndCurrentTurn() => instance.EndTurn();
		public static void DisplayAction(IBattleAction action) => instance.actionDisplay.DisplayAction(action);
		public static void ShowActionUI()
		{
			instance.characterActionUI.SetActive(true);
			instance.stateMachine.TrySetState(BattleState.ChoosingAction);
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
			instance.StartCoroutine(ResolveAction());

			IEnumerator ResolveAction()
			{
				yield return attacker.PerformAction(action, target);
				instance.EndTurn();
			}
		}

		// Dying is now handled inline by BattleCharacter.PerformAction as soon as a target's
		// HP drops to 0, rather than being batched here - by the time EndTurn() runs, any
		// death from this turn's action has already been fully played out.
		void EndTurn()
		{
			characterActing = null;
			instance.stateMachine.TrySetState(BattleState.Idle);
			OnTurnEnd?.Invoke();
			instance.characterActionUI.SetActive(false);
			instance.CheckBattleEnd();
		}

		public void SkipCurrentCharacterTurn() => EndTurn();

		void CheckBattleEnd()
		{
			// Dead characters stay in their party lists (an enemy's model is just hidden on
			// death), so both outcomes are "everyone on that side is dead"
			bool defeat = PlayerBattleParty.Count > 0 && PlayerBattleParty.All(hero => hero.IsDead);
			instance.characterActionUI.SetActive(false);
			if (defeat)
			{
				instance.stateMachine.TrySetState(BattleState.Concluding);
				StartCoroutine(Defeat());
			}
			var victory = EnemyBattleParty.Count > 0 && EnemyBattleParty.All(enemy => enemy.IsDead);
			if (victory)
			{
				instance.stateMachine.TrySetState(BattleState.Concluding);
				StartCoroutine(Victory());
			}
		}

		IEnumerator Defeat()
		{
			print("defeat!!");
			PlayLooped(gameOverJingle);
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
			PlayLooped(Encounter.Type == EncounterType.Grand ? victoryFanfareGrand : victoryFanfareStandard);

			// Gold is a pending reward, like the loot items - it isn't banked until the
			// player collects it from the loot screen (see CollectGoldReward).
			GoldGained = CalculateGoldReward();

			yield return new WaitForSeconds(1);
			BattleUIManager.Instance.OnVictory();

			foreach (HeroBattleCharacter battleCharacter in PlayerBattleParty)
				battleCharacter.SyncToCharacter();

			instance.enabled = false;
		}

		// Banks the pending gold reward and clears it, so a second collection attempt (e.g.
		// pressing Get All twice) adds nothing further - matches how collected loot items
		// have their amounts zeroed.
		public static void CollectGoldReward()
		{
			instance.inventory.Money += GoldGained;
			GoldGained = 0;
		}

		// gold = randInt(0, level^2 + randInt(1, 11)) x random(0.5, 1.5), rolled separately
		// for each defeated enemy and summed.
		static int CalculateGoldReward()
		{
			int total = 0;
			foreach (EnemyBattleCharacter enemy in EnemyBattleParty)
			{
				int upperBound = enemy.Level * enemy.Level + Random.Range(1, 11);
				int baseGold = Random.Range(0, upperBound);
				float variance = Random.Range(0.5f, 1.5f);
				total += (int)(baseGold * variance);
			}
			return total;
		}

		static void PlayLooped(AudioClip clip)
		{
			AudioSource musicSource = AudioManager.GetMusicAudioSource();
			musicSource.clip = clip;
			musicSource.loop = true;
			musicSource.Play();
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
