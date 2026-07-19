using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MARDEK.Battle.BattleManager;
namespace MARDEK.Battle
{
	public class TurnManager
	{
		public static float MaxTimeBetweenTurns = 2f;
		public const float ActResolution = 1000f;

		// The ACT lists built by GetCharacterACTNextTurn and consumed here are parallel to
		// AllCombatants, which always iterates enemies first then heroes - dead characters
		// are included (with frozen ACT) to keep the indices aligned.
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
				foreach (BattleCharacter character in AllCombatants)
				{
					character.ACT = Mathf.Lerp(startACT[listIndex], finalACT[listIndex], waitCompletion);
					listIndex++;
				}
				yield return null;
				timer += Time.deltaTime;
			}
		}

		// Dead characters stay in their party lists but never get another turn. Returns a
		// null nextActor when no one is left alive (the battle is concluding).
		public static void GetTimeToNextTurn(out float timeToTurn, out BattleCharacter nextActor)
		{
			timeToTurn = float.MaxValue;
			nextActor = null;
			foreach (BattleCharacter character in AllCombatants)
			{
				if (character.IsDead)
					continue;
				float characterTimeToTurn = TimeToTurn(character, 1);
				if (characterTimeToTurn < timeToTurn)
				{
					timeToTurn = characterTimeToTurn;
					nextActor = character;
				}
			}
			if (nextActor == null)
				timeToTurn = 0;
		}

		public static void GetCharacterACTNextTurn(float timeToTurn, out List<float> startACT, out List<float> finalACT)
		{
			startACT = new List<float>();
			finalACT = new List<float>();
			foreach (BattleCharacter character in AllCombatants)
			{
				startACT.Add(character.ACT);

				// Dead characters keep their ACT frozen instead of building toward a turn
				if (character.IsDead)
				{
					finalACT.Add(character.ACT);
					continue;
				}

				float actRate = character.ActBuildRate();
				finalACT.Add(character.ACT + actRate * timeToTurn / MaxTimeBetweenTurns);
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
