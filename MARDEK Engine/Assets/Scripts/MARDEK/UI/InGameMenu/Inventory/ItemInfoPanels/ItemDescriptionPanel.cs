using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
	public class ItemDescriptionPanel : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI textField;

		void Update()
		{
			var slot = SlotUI.selectedSlot;
			if (slot != null && slot.item != null)
				textField.text = slot.item.description;
			else
				textField.text = string.Empty;
		}
	}
}