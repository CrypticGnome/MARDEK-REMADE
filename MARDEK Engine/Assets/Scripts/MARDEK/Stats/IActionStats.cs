using MARDEK.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionStats
{
     public int CurrentHP { get; set; }
     public int CurrentMP { get; set; }
     public int MaxHP { get; }
     public int MaxMP { get; }

     public int Attack {  get; }
     public int Defense { get; set; }
     public int MagicDefense { get; set; }
     public Absorbtions Absorbtions { get; set; }
     public int Strength { get; set; }
     public int Vitality { get; set; }
     public int Spirit { get; set; }
     public int Agility { get; set; }

     public Resistances Resistances { get; set; }
     public float ACT { get; set; } // I fucking hate this
     public int Accuracy { get; set; }
     public int CritRate { get; set; }
}
