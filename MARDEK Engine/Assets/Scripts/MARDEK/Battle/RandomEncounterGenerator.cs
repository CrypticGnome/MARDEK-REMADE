using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MARDEK.Core;
using MARDEK.Movement;

namespace MARDEK.Battle
{
    public class RandomEncounterGenerator : MonoBehaviour
    {
        [SerializeField, HideInInspector] Movable movable;
        [SerializeField] EncounterSet areaEncounterSet = null;
        [SerializeField] int minSteps = 20;
        [SerializeField] int maxSteps = 30;
        [SerializeField] UnityEvent onTriggerBattle;
        [SerializeField] GameObject battleBackground;
        [SerializeField] PrefabSO currentBattleFieldBackground;
        int stepsTaken = 0;
        int requiredSteps;

        private void Start()
        {
            currentBattleFieldBackground.CurrentPrefab = battleBackground;
            if (areaEncounterSet == null)
            {
                enabled = false;
                return;
            }
            GenerateRequiredSteps();
            movable.OnEndMove += Step;
        }
        private void LateUpdate()
        {
            if (stepsTaken < requiredSteps)
                return;
            stepsTaken = 0;
            GenerateRequiredSteps();
            TriggerBattle();
        }
        [ContextMenu("Trigger Battle")]
        void TriggerBattle()
        {
            BattleManager.encounter = areaEncounterSet;
            onTriggerBattle.Invoke();
        }
        void Step()
        {
            if (PlayerLocks.isPlayerLocked)
                return;
            stepsTaken++;
        }
        void GenerateRequiredSteps()
        {
            requiredSteps = Random.Range(minSteps, maxSteps + 1);
        }
    }
}