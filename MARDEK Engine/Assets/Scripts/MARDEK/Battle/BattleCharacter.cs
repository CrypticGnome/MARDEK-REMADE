using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using Core;
    using CharacterSystem;
    using MARDEK.Stats;
     using static Codice.CM.Common.CmCallContext;
     using UnityEngine.Profiling;

     public class BattleCharacter : IActionStats
     {
          public Character Character { get; private set; }
          public BattleModel battleModel = null;
          public string Name { get { return Character.Profile.displayName; } }

          public int CurrentHP { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int CurrentMP { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

          public int MaxHP => throw new System.NotImplementedException();

          public int MaxMP => throw new System.NotImplementedException();

          public int Attack => throw new System.NotImplementedException();

          public int Defense { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int MagicDefense { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public Absorbtions Absorbtions { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int Strength { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int Vitality { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int Spirit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int Agility { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public StatusEffects Resistances { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public float ACT { get ; set ;}
          public int Accuracy { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
          public int CritRate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }



          StatusEffects StatusBuildup;
          public bool stunned;




          public BattleCharacter(Character character, Vector3 position)
          {
               Character = Object.Instantiate(character);
               battleModel = Object.Instantiate(Character.Profile.BattleModelPrefab).GetComponent<BattleModel>();
               battleModel.SetBattlePosition(position);
               Character.CurrentHP = character.MaxHP;
               Character.CurrentMP = character.MaxMP;
          }

          public float GetActBuildRate()
          {
               return 2;
          }

          public void OnTurnStart()
          {
               TickStatusEffects();
          }
          void TickStatusEffects()
          {
               StatusEffects resistances = Character.Resistances;
               if (StatusBuildup.Poison > 0)
               {
                    Debug.Log($"{Character.Profile.displayName} is poisoned");
                    StatusBuildup.Poison -= resistances.Poison + 1;
                    Character.CurrentHP -= Mathf.RoundToInt((float)Character.MaxHP / 20 + 0.5f);
               }
               if (StatusBuildup.Paralysis > 0)
               {
                    Debug.Log($"{Character.Profile.displayName} is paralysed");
                    stunned = !stunned;
                    if (stunned)
                    {
                         StatusBuildup.Paralysis -= resistances.Paralysis + 1;
                    }
               }
               else
                    stunned = false;

               if (StatusBuildup.Blindness > 0)
                    StatusBuildup.Blindness -= resistances.Blindness + 1;
               if (StatusBuildup.Silence > 0)
                    StatusBuildup.Silence -= resistances.Silence + 1;
               if (StatusBuildup.Numbness > 0)
                    StatusBuildup.Numbness -= resistances.Numbness + 1;
               if (StatusBuildup.Curse > 0)
                    StatusBuildup.Curse -= resistances.Curse + 1;
               if (StatusBuildup.Confusion > 0)
                    StatusBuildup.Confusion -= resistances.Confusion + 1;
               if (StatusBuildup.Bleed > 0)
                    StatusBuildup.Bleed -= resistances.Bleed + 1;
               // Do not tick zombification
          }
     }
}