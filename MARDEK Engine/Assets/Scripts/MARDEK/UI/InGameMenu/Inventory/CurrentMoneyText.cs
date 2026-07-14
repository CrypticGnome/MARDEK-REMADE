using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
	using MARDEK.Inventory;

	public class CurrentMoneyText : MonoBehaviour
	{
		[SerializeField] TMP_Text text;
		[SerializeField] InventorySO inventory;
		void FixedUpdate()
		{
			text.text = inventory.Money.ToString();
		}
	}
}
