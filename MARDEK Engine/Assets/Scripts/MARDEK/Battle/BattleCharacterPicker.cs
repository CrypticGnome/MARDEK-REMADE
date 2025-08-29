using MARDEK.Battle;
using MARDEK.Core;
using MARDEK.Skill;
using MARDEK.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MARDEK.UI
{
    public class BattleCharacterPicker : MonoBehaviour
    {
          [SerializeField] new Transform transform;
          [SerializeField] Transform pointersPoint;
          [SerializeField] SpriteRenderer crystalPointerRenderer;
          [SerializeField] GameObject lowerBar;
          [SerializeField] GameObject turnDisplay;
          PlayerControls playerControls;
          static List<BattleCharacter> Heroes => BattleManager.PlayerBattleParty;
          static List<BattleCharacter> Enemies => BattleManager.EnemyBattleParty;

          static int index = 0;
          /// <summary>
          /// Used to ensure that the action isn't invoked on a target on the same frame as an action is picked, as they both use the "interact" key
          /// </summary>
          float enabledTime;
          static int PositionIndex;
          public static bool EnemiesSelected;
          public static BattleCharacter SelectedCharacter { get; private set; }
          ApplyBattleAction action;

          int frameTimer;
          const int WaitForFramesToInitialise = 1;
          Action<InputAction.CallbackContext> invokeActionOnTargetAction;
          Action<InputAction.CallbackContext> invokeCancellation;

          private void Awake()
          {
               playerControls = new PlayerControls();
               playerControls.Enable();
               invokeActionOnTargetAction = context => InvokeActionOnTarget();
               invokeCancellation = context => HandleCancellation();
               void HandleCancellation()
               {
                    lowerBar.SetActive(true);
                    gameObject.SetActive(false);
               }
          }
          private void OnEnable()
          {
               frameTimer = WaitForFramesToInitialise;
          }
          private void OnDisable()
          {
               playerControls.DefaultMap.Movement.started -= HandleSelection;
               playerControls.DefaultMap.Interact.started -= invokeActionOnTargetAction;
               playerControls.DefaultMap.Cancel.performed -= invokeCancellation;
               turnDisplay.SetActive(true);

               action = null;
          }
          private void Update()
          {
               if (frameTimer-- != 0) return;
               // Delay the hooking up of events be one frame to ensure target selection isn't done on the same frame as the picker being enabled
               playerControls.DefaultMap.Movement.started += HandleSelection;
               playerControls.DefaultMap.Interact.started += invokeActionOnTargetAction;
               playerControls.DefaultMap.Cancel.performed += invokeCancellation;

          }
          private void OnDestroy()
          {
               playerControls.Dispose();
          }

          public void HandleSelection(InputAction.CallbackContext context)
          {
               if (!enabled)
                    return;

               var value = context.ReadValue<Vector2>();
               if (value.Equals(Vector2.zero)) return;

               // Handle horizontal input
               if (value.x == -1 && !EnemiesSelected)
               {
                    EnemiesSelected = true;
                    PositionIndex = ClampEnemiesIndex(PositionIndex);
               }
               else if (value.x == 1 && EnemiesSelected)
               {
                    EnemiesSelected = false;
                    PositionIndex = ClampHeroesIndex(PositionIndex);
               }

               // Handle verticle input (ordered from top to bottom)
               int previousIndex = PositionIndex;
               if (value.y == -1)
                    PositionIndex = EnemiesSelected ? ClampEnemiesIndex(PositionIndex + 1) : ClampHeroesIndex(PositionIndex + 1);
               else if (value.y == 1)
                    PositionIndex = EnemiesSelected ? ClampEnemiesIndex(PositionIndex - 1) : ClampHeroesIndex(PositionIndex - 1);


               SelectedCharacter = EnemiesSelected ? 
                    Enemies.OrderByDescending(e => e.battleModel.transform.position.y).ElementAt(PositionIndex) :
                    Heroes.OrderByDescending(e => e.battleModel.transform.position.y).ElementAt(PositionIndex);


               SetPosition();
          }
          public void InvokeActionOnTarget()
          {
               if (Time.time == enabledTime) return;
               BattleManager.PerformActionToTarget(action, SelectedCharacter);
               gameObject.SetActive(false);
          }
          public void EnableWithAction(ApplyBattleAction action)
          {
               // Currently assume that the target is the enemy, may want to pass in a parameter to say which team the action should default to at some point
               enabledTime = Time.time;
               lowerBar.SetActive(false);

               gameObject.SetActive(true);
               this.action = action;
               EnemiesSelected = true;
               SelectedCharacter = Enemies[0];
               SetPosition();
               turnDisplay.SetActive(false);
          }

          int ClampHeroesIndex(int index) => Mathf.Clamp(index, 0, Heroes.Count - 1);
          int ClampEnemiesIndex(int index) => Mathf.Clamp(index, 0, Enemies.Count - 1);
          void SetPosition()
          {
               BattleModelComponent target = SelectedCharacter.battleModel;
               transform.Set2DPosition(target.CrystalPointerGoToPosition.position);
               transform.localScale = EnemiesSelected ? new Vector3(-0.1f, 0.1f, 1f) : new Vector3(0.1f, 0.1f, 1f);

          }
     }
}
