using UnityEngine;
namespace MARDEK.Battle
{
	using System;
	using MARDEK.Audio;
	using MARDEK.Stats;
	[Serializable]
	public class BattleAction
	{
		[SerializeField] Element element;
		[SerializeField] SoundEffect[] soundEffects;
		[SerializeReference, SubclassSelector] ActionEffects[] actionEffects;

		[HideInInspector] public ActionType ActionType;
		public Element Element { get { return element; } }

		public void Apply(BattleCharacter user, BattleCharacter target)
		{
			for (int index = 0; index < actionEffects.Length; index++)
				actionEffects[index].ApplyEffect(user, target, element);
			AudioManager.PlayEffectString(soundEffects);
		}
	}
}
