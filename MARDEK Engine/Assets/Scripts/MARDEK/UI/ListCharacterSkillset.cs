using MARDEK.Battle;
using MARDEK.CharacterSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
	public class ListCharacterSkillset : ListBattleActions
	{
		ActionSkillset skillsetToShow;
		[SerializeField] TextMeshProUGUI skillsetNameLabel = null;
		[SerializeField] Image skillsetIcon = null;
		[SerializeField] Selectable thisSelectable;
		private void OnEnable()
		{
			BattleManager.OnTurnStart += GetSkillset;
			GetSkillset();
		}
		private void OnDisable()
		{
			BattleManager.OnTurnStart -= GetSkillset;
		}

		void GetSkillset()
		{
			BattleCharacter character = BattleManager.characterActing;
			skillsetToShow = character.Skillset;

			if (skillsetToShow)
			{
				skillsetNameLabel.text = skillsetToShow.name;
				skillsetIcon.sprite = skillsetToShow.Sprite;
			}
			else
			{
				skillsetNameLabel.text = " - ";
				skillsetIcon.sprite = null;
			}
		}

		public void SetSlots()
		{
			if (!thisSelectable.Selected) return;
			ClearSlots();
			GetSkillset();
			if (!skillsetToShow)
			{
				Debug.LogWarning("No skillset to show");
				return;
			}

			foreach (var skill in skillsetToShow.Skills)
			{
				if (skill is null)
				{
					Debug.LogWarning($"{skillsetToShow.name} has an unassigned skill slot", skillsetToShow);
					continue;
				}

				BattleActionSlot slot = new BattleActionSlot(skill);

				SetNextSlot(slot);
			}
			UpdateLayout();
		}
	}
}