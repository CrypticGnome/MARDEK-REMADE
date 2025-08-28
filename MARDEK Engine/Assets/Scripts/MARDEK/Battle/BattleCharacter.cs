using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using Core;
    using CharacterSystem;
    using MARDEK.Stats;
     using MARDEK.UI;

     public abstract class BattleCharacter : IActionStats
     {
          public CharacterProfile Profile { get; protected set; }
          public BattleModelComponent battleModel = null;
          public ActionSkillset Skillset { get; protected set; }
          public string Name { get { return Profile.displayName; } }

          public int CurrentHP { get; set; }
          public int CurrentMP { get; set ; }
          public float ACT { get; set; }
          public CoreStats BaseStats { get { return Profile.Stats; } }
          public CoreStats VolatileStats { get;  set; }

          public int MaxHP => VolatileStats.MaxHP;

          public int MaxMP => VolatileStats.MaxMP;

          public int Attack => VolatileStats.Attack;

          public int Defense { get => VolatileStats.Defense; set => VolatileStats.Defense = value; }
          public int MagicDefense { get => VolatileStats.MagicDefense; set => VolatileStats.MagicDefense = value; }
          public Absorbtions Absorbtions { get => VolatileStats.Absorbtions; set => VolatileStats.Absorbtions = value; }
          public int Strength { get => VolatileStats.Strength; set => VolatileStats.Strength = value; }
          public int Vitality { get => VolatileStats.Vitality; set => VolatileStats.Vitality = value; }
          public int Spirit { get => VolatileStats.Spirit; set => VolatileStats.Spirit = value; }
          public int Agility { get => VolatileStats.Agility; set => VolatileStats.Agility = value; }
          public StatusEffects Resistances { get => VolatileStats.Resistances; set => VolatileStats.Resistances = value; }
          public int Accuracy { get => VolatileStats.Accuracy; set => VolatileStats.Accuracy = value; }
          public int CritRate { get => VolatileStats.CritRate; set => VolatileStats.CritRate = value; }

          public delegate void StatChanged();
          public event StatChanged OnStatChanged;
          public int Level 
          { get; 
          protected set ; }

          public StatusEffects StatusBuildup = new StatusEffects();
          public bool stunned;




          public float ActBuildRate()
          {
               float actRate = 1 + 0.05f * VolatileStats.Agility;
               actRate *= 1000;
               return actRate;
          }

          public void OnTurnStart()
          {
               TickStatusEffects();
          }
          public void TickStatusEffects()
          {
               StatusEffects resistances = Profile.Stats.Resistances;
               if (StatusBuildup.Poison > 0)
               {
                    StatusBuildup.Poison -= resistances.Poison + 1;
                    int damage = Mathf.RoundToInt((float)BaseStats.MaxHP / 20 + 0.5f);
                    CurrentHP -= damage;
                    Debug.Log($"{Profile.displayName} is poisoned and takes {damage} damage");

               }
               if (StatusBuildup.Paralysis > 0)
               {
                    stunned = !stunned;
                    StatusBuildup.Paralysis -= resistances.Paralysis + 1;
                    Debug.Log($"{Profile.displayName} is paralysed and has {StatusBuildup.Paralysis} paralysis build up");

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