using MARDEK.Animation;
using MARDEK.CharacterSystem;
using MARDEK.Core;
using MARDEK.Progress;
using MARDEK.Save;
using System.Collections.Generic;
using UnityEngine;

namespace MARDEK.Movement
{
    [SelectionBase]
    public class MapParty : MonoBehaviour
    {
          public static bool Loaded;
          public static MapParty Instance;
        static bool forceLoadOnNextAwake = false;
        [SerializeField, FullSerializer.fsIgnore] List<GameObject> inMapCharacters = new List<GameObject>();

        [SerializeField] CharacterPositions positions;
        [SerializeField] SpriteRenderer[] renderers;
        [SerializeField] SpriteAnimator[] animators;
        [SerializeField] Movable[] movables;
        [SerializeField] PartySO currentParty;


        List<Vector2> GetPartyPosition()
        {
            List<Vector2> pos = new List<Vector2>();
            foreach (var character in inMapCharacters)
                pos.Add(character.transform.position);
            return pos;
        }
        List<MoveDirection> GetPartyDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>();
            for (int i = 0; i < 4; i++)
                directions.Add(movables[i].currentDirection);
            return directions;
        }

          private void OnDestroy()
          {
               positions.Positions = GetPartyPosition().ToArray();
               positions.Directions = GetPartyDirections().ToArray();
          }
          void Awake()
          {
               Instance = this;

               if (!Loaded)
                {
                     for (int i = 0; i < 4; i++)
                          positions.Positions[i] = (Vector2)transform.position;
                    Loaded = true;
                }


               for (int i = 0; i < 4; i++)
               {
                    Character character = i < currentParty.Count ? currentParty[i] : null;
               
                    if (character == null)
                    {
                         inMapCharacters[i].SetActive(false);
                         continue;
                    }
                    inMapCharacters[i].SetActive(true);
               
                    SpriteAnimationClipList animationClips = character.Profile.WalkSprites;
                    MoveDirection direction = positions.Directions[i];
                    SpriteAnimationClip clip = direction == null ? animationClips.GetClipByIndex(0) : animationClips.GetClipByReference(direction);
                    renderers[i].sprite = clip.GetSprite(0);
                    animators[i].ClipList = animationClips;
               }
               PositionCharactersAt(positions.Positions, positions.Directions);

          }

          void PositionCharactersAt(Vector2[] positions, MoveDirection[] directions)
        {
            for (int i = 0; i < inMapCharacters.Count; i++)
            {
                GameObject character = inMapCharacters[i];
                if (character == null)
                    continue;
                
                var position = i < positions.Length ? positions[i] : positions[positions.Length - 1];
                character.transform.Set2DPosition(position);
                if (directions != null && directions.Length > 0)
                {
                    var direction = i < directions.Length ? directions[i] : directions[directions.Length - 1];
                    if (direction == null) continue;
                    movables[i].FaceDirection(direction);
                }
                
            }
        }
          void PositionCharactersAt(Vector2 position, MoveDirection direction)
          {
               for (int i = 0; i < inMapCharacters.Count; i++)
               {
                    GameObject character = inMapCharacters[i];
                    if (character == null)
                         continue;

                    character.transform.Set2DPosition(position);
                    if (direction != null)
                    {
                         movables[i].FaceDirection(direction);
                    }
               }
          }
          public void SetForceLoadOnNextAwake()
        {
            forceLoadOnNextAwake = true;
        }

        public static void OverrideAfterTransition(Vector2 position, MoveDirection direction)
        {
            Instance.PositionCharactersAt(position, direction);
        }
    }
}