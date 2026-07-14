using MARDEK.Progress;
using UnityEngine;
namespace MARDEK.Inventory
{
	[CreateAssetMenu(fileName = "InventorySO", menuName = "MARDEK/Progress/InventorySO")]
	public class InventorySO : ScriptableObject
	{
		public PlotItems PlotItems;
		public int Money;
		public PlayableCharacters Characters;
	}

}