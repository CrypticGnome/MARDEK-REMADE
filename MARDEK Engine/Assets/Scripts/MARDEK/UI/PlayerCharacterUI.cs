using System.Collections;
using System.Linq;
using MARDEK.Animation;
using MARDEK.Battle;
using MARDEK.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCharacterUI : MonoBehaviour, IPointerClickHandler
{
	public HeroBattleCharacter character { get; private set; }
	[SerializeField] GameObject basePanel;
	[SerializeField] Image elementImage;
	[SerializeField] UISpriteAnimator characterAnimator;
	[SerializeField] TextMeshProUGUI characterName;
	[SerializeField] HealthBar healthBar;
	[SerializeField] ManaBar manaBar;
	[SerializeField] ExperienceBar expBar;
	[SerializeField] TextMeshProUGUI expRewardText;
	[SerializeField] AnimationCurve expRewardBounce;
	[SerializeField] RectTransform expRewardRectTrandform;

	private void Start()
	{
		UpdateCharacter();
		if (character == null)
		{
			basePanel.SetActive(false);
			return;
		}
		manaBar.SetCharacter(character);
		healthBar.SetCharacter(character);
		expBar.SetCharacter(character);
		characterName.text = character.Name;
		elementImage.sprite = character.Profile.element.thinSprite;
		characterAnimator.ClipList = character.Profile.WalkSprites;
		characterAnimator.gameObject.SetActive(true);

		character.OnExperienceGained += IncreaseExperience;
		character.OnLeveledUp += IncreaseLevel;
	}

	void UpdateCharacter()
	{
		basePanel.SetActive(false);
		var index = transform.GetSiblingIndex();

		if (index < BattleManager.PlayerBattleParty.Count)
		{
			character = BattleManager.PlayerBattleParty[index];
			basePanel.SetActive(true);
		}
		else
		{
			character = null;
			basePanel.SetActive(false);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		BattleUIManager.Instance.InspectCharacter(character);
	}

	public void IncreaseExperience(int experience)
	{
		StartCoroutine(PlayExperienceText($"+{experience} EXP"));
	}

	public void IncreaseLevel()
	{
		StartCoroutine(PlayExperienceText("LEVEL UP!"));
	}

	private IEnumerator PlayExperienceText(string text)
	{
		const float FadeOutTime = 0.5f;
		expRewardText.enabled = true;
		expRewardText.text = text;
		expRewardText.alpha = 1;
		float time = 0;
		float animationDuration = expRewardBounce.keys.Last().time;
		while (time < animationDuration)
		{
			expRewardRectTrandform.position = new Vector3(0,expRewardBounce.Evaluate(time),0);
			float fadeOutStrength = (FadeOutTime - (animationDuration - time)) / FadeOutTime;
			expRewardText.alpha = 1 - Mathf.Clamp01(fadeOutStrength);

			yield return null;
			time += Time.deltaTime;
		}
		expRewardText.enabled = false;
		expRewardText.text = null;

	}
}
