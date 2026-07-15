using MARDEK.Battle;
using TMPro;
using UnityEngine;

namespace MARDEK.UI
{
	public class ExperienceBar : MonoBehaviour
	{
		[SerializeField] RectTransform barTransform;
		[SerializeField] TextMeshProUGUI statText;
		HeroBattleCharacter character;

		public void SetCharacter(HeroBattleCharacter character)
		{
			if (character is null)
				return;
			this.character = character;
			character.OnStatChanged += UpdateBar;
			UpdateBar();
		}
		private void Update()
		{
			// Currently when a stat changes the OnStatChanged delegate isn't fired. Therefore, the healthbar doesn't update.
			// Instead of fixing that I'm just doing a quick band aid fix. Future work.
			UpdateBar();
		}
		[ContextMenu("Update Bar")]
		void UpdateBar()
		{
			if (character == null)
				return;

			var statValue = (float)character.Experience;
			var maxStatValue = 100;
			if (statText)
				statText.text = "Lv " + character.Level.ToString();
			if (barTransform)
			{
				float xScale = Mathf.Clamp(statValue / maxStatValue, 0f, 1f);
				if (float.IsFinite(xScale))
					barTransform.localScale = new Vector3(xScale, 1, 1);
			}
		}
	}
}
