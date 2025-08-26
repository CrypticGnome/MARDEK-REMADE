using UnityEngine;
using MARDEK.Core;

namespace MARDEK.Animation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        public static SpriteAnimator PlayerSpriteAnimator { get; private set; }
        [SerializeField] float animationSpeed = 1f;
        [SerializeField] bool _isAnimating = false;
        public SpriteAnimationClipList ClipList = null;
        
        public bool isAnimating { get { return _isAnimating; } private set { _isAnimating = value; } }
        public bool currentClipLoops
        {
            get
            {
                if (currentClip == null)
                    return false;
                return currentClip.loop;
            }
        }
        
        SpriteAnimationClip currentClip = null;
        [HideInInspector] [SerializeField] SpriteRenderer spriteRenderer = null;
        float animationRatio = 0f;

        public void SetAsPlayerAnimator()
        {
            PlayerSpriteAnimator = this;
        }

        private void OnValidate()
        {
            InitializeFields();
        }

        private void Awake()
        {
            
        }

        void InitializeFields()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentClip = ClipList?.GetClipByIndex(0);

            if (currentClip != null && spriteRenderer.sprite == null)
                    UpdateSprite(0);
        }
          private void Start()
          {
               //InitializeFields();
               if (currentClip is null)
               {
                    Debug.LogWarning($"Null current clip on {name}");
                    enabled = false;
               }
          }
          private void Update()
          {
               if (!isAnimating) return;
               
               animationRatio += animationSpeed * Time.deltaTime;
               bool endAnimation = !currentClip.loop && animationRatio > 1;
               if (endAnimation)
               {
                   isAnimating = false;
                   animationRatio = 0;
               }
               else
               {
                   UpdateSprite(animationRatio);
                   if (animationRatio > 1)
                       animationRatio = 0;
               }
          }

        void UpdateSprite(float _animationRatio)
        {
               spriteRenderer.sprite = currentClip.GetSprite(_animationRatio);
        }

        public void StopCurrentAnimation(float forceAnimationRatio)
        {
            StopCurrentAnimation();
            animationRatio = forceAnimationRatio;
            UpdateSprite(animationRatio);
        }

        public void StopCurrentAnimation()
        {
            isAnimating = false;
        }

        public void PlayClipByMoveDirectionReference(MoveDirection reference)
        {
            if(reference == null)
            {
                StopCurrentAnimation(1);
                return;
            }
            SpriteAnimationClip nextClip = ClipList.GetClipByReference(reference);
            currentClip = nextClip;
            isAnimating = true;                    
            animationRatio = 0;
        }
    }
}