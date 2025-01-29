using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationComponent : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Scales the character to look in a given direction
        /// </summary>
        /// <param name="dir"></param>
        public void FaceDirection(Vector2 dir)
        {
            var currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(dir.x);
            transform.localScale = currentScale;
        }
    }
}
