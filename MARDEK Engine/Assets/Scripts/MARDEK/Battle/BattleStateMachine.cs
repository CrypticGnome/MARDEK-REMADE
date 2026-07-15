using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
	using BattleState = BattleManager.BattleState;

	// Makes BattleState an enforced state machine rather than a label set from wherever:
	// every transition is checked against Transitions below, and an illegal one is
	// refused (and logged) instead of silently being applied.
	public class BattleStateMachine
	{
		public BattleState CurrentState { get; private set; } = BattleState.Idle;

		static readonly Dictionary<BattleState, HashSet<BattleState>> Transitions = new()
		{
			[BattleState.Idle] = new HashSet<BattleState> { BattleState.ChoosingAction, BattleState.ActionPerforming, BattleState.Concluding },
			[BattleState.ChoosingAction] = new HashSet<BattleState> { BattleState.ActionPerforming, BattleState.Idle },
			[BattleState.ActionPerforming] = new HashSet<BattleState> { BattleState.Idle },
			[BattleState.Concluding] = new HashSet<BattleState>(),
		};

		public bool TrySetState(BattleState next)
		{
			if (next == CurrentState) return true;

			if (!Transitions[CurrentState].Contains(next))
			{
				Debug.LogWarning($"Illegal battle state transition: {CurrentState} -> {next}");
				return false;
			}

			CurrentState = next;
			return true;
		}
	}
}
