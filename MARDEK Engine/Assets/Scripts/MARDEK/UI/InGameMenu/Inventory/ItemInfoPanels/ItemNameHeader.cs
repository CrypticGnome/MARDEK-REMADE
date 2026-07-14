using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
	public class ItemNameHeader : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI textField;

		void Update()
		{
			var slot = SlotUI.selectedSlot;
			if (slot != null && slot.item != null)
				textField.text = $"{slot.item.displayName} x{slot.amount}";
			else
				textField.text = string.Empty;
		}
	}
}