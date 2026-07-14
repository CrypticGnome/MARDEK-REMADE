using MARDEK.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
	public class CharacterImageAnimation : MonoBehaviour
	{
		[SerializeField] CharacterSelectable characterSelectable;
		[SerializeField] Image characterImage;
		public MoveDirection movementSpriteAnimationDirection;
		[SerializeField] Sprite disabledSprite;

		public void Update()
		{
			if (characterSelectable.Character == null)
			{
				characterImage.sprite = disabledSprite;
				return;
			}

			var characterInfo = characterSelectable.Character.Profile;
			var clip = characterInfo.WalkSprites.GetClipByReference(movementSpriteAnimationDirection);
			var animRatio = Time.time % 1;
			characterImage.sprite = clip.GetSprite(animRatio);
		}
	}
}