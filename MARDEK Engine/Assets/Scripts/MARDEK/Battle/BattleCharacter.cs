using System.Collections;
using MARDEK.Animation;
using MARDEK.CharacterSystem;
using MARDEK.Skill;
using MARDEK.Stats;
using UnityEngine;

namespace MARDEK.Battle
{
	// Lives on the root of a battle-model prefab; the visuals (Animation + sprites) live on
	// a child, so hiding the model on death keeps this component and its state alive.
	public abstract class BattleCharacter : MonoBehaviour
	{
		public CharacterProfile Profile { get; protected set; }
		public BattleModelAnimator battleModel = null;
		public ActionSkillset Skillset { get; protected set; }
		public string Name { get { return Profile.displayName; } }

		public int CurrentHP { get; set; }
		public int CurrentMP { get; set; }
		public float ACT { get; set; }
		public CoreStats BaseStats { get { return Profile.Stats; } }
		public CoreStats VolatileStats { get; set; }

		public int MaxHP => VolatileStats.MaxHP;

		public int MaxMP => VolatileStats.MaxMP;

		public int Attack => VolatileStats.Attack;

		public int Defense { get => VolatileStats.Defense; set => VolatileStats.Defense = value; }
		public int MagicDefense { get => VolatileStats.MagicDefense; set => VolatileStats.MagicDefense = value; }
		public Absorbtions Absorbtions { get => VolatileStats.Absorbtions; set => VolatileStats.Absorbtions = value; }
		public int Strength { get => VolatileStats.Strength; set => VolatileStats.Strength = value; }
		public int Vitality { get => VolatileStats.Vitality; set => VolatileStats.Vitality = value; }
		public int Spirit { get => VolatileStats.Spirit; set => VolatileStats.Spirit = value; }
		public int Agility { get => VolatileStats.Agility; set => VolatileStats.Agility = value; }
		public StatusEffects Resistances { get => VolatileStats.Resistances; set => VolatileStats.Resistances = value; }
		public int Accuracy { get => VolatileStats.Accuracy; set => VolatileStats.Accuracy = value; }
		public int CritRate { get => VolatileStats.CritRate; set => VolatileStats.CritRate = value; }

		public delegate void StatChanged();
		public event StatChanged OnStatChanged;
		public int Level
		{
			get;
			protected set;
		}

		public StatusEffects StatusBuildup = new StatusEffects();
		public bool stunned;

		private void OnValidate()
		{
			if (battleModel == null)
				battleModel = GetComponentInChildren<BattleModelAnimator>(true);
		}

		/// <summary>
		/// Initialises this battle character from the persistent Character it represents.
		/// Called by BattleManager right after the battle-model prefab is instantiated.
		/// </summary>
		public abstract void LoadCharacter(Character character);

		// Setup shared by both LoadCharacter implementations: profile/level, model lookup,
		// and a fresh VolatileStats copy so battle changes never write into the shared
		// CharacterProfile asset.
		protected void InitialiseFrom(Character character)
		{
			Profile = character.Profile;
			Level = character.Level;
			if (battleModel == null)
				battleModel = GetComponentInChildren<BattleModelAnimator>(true);

			VolatileStats = new CoreStats(BaseStats);
			BaseStats.CalculateMaxValues(this);
			VolatileStats.CalculateMaxValues(this);
		}

		public float ActBuildRate()
		{
			float actRate = 1 + 0.05f * VolatileStats.Agility;
			actRate *= 1000;
			return actRate;
		}

		// Advances ACT and status effects for the turn about to happen. Returns false if
		// the character is stunned and can't act this turn.
		public bool BeginTurn()
		{
			ACT -= TurnManager.ActResolution;
			TickStatusEffects();

			if (stunned)
			{
				Debug.Log($"{Name} is stunned");
				return false;
			}
			return true;
		}

		// What this character does with a turn it's able to take. Base (hero) behavior
		// shows the action UI and waits for player input; EnemyBattleCharacter overrides
		// this to dispatch its queued move instead.
		public virtual IEnumerator TakeAction()
		{
			BattleManager.ShowActionUI();
			yield break;
		}

		// Plays this character's animation against target and applies the action at the
		// right moment (melee/breath: at the strike's damage point; spell/item: immediately).
		// Falls back to a fixed wait when there's no battle model to animate against. If the
		// target dies as a result, their death animation starts immediately rather than
		// waiting for the end of the turn, and this waits for whichever finishes later - the
		// attacker's action animation or the target's death animation. The death routine runs
		// on the target, which is safe because Die() only hides the visuals child - the
		// target's own GameObject stays active.
		public IEnumerator PerformAction(IBattleAction action, BattleCharacter target)
		{
			Coroutine deathRoutine = null;

			void ApplyAction()
			{
				action.TryPerformAction(this, target);
				if (target.IsDead)
					deathRoutine = target.StartCoroutine(target.Die());
			}

			if (action is ActionSkill skill && battleModel != null)
				yield return battleModel.PlayAction(skill.Action.ActionType, target.battleModel, ApplyAction);
			else
			{
				ApplyAction();
				yield return new WaitForSeconds(1.5f);
			}

			if (deathRoutine != null)
				yield return deathRoutine;
		}

		public void TickStatusEffects()
		{
			StatusEffects resistances = Profile.Stats.Resistances;
			if (StatusBuildup.Poison > 0)
			{
				StatusBuildup.Poison -= resistances.Poison + 1;
				int damage = Mathf.RoundToInt((float)BaseStats.MaxHP / 20 + 0.5f);
				CurrentHP -= damage;
				Debug.Log($"{Profile.displayName} is poisoned and takes {damage} damage");

			}
			if (StatusBuildup.Paralysis > 0)
			{
				stunned = !stunned;
				StatusBuildup.Paralysis -= resistances.Paralysis + 1;
				Debug.Log($"{Profile.displayName} is paralysed and has {StatusBuildup.Paralysis} paralysis build up");

			}
			else
				stunned = false;

			if (StatusBuildup.Blindness > 0)
				StatusBuildup.Blindness -= resistances.Blindness + 1;
			if (StatusBuildup.Silence > 0)
				StatusBuildup.Silence -= resistances.Silence + 1;
			if (StatusBuildup.Numbness > 0)
				StatusBuildup.Numbness -= resistances.Numbness + 1;
			if (StatusBuildup.Curse > 0)
				StatusBuildup.Curse -= resistances.Curse + 1;
			if (StatusBuildup.Confusion > 0)
				StatusBuildup.Confusion -= resistances.Confusion + 1;
			if (StatusBuildup.Bleed > 0)
				StatusBuildup.Bleed -= resistances.Bleed + 1;
			// Do not tick zombification
		}

		public bool IsDead => CurrentHP <= 0;

		// What happens when this character's HP hits zero. No-op by default - heroes stay
		// downed in the party rather than being removed (see CheckBattleEnd), and hero death
		// handling isn't implemented yet (see BattleSystem.md's Notable Gaps). EnemyBattleCharacter
		// overrides this to play its death animation and hide the model.
		public virtual IEnumerator Die()
		{
			yield break;
		}

		public Sprite GetBattleIcon()
		{
			if (Profile.WalkSprites != null)
			{
				SpriteAnimationClip clip = Profile.WalkSprites.GetClipByDirection(Vector2.down);
				if (clip != null)
				{
					Sprite sprite = clip.GetSprite(0);
					if (sprite != null) return sprite;
				}
			}
			return CharacterTypePortraits.CharacterTypeSprites[Profile.Type];
		}


	}
}
