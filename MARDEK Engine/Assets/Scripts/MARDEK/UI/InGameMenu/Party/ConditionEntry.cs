using MARDEK.CharacterSystem;
using MARDEK.Stats;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MARDEK.UI
{
    public class ConditionEntry : MonoBehaviour, PartyEntry
    {
        [SerializeField] Image elementImage;

        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI levelAndClassText;

        [SerializeField] ConditionBar hpBar;
        [SerializeField] ConditionBar mpBar;
        [SerializeField] Image xpProgressBar;
        [SerializeField] TextMeshProUGUI xpText;

        [SerializeField] IntegerStat levelStat;
        [SerializeField] IntegerStat currentHpStat;
        [SerializeField] IntegerStat maxHpStat;
        [SerializeField] IntegerStat currentMpStat;
        [SerializeField] IntegerStat maxMpStat;

        public void SetCharacter(Character character)
        {
            if (character == null || character.Profile == null)
                return;

            elementImage.sprite = character.Profile.element.thickSprite;
            nameText.text = character.Profile.displayName;
            levelAndClassText.text = "Lv " + character.Level + " " + character.Profile.displayClass;
            hpBar.SetValues(character.CurrentHP, character.MaxHP);
            mpBar.SetValues(character.CurrentMP, character.MaxMP);
            // TODO Update XP bar
        }
    }
}
