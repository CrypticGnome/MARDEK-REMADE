using System;
using System.Collections;
using MARDEK.Skill;
using UnityEngine;

namespace MARDEK.Battle
{
	public class BattleModelComponent : MonoBehaviour
	{
		[SerializeField] AnimationClip idle, moveto, strike, jumpback, hurt, die, dead, spellcast, useItem, victory;
		[SerializeField] AnimationClip breath;
		[SerializeField, HideInInspector] UnityEngine.Animation animation;
		[SerializeField, HideInInspector] new Transform transform;
		[SerializeField] Transform crystalPointerGoToPosition;
		[SerializeField] Transform strikePoint;
		[SerializeField] Transform hitPoint;
		[SerializeField] float moveToDuration = 0.5f;
		[SerializeField] DamageDisplay damageDisplay;
		public DamageDisplay DamageDisplay => damageDisplay;
		public Transform CrystalPointerGoToPosition => crystalPointerGoToPosition;
		public Transform StrikePoint => strikePoint;
		public Transform HitPoint => hitPoint;

		private void OnValidate()
		{
			if (transform == null)
				transform = GetComponent<Transform>();
			if (animation == null)
				animation = GetComponentInChildren<UnityEngine.Animation>();
			if (damageDisplay == null)
				damageDisplay = GetComponentInChildren<DamageDisplay>();
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			if (TryPlayClip(idle))
			{
				animation.wrapMode = WrapMode.Loop;
				AnimationState state = animation[idle.name];
				state.time = UnityEngine.Random.Range(0f, state.length);
			}

			var layer = SortingLayer.NameToID($"BattleModel {(int)transform.position.z}");
			foreach (var r in GetComponentsInChildren<SpriteRenderer>())
				r.sortingLayerID = layer;
		}

		/// <summary>
		/// Plays the given clip, logging (not throwing) if it wasn't assigned in the
		/// inspector so a missing animation doesn't break the rest of the battle flow.
		/// </summary>
		bool TryPlayClip(AnimationClip clip)
		{
			try
			{
				animation.clip = clip;
				animation.Play(clip.name);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError($"{name}: BattleModelComponent tried to play an unassigned animation clip - {e.Message}", this);
				return false;
			}
		}

		// Falls back to 0 for an unassigned clip so waits based on clip length resolve
		// immediately instead of throwing.
		static float ClipLength(AnimationClip clip) => clip != null ? clip.length : 0f;

		/// <summary>
		/// Fired by an Animation Event calling OnDamagePoint(), placed on a strike/breath
		/// clip's timeline at the frame the attack actually connects. The battle flow waits
		/// for this to apply damage/effects and trigger the target's Hurt animation at that
		/// exact moment rather than at the start of the attacker's animation.
		/// </summary>
		public event Action DamagePoint;
		public void OnDamagePoint() => DamagePoint?.Invoke();

		public void PlayAnimation(BattleAnimationType animType)
		{
			switch (animType)
			{
				default:
				case BattleAnimationType.Idle:
				{
					TryPlayClip(idle);
					break;
				}
				case BattleAnimationType.MoveTo:
				{
					StartCoroutine(PlayClipAndReturnToIdle(moveto));
					break;
				}
				case BattleAnimationType.Strike:
				{
					StartCoroutine(PlayClipAndReturnToIdle(strike));
					break;
				}
				case BattleAnimationType.JumpBack:
				{
					StartCoroutine(PlayClipAndReturnToIdle(jumpback));
					break;
				}
				case BattleAnimationType.Hurt:
				{
					StartCoroutine(PlayClipAndReturnToIdle(hurt));
					break;
				}
				case BattleAnimationType.Die:
				{
					TryPlayClip(die);
					animation.wrapMode = WrapMode.Once;
					break;
				}
				case BattleAnimationType.Dead:
				{
					TryPlayClip(dead);
					animation.wrapMode = WrapMode.Loop;
					break;
				}
				case BattleAnimationType.Spellcast:
				{
					StartCoroutine(PlayClipAndReturnToIdle(spellcast));
					break;
				}
				case BattleAnimationType.UseItem:
				{
					StartCoroutine(PlayClipAndReturnToIdle(useItem));
					break;
				}
				case BattleAnimationType.Victory:
				{
					TryPlayClip(victory);
					animation.wrapMode = WrapMode.Loop;
					break;
				}
			}

			IEnumerator PlayClipAndReturnToIdle(AnimationClip clip)
			{
				TryPlayClip(clip);
				yield return new WaitForSeconds(ClipLength(clip));
				TryPlayClip(idle);
			}
		}

		/// <summary>
		/// Runs the full melee/breath approach: MoveTo while travelling until this model's
		/// Strike Point overlaps the target's Hit Point, play the strike clip (applying
		/// onDamagePoint when the clip's DamagePoint Animation Event fires, or at the end
		/// of the clip if it doesn't have one authored yet), then JumpBack while returning
		/// to the idle position.
		/// </summary>
		IEnumerator PlayApproachAndStrikeSequence(BattleModelComponent target, AnimationClip strikeClip, Action onDamagePoint)
		{
			Vector3 idlePosition = transform.position;

			Vector3 attackPosition = idlePosition;
			if (target is not null && strikePoint is not null && target.hitPoint is not null)
			{
				attackPosition = idlePosition + (target.hitPoint.position - strikePoint.position);
			}

			yield return MoveWithClip(moveto, idlePosition, attackPosition, moveToDuration);
			TryPlayClip(strikeClip);
			yield return WaitForDamagePoint(ClipLength(strikeClip), onDamagePoint);
			yield return MoveWithClip(jumpback, attackPosition, idlePosition, ClipLength(jumpback));
			TryPlayClip(idle);

			IEnumerator MoveWithClip(AnimationClip clip, Vector3 from, Vector3 to, float duration)
			{
				TryPlayClip(clip);
				for (float t = 0; t < duration; t += Time.deltaTime)
				{
					transform.position = Vector3.Lerp(from, to, t / duration);
					yield return null;
				}
				transform.position = to;
			}
		}

		IEnumerator WaitForDamagePoint(float clipLength, Action onDamagePoint)
		{
			bool applied = false;
			void ApplyOnce()
			{
				if (applied) return;
				applied = true;
				onDamagePoint?.Invoke();
			}

			DamagePoint += ApplyOnce;
			float elapsed = 0f;
			while (!applied && elapsed < clipLength)
			{
				elapsed += Time.deltaTime;
				yield return null;
			}
			DamagePoint -= ApplyOnce;

			// No DamagePoint Animation Event authored on this clip yet - fall back to
			// applying at the end of the clip so the action still resolves.
			ApplyOnce();
		}

		/// <summary>
		/// Plays the appropriate animation sequence for the given skill's ActionType:
		/// Melee/Breath run the full approach-strike-return sequence (with the breath
		/// clip swapped in for Breath) and invoke applyEffect at the strike's DamagePoint,
		/// while Spellcast/Item just play their animation in place and apply immediately.
		/// </summary>
		public IEnumerator PlayAction(ActionSkill skill, BattleModelComponent target, Action applyEffect)
		{
			switch (skill.Action.ActionType)
			{
				case ActionType.Melee:
					yield return PlayApproachAndStrikeSequence(target, strike, applyEffect);
					break;
				case ActionType.Breath:
					yield return PlayApproachAndStrikeSequence(target, breath, applyEffect);
					break;
				case ActionType.Spellcast:
					applyEffect?.Invoke();
					PlayAnimation(BattleAnimationType.Spellcast);
					break;
				case ActionType.Item:
					applyEffect?.Invoke();
					PlayAnimation(BattleAnimationType.UseItem);
					break;
			}
		}

		/// <summary>
		/// Plays the Die clip and waits for it to finish - used so a defeated character's
		/// model can be destroyed only after its death animation has played out.
		/// </summary>
		public IEnumerator PlayDeathSequence()
		{
			TryPlayClip(die);
			animation.wrapMode = WrapMode.Once;
			yield return new WaitForSeconds(ClipLength(die));
		}

	}
	public enum BattleAnimationType
	{
		Idle,
		MoveTo,
		Strike,
		JumpBack,
		Hurt,
		Die,
		Dead,
		Spellcast,
		UseItem,
		Victory
	}
	public enum ActionType
	{
		Melee,
		Spellcast,
		Breath,
		Item,
	}
}
