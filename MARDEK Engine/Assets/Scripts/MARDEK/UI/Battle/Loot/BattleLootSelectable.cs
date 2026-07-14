using System.Collections.Generic;
using MARDEK.Inventory;


namespace MARDEK.UI
{
	public abstract class BattleLootSelectable : Selectable
	{
		public static BattleLootSelectable currentlySelected;

		public abstract void Interact(List<Item> items, List<int> amounts);

		override public void Select(bool playSFX = true)
		{
			base.Select(playSFX);
			currentlySelected = this;
		}
	}
}
