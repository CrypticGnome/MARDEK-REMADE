using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Battle
{
    using CharacterSystem;

    [CreateAssetMenu(menuName = "MARDEK/Battle/EncounterSet")]
    public class EncounterSet : ScriptableObject
    {
        [SerializeField] List<WeightedEncounter> possibleEncounters = new List<WeightedEncounter>();

          [System.Serializable]
          class WeightedEncounter
          {
               [Range(1, 10)]
               public int EncounterWeight = 1;
               public Encounter Encounter;
          }


        public List<Character> InstantiateEncounter(out Encounter encounter)
        {
            List<Character> enemies = new List<Character>();
            var chosenEncounter = ChooseEncounter();
               encounter = chosenEncounter.Encounter;
            foreach(var enemyEntry in encounter.Enemies)
            {
                var enemyLevel = Random.Range(enemyEntry.minLevel, enemyEntry.maxLevel + 1);
                var enemy = enemyEntry.enemy.Clone(enemyLevel);
                enemies.Add(enemy);
            }
            return enemies;
        }

        WeightedEncounter ChooseEncounter()
        {
            var totalWeight = TotalEncounterWeight();
            var desiredWeight = Random.Range(0, totalWeight);
            var weight = 0;


            foreach (var encounter in possibleEncounters)
            {
                weight += encounter.EncounterWeight;
                if (desiredWeight < weight)
                    return encounter;
            }
            return null;
        }

        int TotalEncounterWeight()
        {
            int totalWeight = 0;
            foreach (var encounter in possibleEncounters)
                totalWeight += encounter.EncounterWeight;
            return totalWeight;
        }
    }
}