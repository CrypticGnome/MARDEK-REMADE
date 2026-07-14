using MARDEK.Skill;
using UnityEngine;

namespace MARDEK.UI
{
	public class AttackButton : MonoBehaviour
	{
		[SerializeField] ActionSkill attack;
		[SerializeField] BattleCharacterPicker targetPicker;
		private void OnEnable()
		{
			// Set sprite to character specific attack sprite
		}
		public void SelectAction()
		{
			targetPicker.EnableWithAction(attack);
		}
	}
}