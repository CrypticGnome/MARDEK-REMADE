using MARDEK.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace MARDEK.UI
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] UnityEvent OnSelected = new UnityEvent();
        [SerializeField] UnityEvent OnDeselected = new UnityEvent();
        [SerializeField] bool deselectOnDisable = false;
        [SerializeField] AudioObject selectionSFX;
          bool selected;
          public bool Selected => selected;
        public virtual bool IsValid()
        {
            return gameObject.activeSelf;
        }
        public virtual void Select(bool playSFX = true)
        {
               selected = true;
            OnSelected.Invoke();
            if(playSFX && selectionSFX)
                AudioManager.PlaySoundEffect(selectionSFX);
        }

        public virtual void Deselect()
        {
               selected = false;
            OnDeselected.Invoke();
        }

        private void OnDisable()
        {
            if (deselectOnDisable)
                Deselect();
        }
    }
}