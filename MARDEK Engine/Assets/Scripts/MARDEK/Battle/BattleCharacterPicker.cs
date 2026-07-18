using System;
using System.Collections.Generic;
using System.Linq;
using MARDEK.Battle;
using MARDEK.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MARDEK.UI
{
	public class BattleCharacterPicker : MonoBehaviour
	{
		[SerializeField] new Transform transform;
		[SerializeField] Transform pointersPoint;
		[SerializeField] SpriteRenderer crystalPointerRenderer;
		[SerializeField] GameObject lowerBar;
		[SerializeField] GameObject turnDisplay;
		[SerializeField] ActionDisplay actionDisplay;
		PlayerControls playerControls;
		static IReadOnlyList<BattleCharacter> Heroes => BattleManager.PlayerBattleParty;
		// Dead enemies stay in EnemyBattleParty with their model hidden, so exclude them
		// from target selection. Downed heroes stay individually selectable (e.g. for future
		// revival), but are excluded from "heal all" - there's no resurrection mechanic yet.
		static IReadOnlyList<BattleCharacter> Enemies => BattleManager.EnemyBattleParty.Where(enemy => !enemy.IsDead).ToList();
		static IReadOnlyList<BattleCharacter> HealableHeroes => Heroes.Where(hero => !hero.IsDead).ToList();

		/// <summary>
		/// Used to ensure that the action isn't invoked on a target on the same frame as an action is picked, as they both use the "interact" key
		/// </summary>
		float enabledTime;
		static int PositionIndex;
		public static bool EnemiesSelected;
		// Whether the armed action is currently targeting everyone on the selected side
		// rather than just SelectedCharacter.
		public static bool TargetingAll { get; private set; }
		public static BattleCharacter SelectedCharacter { get; private set; }
		IBattleAction action;

		// Pooled clones of crystalPointerRenderer, one per eligible target, shown instead of
		// the single pointer while TargetingAll is active.
		readonly List<SpriteRenderer> allTargetPointers = new();

		int frameTimer;
		const int WaitForFramesToInitialise = 1;
		Action<InputAction.CallbackContext> invokeActionOnTargetAction;
		Action<InputAction.CallbackContext> invokeCancellation;

		private void Awake()
		{
			playerControls = new PlayerControls();
			playerControls.Enable();
			invokeActionOnTargetAction = context => InvokeActionOnTarget();
			invokeCancellation = context => HandleCancellation();
			void HandleCancellation()
			{
				lowerBar.SetActive(true);
				gameObject.SetActive(false);
				turnDisplay.SetActive(true);
			}
		}
		private void OnEnable()
		{
			frameTimer = WaitForFramesToInitialise;
		}
		private void OnDisable()
		{
			playerControls.DefaultMap.Movement.started -= HandleSelection;
			playerControls.DefaultMap.Interact.started -= invokeActionOnTargetAction;
			playerControls.DefaultMap.Cancel.performed -= invokeCancellation;

			action = null;
		}
		private void Update()
		{
			if (frameTimer-- != 0) return;
			// Delay the hooking up of events be one frame to ensure target selection isn't done on the same frame as the picker being enabled
			playerControls.DefaultMap.Movement.started += HandleSelection;
			playerControls.DefaultMap.Interact.started += invokeActionOnTargetAction;
			playerControls.DefaultMap.Cancel.performed += invokeCancellation;

		}
		private void OnDestroy()
		{
			playerControls.Dispose();
		}

		public void HandleSelection(InputAction.CallbackContext context)
		{
			if (!enabled)
				return;

			// AllOnly actions have nothing to navigate - the pointer stays locked over every
			// eligible target on the side TargetsAllies already fixed.
			if (action.Action.TargetScope == TargetScope.AllOnly)
				return;

			var value = context.ReadValue<Vector2>();
			if (value.Equals(Vector2.zero)) return;

			bool supportsAllToggle = action.Action.TargetScope == TargetScope.SingleOrAll;

			// Handle horizontal input
			if (value.x == -1 && !EnemiesSelected)
			{
				EnemiesSelected = true;
				TargetingAll = false;
				PositionIndex = ClampEnemiesIndex(PositionIndex);
			}
			else if (value.x == 1 && EnemiesSelected)
			{
				EnemiesSelected = false;
				TargetingAll = false;
				PositionIndex = ClampHeroesIndex(PositionIndex);
			}
			// Pressing further into the side already being viewed toggles targeting everyone
			// on it instead of doing nothing - only offered when there's more than one
			// eligible target, so it's never a no-op stand-in for single-select.
			else if (value.x == -1 && EnemiesSelected && supportsAllToggle && Enemies.Count > 1)
				TargetingAll = !TargetingAll;
			else if (value.x == 1 && !EnemiesSelected && supportsAllToggle && HealableHeroes.Count > 1)
				TargetingAll = !TargetingAll;

			// Handle verticle input (ordered from top to bottom) - moving the cursor always
			// means picking someone specific, so it cancels targeting-all.
			if (value.y == -1)
			{
				TargetingAll = false;
				PositionIndex = EnemiesSelected ? ClampEnemiesIndex(PositionIndex + 1) : ClampHeroesIndex(PositionIndex + 1);
			}
			else if (value.y == 1)
			{
				TargetingAll = false;
				PositionIndex = EnemiesSelected ? ClampEnemiesIndex(PositionIndex - 1) : ClampHeroesIndex(PositionIndex - 1);
			}

			SelectedCharacter = EnemiesSelected ?
				 Enemies.OrderByDescending(e => e.battleModel.transform.position.y).ElementAt(PositionIndex) :
				 Heroes.OrderByDescending(e => e.battleModel.transform.position.y).ElementAt(PositionIndex);

			SetPosition();
		}
		public void InvokeActionOnTarget()
		{
			if (Time.time == enabledTime) return;

			if (TargetingAll)
				BattleManager.PerformActionToTarget(action, EnemiesSelected ? Enemies : HealableHeroes);
			else
				BattleManager.PerformActionToTarget(action, SelectedCharacter);

			actionDisplay.DisplayAction(action);
			gameObject.SetActive(false);

		}
		public void EnableWithAction(IBattleAction action)
		{
			// Currently assume that the target is the enemy, may want to pass in a parameter to say which team the action should default to at some point
			enabledTime = Time.time;
			lowerBar.SetActive(false);

			gameObject.SetActive(true);
			this.action = action;
			TargetingAll = action.Action.TargetScope == TargetScope.AllOnly;
			// Heals/buffs default to the caster's own team; everything else defaults to the enemy team.
			EnemiesSelected = !action.Action.TargetsAllies;
			SelectedCharacter = EnemiesSelected ? Enemies[0] : Heroes[0];
			SetPosition();
			turnDisplay.SetActive(false);
		}

		int ClampHeroesIndex(int index) => Mathf.Clamp(index, 0, Heroes.Count - 1);
		int ClampEnemiesIndex(int index) => Mathf.Clamp(index, 0, Enemies.Count - 1);
		void SetPosition()
		{
			crystalPointerRenderer.gameObject.SetActive(!TargetingAll);

			if (TargetingAll)
			{
				ShowAllTargetPointers(EnemiesSelected ? Enemies : HealableHeroes);
				return;
			}

			foreach (SpriteRenderer pointer in allTargetPointers)
				pointer.gameObject.SetActive(false);

			BattleModelAnimator target = SelectedCharacter.battleModel;
			transform.Set2DPosition(target.CrystalPointerGoToPosition.position);
			transform.localScale = EnemiesSelected ? new Vector3(-0.1f, 0.1f, 1f) : new Vector3(0.1f, 0.1f, 1f);
		}

		void ShowAllTargetPointers(IReadOnlyList<BattleCharacter> targets)
		{
			while (allTargetPointers.Count < targets.Count)
				allTargetPointers.Add(Instantiate(crystalPointerRenderer, pointersPoint));

			for (int i = 0; i < allTargetPointers.Count; i++)
			{
				bool active = i < targets.Count;
				allTargetPointers[i].gameObject.SetActive(active);
				if (active)
					allTargetPointers[i].transform.position = targets[i].battleModel.CrystalPointerGoToPosition.position;
			}
		}
	}
}
