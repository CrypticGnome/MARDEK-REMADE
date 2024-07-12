using MARDEK.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MaxHpCalculator : ScriptableObject
{
     public abstract int GetMaxHP(StatsClass stats);
}

public class DefaultMaxHP : MaxHpCalculator
{
     public override int GetMaxHP(StatsClass stats)
     {
          return stats.Vitality;
     }
}