using System.Collections;
using UnityEngine;

public class ReactionBar : MonoBehaviour
{
	[SerializeField] Canvas canvas;
	[SerializeField] RectTransform actionPoint;
	[SerializeField] GameObject actionElement;
	
	public void PlayReaction(ReactionParams param)
	{
		float pixelWidth = canvas.pixelRect.width;
		float reactionPixelWidth = pixelWidth * param.RelativeWidth;

		StartCoroutine(Reaction());

		IEnumerator Reaction()
		{
			Instantiate(actionElement);

			actionPoint.anchoredPosition = new Vector2(param.RelativeWidth * pixelWidth, 0);

			RectTransform actionRect = actionElement.GetComponent<RectTransform>();
			float startX = -reactionPixelWidth;
			float pixelVelocity = pixelWidth / param.TimeToCross;
			float x = startX;

			actionRect.anchoredPosition = new Vector2(x, 0);

			yield return new WaitForSeconds(param.DelaySeconds);

			while (x < pixelWidth)
			{
				yield return null;
				x += pixelVelocity / Time.deltaTime;
				actionRect.anchoredPosition = new Vector2(x, 0);
			}

			Destroy(actionElement);
		}
	}

	public class ReactionParams
	{
		public float RelativeWidth;
		public float TimeToCross;
		public float ReactionRelativePosition;
		public float DelaySeconds;
	}
}
