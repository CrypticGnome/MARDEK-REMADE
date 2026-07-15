using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MARDEK.Battle;
using TMPro;

namespace MARDEK.UI
{
	public class EnemyCharacterUI : MonoBehaviour, IPointerClickHandler
	{
		public BattleCharacter character { get; private set; }
		[SerializeField] GameObject basePanel;
		[SerializeField] EnemyHealthBar healthBar;
		[SerializeField] TextMeshProUGUI characterName;
		[SerializeField] Image elementImage;
		[SerializeField] TextMeshProUGUI levelText;

		private void Start()
		{
			UpdateCharacter();
			if (character == null)
			{
				basePanel.SetActive(false);
				return;
			}
			healthBar.SetCharacter(character);
			characterName.text = character.Name;
			elementImage.sprite = character.Profile.element.thickSprite;
			levelText.text = $"Lv {character.Level}";
		}

		void UpdateCharacter()
		{
			basePanel.SetActive(false);
			var index = transform.GetSiblingIndex();

			if (index < BattleManager.EnemyBattleParty.Count)
			{
				character = BattleManager.EnemyBattleParty[index];
				basePanel.SetActive(true);
			}
			else
			{
				character = null;
				basePanel.SetActive(false);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			BattleUIManager.Instance.InspectCharacter(character);
		}
	}
}
