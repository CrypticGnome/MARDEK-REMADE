using MARDEK.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI
{
	public class ArtefactDetails : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI title;
		[SerializeField] Image image;
		[SerializeField] TextMeshProUGUI description;

		public void SetArtefact(EncyclopediaArtefact artefact)
		{
			title.text = artefact.displayName;
			image.sprite = artefact.image;
			description.text = artefact.description;
		}
	}
}
