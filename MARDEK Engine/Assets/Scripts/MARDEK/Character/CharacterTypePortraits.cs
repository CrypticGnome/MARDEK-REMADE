using System;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.CharacterSystem
{
     [CreateAssetMenu(fileName = "CharacterTypePortrait", menuName = "Singeltons/CharacterTypePortraits")]
     public class CharacterTypePortraits : ScriptableObject
     {
          [SerializeField] CharacterTypePortrait[] characterTypeSprites;
          public CharacterTypePortrait[] CharacterSprites => characterTypeSprites;

          public static Dictionary<CharacterType, Sprite> CharacterTypeSprites;

          [RuntimeInitializeOnLoadMethod]
          static void OnGameInitialisation()
          {
               var instance = Resources.Load<CharacterTypePortraits>("CharacterTypeSprites");
               CharacterTypeSprites = new Dictionary<CharacterType, Sprite>();
               foreach (CharacterTypePortrait character in instance.CharacterSprites)
                    CharacterTypeSprites.Add(character.Type, character.Sprite);
          }

          [Serializable]
          public class CharacterTypePortrait
          {
               public CharacterType Type;
               public Sprite Sprite;
          }
     }
}