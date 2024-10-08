using UnityEngine;
using UnityEngine.UI;
using MARDEK.CharacterSystem;
using MARDEK.Skill;
using Codice.Client.Common;

namespace MARDEK.UI
{
    public class SkillMenuSkillSlotUI : SelectableWithCurrentSelected<SkillMenuSkillSlotUI>
    {
        static readonly Color SELECTED_TEXT_COLOR = new Color(240f / 255f, 224f / 255f, 185f / 255f);
        static readonly Color DEFAULT_TEXT_COLOR = new Color(238f / 255f, 203f / 255f, 127f / 255f);

        public static SkillMenuSkillSlotUI selectedSkill { get; private set; }

        [SerializeField] Image elementOrCheckbox;
        [SerializeField] Text skillNameText;
        [SerializeField] Text mpOrRp;
        [SerializeField] ConditionBar apBar;
        [SerializeField] GameObject masteredImage;
        [SerializeField] Image disabledCover;

        [SerializeField] Sprite checkedSprite;
        [SerializeField] Sprite uncheckedSprite;

        Text selectedSkillName;
        Text selectedSkillDescription;
        Image selectedSkillElement;
        GameObject selectedSkillPointer;

        Character character;
        Skill.Skill skill;
        bool isSelected;
        bool isEnabled = true;

        int shouldMoveSelectedSkillPointer;

        public void Toggle()
        {
            this.isEnabled = !this.isEnabled;
            this.UpdateAppearance();
        }

        void UpdateAppearance()
        {
            this.skillNameText.text = this.skill.DisplayName;
            this.mpOrRp.text = this.skill.Cost.ToString();

            Color textColor = this.isSelected ? SELECTED_TEXT_COLOR : DEFAULT_TEXT_COLOR;
            this.skillNameText.color = textColor;
            this.mpOrRp.color = textColor;

            //if (this.skill.category.isActive)
            //{
            //    this.elementOrCheckbox.sprite = this.skill.element.thickSprite;
            //    this.elementOrCheckbox.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            //}
            //else
            //{
            //    if (this.isEnabled) this.elementOrCheckbox.sprite = this.checkedSprite;
            //    else this.elementOrCheckbox.sprite = this.uncheckedSprite;
            //    this.elementOrCheckbox.transform.localScale = new Vector3(1f, 1f, 1f);
            //}

            int currentMastery = -1;
            int maxMastery = this.skill.PointsRequiredToMaster;

            if (currentMastery >= maxMastery)
            {
                this.apBar.gameObject.SetActive(false);
                this.masteredImage.gameObject.SetActive(true);
                this.disabledCover.gameObject.SetActive(false);
            }
            else
            {
                this.apBar.gameObject.SetActive(true);
                this.masteredImage.gameObject.SetActive(false);
                this.apBar.SetValues(currentMastery, maxMastery);

                //bool canLearn = this.character.GetStat(this.skill.canLearnStat).Value > 0;
                //this.disabledCover.gameObject.SetActive(!canLearn);
                disabledCover.gameObject.SetActive(true);
            }

            this.selectedSkillName.text = this.skill.DisplayName;
            this.selectedSkillDescription.text = this.skill.Description;
               this.selectedSkillElement.sprite = null;
               Debug.LogAssertion("SkillMenuSkillSlotUI is not functional");
        }

        public void Init(Text selectedSkillName, Text selectedSkillDescription, Image selectedSkillElement, GameObject selectedSkillPointer)
        {
            this.selectedSkillName = selectedSkillName;
            this.selectedSkillDescription = selectedSkillDescription;
            this.selectedSkillElement = selectedSkillElement;
            this.selectedSkillPointer = selectedSkillPointer;
            this.isEnabled = true;
        }

        public void SetSkill(Character character, Skill.Skill skill)
        {
            this.character = character;
            this.skill = skill;

            this.UpdateAppearance();
        }

        void FixedUpdate()
        {
            if (this.shouldMoveSelectedSkillPointer == 1)
            {
                this.selectedSkillPointer.transform.position = this.transform.position;
                this.selectedSkillPointer.SetActive(true);
            }
            if (this.shouldMoveSelectedSkillPointer > 0)
            {
                this.shouldMoveSelectedSkillPointer -= 1;
            }
            
        }

        public override void Select(bool playSFX = true)
        {
            base.Select(playSFX: playSFX);
            this.isSelected = true;
            this.shouldMoveSelectedSkillPointer = 2;
            this.UpdateAppearance();
            SkillMenuSkillSlotUI.selectedSkill = this;
        }

        public override void Deselect()
        {
            base.Deselect();
            this.isSelected = false;
            this.UpdateAppearance();
        }
    }
}