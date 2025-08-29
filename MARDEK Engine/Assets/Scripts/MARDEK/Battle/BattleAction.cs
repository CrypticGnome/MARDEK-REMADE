using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MARDEK.Battle
{
     using MARDEK.Audio;
     using MARDEK.CharacterSystem;
     using MARDEK.Stats;
     using System;
     [Serializable]
     public class BattleAction
     {
          [SerializeField] Element element;
          [SerializeField] SoundEffect soundEffect;
          [SerializeReference, SubclassSelector] ActionEffects[] actionEffects;
          public Element Element { get { return element; } }
          public void Apply(BattleCharacter user, BattleCharacter target)
          {
               for (int index = 0; index < actionEffects.Length; index++)
                    actionEffects[index].ApplyEffect(user, target, element);
               AudioManager.PlaySoundEffect(soundEffect);
          }

  
          
     }
}