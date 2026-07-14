using MARDEK.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
	public class PlaceDetails : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI title;
		[SerializeField] Image landscape;
		[SerializeField] TextMeshProUGUI description;

		public void SetPlace(EncyclopediaPlace place)
		{
			title.text = place.displayName;
			landscape.sprite = place.landscape;
			description.text = place.description;
		}
	}
}
