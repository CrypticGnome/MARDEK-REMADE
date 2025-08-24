using MARDEK.Audio;
using MARDEK.CharacterSystem;
using MARDEK.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MARDEK.UI
{
    using Progress;
    public class PartySelection : MonoBehaviour
    {
        [SerializeField] List<CharacterSlotUI> selectedCharacterSlots;
        [SerializeField] List<CharacterSlotUI> unselectedCharacterSlots;
        [SerializeField] AudioObject rejectSound;
        [SerializeField] Sprite transparentSprite;
        [SerializeField] Image mouseCharacterImage;
          [SerializeField] PlayableCharacters playableCharacters;

        Character mouseCharacter;

        void OnEnable()
        {
               mouseCharacter = new();
            PlayerLocks.UISystemLock += 1;
            SetClickActions();
            RefreshSlots();
        }
        void SetClickActions()
        {
            for (int index = 0; index < selectedCharacterSlots.Count; index++)
            {
                int rememberIndex = index;
                selectedCharacterSlots[index].SetClickAction(() => HandleClick(rememberIndex, true));
            }
            for (int index = 0; index < unselectedCharacterSlots.Count; index++)
            {
                int rememberIndex = index;
                unselectedCharacterSlots[index].SetClickAction(() => HandleClick(rememberIndex, false));
            }
        }

        void HandleClick(int index, bool isSelected)
        {
            var onfieldCharacters = playableCharacters.Party.OnFieldCharacters;
            Character characterAtIndex = (isSelected ? onfieldCharacters : playableCharacters.BenchedCharacters)[index];
            if (characterAtIndex.isRequired)
            {
                AudioManager.PlaySoundEffect(rejectSound);
                return;
            }
            if (isSelected)
                onfieldCharacters[index] = characterAtIndex;
            else
                playableCharacters.BenchedCharacters[index] = characterAtIndex;

            mouseCharacter = characterAtIndex;
        }

        void OnDisable()
        {
            PlayerLocks.UISystemLock -= 1;
        }
        void Update()
        {
            mouseCharacterImage.transform.position = new Vector2(
                Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()
            );
        }
        void LateUpdate()
        {
            RefreshSlots();
        }
        public void RefreshSlots()
        {
               PartySO party = playableCharacters.Party;
            for (int index = 0; index < selectedCharacterSlots.Count; index++)
            {
                if(index < party.Count)
                    selectedCharacterSlots[index].SetCharacter(party[index]);
                else
                    selectedCharacterSlots[index].SetCharacter(null);
            }
            for (int index = 0; index < unselectedCharacterSlots.Count; index++)
            {
                if(index < playableCharacters.BenchedCharacters.Count)
                    unselectedCharacterSlots[index].SetCharacter(playableCharacters.BenchedCharacters[index]);
                else
                    unselectedCharacterSlots[index].SetCharacter(null);
            }

            if (mouseCharacter == null)
                mouseCharacterImage.sprite = transparentSprite;
            else
                mouseCharacterImage.sprite = CharacterSlotUI.GetSprite(mouseCharacter);
        }
    }
}
