using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ReactionBar : MonoBehaviour
{
	// FormerlySerializedAs preserves existing scene wiring across the rename - the type of
	// each field is unchanged, only which one moves and which stays fixed has swapped.
	[FormerlySerializedAs("actionPoint")][SerializeField] RectTransform slider;
	[FormerlySerializedAs("actionElement")][SerializeField] GameObject reactionRegion;
	[SerializeField] Canvas canvas;

	PlayerControls playerControls;

	void Awake()
	{
		playerControls = new PlayerControls();
		playerControls.Enable();
		gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		playerControls.Dispose();
	}

	// Plays the sliding-marker minigame once, then reports through onResult whether Interact
	// was pressed while the slider overlapped the (stationary) reaction region. Callers
	// `yield return` this directly rather than firing it via StartCoroutine.
	public IEnumerator PlayReaction(ReactionParams param, Action<bool> onResult)
	{
		gameObject.SetActive(true);

		// anchoredPosition is measured in the canvas's own local RectTransform space, not
		// real screen pixels - canvas.pixelRect.width is the actual render-target size,
		// which only equals the local width when the Canvas Scaler is "Constant Pixel
		// Size". Under "Scale With Screen Size" (the usual choice for resolution-
		// independent UI), the two diverge whenever the real resolution isn't exactly the
		// reference resolution, so using pixelRect.width made the loop's exit condition
		// (and the slider's target travel distance) wrong.
		float canvasWidth = ((RectTransform)canvas.transform).rect.width;
		float reactionWidth = canvasWidth * param.RelativeWidth;
		float lineX = param.ReactionRelativePosition * canvasWidth;

		// The reaction region is stationary - positioned once at the target line and left
		// there for the duration of the reaction, rather than animated every frame.
		GameObject instance = Instantiate(reactionRegion, transform);
		instance.SetActive(true);
		RectTransform regionRect = instance.GetComponent<RectTransform>();
		regionRect.anchoredPosition = new Vector2(lineX, 0);

		// The slider is what moves now - starts off-canvas to the left and travels the
		// canvas's full width over TimeToCross seconds.
		float velocity = canvasWidth / param.TimeToCross;
		float x = -reactionWidth;
		slider.anchoredPosition = new Vector2(x, 0);

		yield return new WaitForSeconds(param.DelaySeconds);

		bool pressed = false;
		void OnPress(InputAction.CallbackContext ctx) => pressed = true;
		playerControls.DefaultMap.Interact.started += OnPress;

		while (x < canvasWidth && !pressed)
		{
			yield return null;
			x += velocity * Time.deltaTime;
			slider.anchoredPosition = new Vector2(x, 0);
		}

		playerControls.DefaultMap.Interact.started -= OnPress;

		bool success = pressed && Mathf.Abs(x - lineX) <= reactionWidth / 2f;
		Destroy(instance);
		gameObject.SetActive(false);
		onResult?.Invoke(success);
	}

	public class ReactionParams
	{
		public float RelativeWidth;
		public float TimeToCross;
		public float ReactionRelativePosition;
		public float DelaySeconds;
	}
}
