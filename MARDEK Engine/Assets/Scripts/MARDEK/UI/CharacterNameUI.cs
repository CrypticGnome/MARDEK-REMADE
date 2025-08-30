using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MARDEK.UI
{
    public class CharacterNameUI : MonoBehaviour
    {
        [SerializeField] EnemyCharacterUI characterUI;
        [SerializeField] Text characteName;

        void OnEnable()
        {
            var profile = characterUI.character?.Profile;
            if (profile == null)
                characteName.text = "Null";
            else
                characteName.text = profile.displayName;
        }
    }
}