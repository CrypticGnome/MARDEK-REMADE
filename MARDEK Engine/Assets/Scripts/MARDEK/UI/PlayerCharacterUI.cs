using System;
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
	}

	void UpdateCharacter()
	{
		basePanel.SetActive(false);
		var index = transform.GetSiblingIndex();

		if (index < BattleManager.PlayerBattleParty.Count)
		{
			character = (HeroBattleCharacter)BattleManager.PlayerBattleParty[index];
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
		expRewardText.enabled = true;
		expRewardText.text = text;
		float time = 0;
		while (time < expRewardBounce.keys.Last().time)
		{
			expRewardRectTrandform.position = new Vector3(0,expRewardBounce.Evaluate(time),0);
			yield return null;
		}
		expRewardText.enabled = false;
		expRewardText.text = null;

	}
}
