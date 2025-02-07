using System;
using UnityEngine;

namespace Character
{
    public enum AnimationPriority
    {
        None,
        Normal,
        Locked,
        Priority
    }
    
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationComponent : MonoBehaviour
    {
        private Animator _animator;
        private string _currentAnimation;
        private AnimationPriority _currentAnimationPriority;

        private bool _comboWindowOpen = false;
        
        public string CurrentAnimation => _currentAnimation;
        public bool ComboWindowOpen => _comboWindowOpen;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _currentAnimation = "None";
        }

        private void Start()
        {
            PlayAnimation("CharacterIdle");
        }

        private void Update()
        {
            if (_currentAnimationPriority == AnimationPriority.None)
                PlayAnimation("CharacterIdle");
        }

        /// <summary>
        /// Plays a normal animation, overriding any low-priority or unlocked animation types
        /// </summary>
        /// <param name="anim">Animation state to play</param>
        /// <param name="crossFade">Fade time from current animation</param>
        public void PlayAnimation(string anim, float crossFade = 0.1f)
        {
            if (_currentAnimation == anim || _currentAnimationPriority >= AnimationPriority.Locked)
                return;
            
            ChangeAnimation(anim, crossFade);
        }

        /// <summary>
        /// Plays a locked animation, stopping animation override
        /// </summary>
        /// <param name="anim">Animation state to play</param>
        /// <param name="crossFade">Fade time from current animation</param>
        public void PlayLockedAnimation(string anim, float crossFade = 0.1f)
        {
            if (_currentAnimation == anim || _currentAnimationPriority >= AnimationPriority.Locked)
                return;
            
            ChangeAnimation(anim, crossFade, AnimationPriority.Locked);
        }

        /// <summary>
        /// Plays a priority animation, overriding any other current animation no matter its type
        /// </summary>
        /// <param name="anim">Animation state to play</param>
        /// <param name="crossFade">Fade time from current animation</param>
        public void PlayPriorityAnimation(string anim, float crossFade = 0.1f)
        {
            if (_currentAnimation == anim)
                return;
            
            ChangeAnimation(anim, crossFade, AnimationPriority.Priority);
        }

        private void ChangeAnimation(string anim, float crossFade = 0.1f,
            AnimationPriority animationPriority = AnimationPriority.Normal)
        {
            _comboWindowOpen = false;
            
            _animator.CrossFade(anim, crossFade);
            _currentAnimation = anim;
            _currentAnimationPriority = animationPriority;
        }

        /// <summary>
        /// Scales the character to look in a given direction
        /// </summary>
        /// <param name="dir">Direction to face; used the sign of the X-axis motion</param>
        public void FaceDirection(Vector2 dir)
        {
            var currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(dir.x);
            transform.localScale = currentScale;
        }

        public void ResetAnimation()
        {
            _currentAnimation = "None";
            _currentAnimationPriority = AnimationPriority.None;
        }

        public void ResetFromRun()
        {
            if (_currentAnimation == "CharacterRun")
                ResetAnimation();
        }

        /// <summary>
        /// Opens the window for the animation to Combo. Usually called from within an animation trigger
        /// </summary>
        public void OpenComboWindow()
        {
            _comboWindowOpen = true;
        }
    }
}
