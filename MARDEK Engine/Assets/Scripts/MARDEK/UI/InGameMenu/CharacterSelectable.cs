using MARDEK.CharacterSystem;
using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
	using Progress;
	public class CharacterSelectable : SelectableWithCurrentSelected<CharacterSelectable>
	{
		[SerializeField] TextMeshProUGUI characterNameText;
		[SerializeField] GameObject wrapper;
		[SerializeField] PartySO party;
		public Character Character
		{
			get
			{
				var index = transform.GetSiblingIndex();
				if (party is null || party.Count <= index)
					return null;
				return party[index];
			}
		}

		public override bool IsValid() => Character != null;


		private void OnEnable()
		{
			if (IsValid())
			{
				if (wrapper)
					wrapper.SetActive(true);
				if (characterNameText)
					characterNameText.text = Character.Profile.displayName;
			}
			else
			{
				if (wrapper)
					wrapper.SetActive(false);
			}
		}
	}
}