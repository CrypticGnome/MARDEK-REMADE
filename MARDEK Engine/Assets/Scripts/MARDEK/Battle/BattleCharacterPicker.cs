using MARDEK.Battle;
using MARDEK.Core;
using MARDEK.Skill;
using MARDEK.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
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
          PlayerControls playerControls;
          static List<BattleCharacter> Heroes => BattleManager.PlayerBattleParty;
          static List<BattleCharacter> Enemies => BattleManager.EnemyBattleParty;

          static int index = 0;
          /// <summary>
          /// Used to ensure that the action isn't invoked on a target on the same frame as an action is picked, as they both use the "interact" key
          /// </summary>
          float enabledTime;
          static int SelectedCharacterIndex
          {
               get { return index; }
               set
               {
                    index = value;
                    SelectedCharacter = EnemiesSelected ? Enemies[index] : Heroes[index];
               }
          }
          static int PositionIndex;
          public static bool EnemiesSelected;
          public static BattleCharacter SelectedCharacter { get; private set; }
          ApplyBattleAction action;

          int frameTimer;
          const int WaitForFramesToInitialise = 1;
          Action<InputAction.CallbackContext> invokeActionOnTargetAction;
          private void Awake()
          {
               playerControls = new PlayerControls();
               playerControls.Enable();
               invokeActionOnTargetAction = context => InvokeActionOnTarget();
          }
          private void OnEnable()
          {
               frameTimer = WaitForFramesToInitialise;
          }
          private void OnDisable()
          {
               playerControls.DefaultMap.Movement.started -= HandleSelection;
               playerControls.DefaultMap.Interact.started -= invokeActionOnTargetAction;
               action = null;
          }
          private void Update()
          {
               if (frameTimer-- != 0) return;
               // Delay the hooking up of events be one frame to ensure target selection isn't done on the same frame as the picker being enabled
               playerControls.DefaultMap.Movement.started += HandleSelection;
               playerControls.DefaultMap.Interact.started += invokeActionOnTargetAction;
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
                    SelectedCharacterIndex = ClampEnemiesIndex(SelectedCharacterIndex);
               }
               else if (value.x == 1 && EnemiesSelected)
               {
                    EnemiesSelected = false;
                    SelectedCharacterIndex = ClampHeroesIndex(SelectedCharacterIndex);
               }

               // Handle verticle input
               if (value.y == 1)
                    SelectedCharacterIndex = EnemiesSelected ? ClampEnemiesIndex(SelectedCharacterIndex + 1) : ClampHeroesIndex(SelectedCharacterIndex + 1);
               else if (value.y == -1)
                    SelectedCharacterIndex = EnemiesSelected ? ClampEnemiesIndex(SelectedCharacterIndex - 1) : ClampHeroesIndex(SelectedCharacterIndex - 1);

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
               SelectedCharacterIndex = 0;
               SetPosition();
          }

          int ClampHeroesIndex(int index) => Mathf.Clamp(index, 0, Heroes.Count - 1);
          int ClampEnemiesIndex(int index) => Mathf.Clamp(index, 0, Enemies.Count - 1);
          void SetPosition()
          {
               transform.localScale = EnemiesSelected ? new Vector3(-0.1f, 0.1f, 1f) : new Vector3(0.1f, 0.1f, 1f);
               Vector3 targetsPosition = SelectedCharacter.battleModel.transform.position;
               Vector3 scale = transform.localScale;
               Vector2 targetOffset = new Vector2(pointersPoint.localPosition.x * scale.x, pointersPoint.localPosition.y * scale.y);
               transform.Set2DPosition((Vector2)targetsPosition - targetOffset);
          }
     }
}
