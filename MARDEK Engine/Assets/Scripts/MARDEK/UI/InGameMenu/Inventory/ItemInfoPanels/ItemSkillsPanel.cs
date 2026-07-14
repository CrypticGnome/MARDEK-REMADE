using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
	public class ItemSkillsPanel : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI textField;

		void Update()
		{
			var slot = SlotUI.selectedSlot;
			if (slot != null && slot.item != null)
				textField.text = "Skills: WIP";
			else
				textField.text = string.Empty;
		}
	}
}
