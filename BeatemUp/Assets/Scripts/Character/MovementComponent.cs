using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody), typeof(CharacterAnimationComponent))]
    public class MovementComponent : MonoBehaviour
    {
        private Rigidbody _rb;
        private CharacterAnimationComponent _animationController;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animationController = GetComponent<CharacterAnimationComponent>();
        }

        /// <summary>
        /// Moves the rigidbody across the xz plane, maintaining the current rigidbody y motion
        /// </summary>
        /// <param name="movement"></param>
        public void Move2D(Vector2 movement)
        {
            var currY = _rb.linearVelocity.y;
            _rb.linearVelocity = new Vector3(movement.x, currY, movement.y);

            if (Mathf.Abs(movement.x) <= float.Epsilon)
                return;
            
            _animationController.FaceDirection(movement);
        }

        /// <summary>
        /// Dampens the velocity of the game object across the xz plane
        /// </summary>
        /// <param name="damping">Damping value. Clamped between 0 and 1</param>
        public void DampenMotion2D(float damping)
        {
            damping = Mathf.Clamp01(damping);
            _rb.linearVelocity = new Vector3(
                _rb.linearVelocity.x * damping, 
                _rb.linearVelocity.y, 
                _rb.linearVelocity.z * damping
            );
        }
    }
}
