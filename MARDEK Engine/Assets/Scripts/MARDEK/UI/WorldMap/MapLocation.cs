using MARDEK.Core;
using MARDEK.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace MARDEK.UI
{
     [CreateAssetMenu(menuName ="MARDEK/World Map/Waypoint ")]
     public class MapLocation : AddressableScriptableObject
     {
          [SerializeField] Sprite nameSprite;

          [SerializeField] Sprite defaultIcon;
          [SerializeField] Sprite activeIcon;
          [SerializeField] Sprite disabledIcon;
          [SerializeField] Sprite path;
          public Sprite DefaultIcon { get { return defaultIcon; } }
          public Sprite ActiveIcon { get { return activeIcon; } }
          public Sprite DisabledIcon { get { return disabledIcon; } }
          public Sprite NameSprite { get { return nameSprite; } }
          public Sprite Path { get { return path; } }

          public SceneReference Scene;
          public bool Discovered;
          //[SerializeField] Availability availability;
          //
          //public enum Availability
          //{
          //     NotDiscovered,
          //     Discovered,
          //     Unavailable
          //}
     }
}