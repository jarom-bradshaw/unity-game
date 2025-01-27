using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementComponent : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Moves the game object, setting its linear velocity
        /// </summary>
        /// <param name="movement"></param>
        public void Move(Vector3 movement)
        {
            rb.linearVelocity = movement;
        }

        /// <summary>
        /// Moves the rigidbody across the xz plane, maintaining the current rigidbody y motion
        /// </summary>
        /// <param name="movement"></param>
        public void Move2D(Vector2 movement)
        {
            var currY = rb.linearVelocity.y;
            rb.linearVelocity = new Vector3(movement.x, currY, movement.y);
        }

        /// <summary>
        /// Dampens the velocity of the game object
        /// </summary>
        /// <param name="damping">Damping value. Clamped between 0 and 1</param>
        public void DampenMotion(float damping)
        {
            rb.linearVelocity *= Mathf.Clamp01(damping);
        }

        /// <summary>
        /// Dampens the velocity of the game object across the xz plane
        /// </summary>
        /// <param name="damping">Damping value. Clamped between 0 and 1</param>
        public void DampenMotion2D(float damping)
        {
            damping = Mathf.Clamp01(damping);
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x * damping, 
                rb.linearVelocity.y, 
                rb.linearVelocity.z * damping
            );
        }
    }
}
