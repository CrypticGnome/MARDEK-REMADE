using UnityEngine;

namespace MARDEK.Audio
{
    public abstract class AudioObject : ScriptableObject
    {
        [SerializeField] protected AudioClip clip = default;
        #if UNITY_EDITOR
          [SerializeField, Multiline] string description;
        #endif
        public abstract void PlayOnSource(AudioSource audioSource);
        public AudioClip Clip => clip;
    }
}