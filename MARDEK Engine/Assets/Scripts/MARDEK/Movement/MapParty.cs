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
    public class MapParty : AddressableMonoBehaviour
    {
        static MapParty instance;
        static bool forceLoadOnNextAwake = false;
        [SerializeField, FullSerializer.fsIgnore] List<GameObject> inMapCharacters = new List<GameObject>();

        [SerializeField] List<Vector2> partyPositions = new List<Vector2>();
        [SerializeField] List<MoveDirection> partyDirections = new List<MoveDirection>();
        [SerializeField] SpriteRenderer[] renderers;
        [SerializeField] SpriteAnimator[] animators;
        [SerializeField] Movable[] movables;
        [SerializeField] PartySO currentParty;

        List<Vector2> GetPartyPosition()
        {
            if (instance == null) return null;
            List<Vector2> pos = new List<Vector2>();
            foreach (var character in instance.inMapCharacters)
                pos.Add(character.transform.position);
            return pos;
        }
        List<MoveDirection> GetPartyDirections()
        {
            if (instance == null) return null;
            List<MoveDirection> directions = new List<MoveDirection>();
            for (int i = 0; i < 4; i++)
                directions.Add(movables[i].currentDirection);
            return directions;
        }

        public override void Save()
        {
            partyPositions = GetPartyPosition();
            partyDirections = GetPartyDirections();
            base.Save();
        }

        override protected void Awake()
        {
            instance = this;

            if (forceLoadOnNextAwake)
            {
                Load();
                forceLoadOnNextAwake = false;
            }
            if (partyPositions.Count > 0)
                PositionCharactersAt(partyPositions, partyDirections);
            
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
                 MoveDirection direction = movables[i].currentDirection;
                 SpriteAnimationClip clip = direction == null ? animationClips.GetClipByIndex(0) : animationClips.GetClipByReference(direction);
                 renderers[i].sprite = clip.GetSprite(0);
                 animators[i].ClipList = animationClips;
            }
          }

        void PositionCharactersAt(List<Vector2> positions, List<MoveDirection> directions)
        {
            for (int i = 0; i < inMapCharacters.Count; i++)
            {
                GameObject character = inMapCharacters[i];
                if (character == null)
                    continue;
                
                var position = i < positions.Count ? positions[i] : positions[positions.Count - 1];
                character.transform.Set2DPosition(position);
                if (directions != null && directions.Count > 0)
                {
                    var direction = i < directions.Count ? directions[i] : directions[directions.Count - 1];
                    movables[i].FaceDirection(direction);
                }
                
            }
        }

        public void SetForceLoadOnNextAwake()
        {
            forceLoadOnNextAwake = true;
        }

        public static void OverrideAfterTransition(List<Vector2> positions, List<MoveDirection> directions)
        {
            instance.PositionCharactersAt(positions, directions);
        }
    }
}