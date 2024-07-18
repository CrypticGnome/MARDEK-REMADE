using MARDEK.CharacterSystem;
using MARDEK.Stats;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MARDEK.UI
{
    public class VitalStatisticsEntry : MonoBehaviour, PartyEntry
    {
        [SerializeField] TextMeshProUGUI attackValue;
        [SerializeField] TextMeshProUGUI defValue;
        [SerializeField] TextMeshProUGUI mdefValue;

        [SerializeField] TextMeshProUGUI strValue;
        [SerializeField] TextMeshProUGUI vitValue;
        [SerializeField] TextMeshProUGUI sprValue;
        [SerializeField] TextMeshProUGUI aglValue;

        [SerializeField] TextMeshProUGUI strBonus;
        [SerializeField] TextMeshProUGUI vitBonus;
        [SerializeField] TextMeshProUGUI sprBonus;
        [SerializeField] TextMeshProUGUI aglBonus;

        [SerializeField] IntegerStat attackStat;
        [SerializeField] IntegerStat defStat;
        [SerializeField] IntegerStat mdefStat;

        [SerializeField] IntegerStat strStat;
        [SerializeField] IntegerStat vitStat;
        [SerializeField] IntegerStat sprStat;
        [SerializeField] IntegerStat aglStat;

        public void SetCharacter(Character character)
        {
               attackValue.text = character.Attack.ToString();
            defValue.text = character.Defense.ToString();
            mdefValue.text = character.MagicDefense.ToString();

            strValue.text = character.Strength.ToString();
            vitValue.text = character.Vitality.ToString();
            sprValue.text = character.Spirit.ToString();
            aglValue.text = character.Agility.ToString();

            // TODO Update the colors of the values and bonuses
            // TODO Update the bonuses
        }
    }
}
