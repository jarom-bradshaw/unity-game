using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody), typeof(CharacterAnimationComponent), typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerSettingsSO playerSettings;
        private int _maxHealth;
        private int _health;
        
        private Rigidbody _rb;
        private CharacterAnimationComponent _animations;

        private void Awake()
        {
            // setting component references
            _rb = GetComponent<Rigidbody>();
            _animations = GetComponent<CharacterAnimationComponent>();
        }

        private void Start()
        {
            // settings health
            _maxHealth = playerSettings.health;
            _health = playerSettings.health;
        }

        private void FixedUpdate()
        {
            HandleMovement(Time.deltaTime);
        }

        /// <summary>
        /// Handles player movement
        /// </summary>
        /// <param name="delta">Takes in delta time for movement standardization</param>
        private void HandleMovement(float delta)
        {
            var movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (movement.magnitude <= float.Epsilon)
            {
                DampenMotion2D(0.8f);
                _animations.ResetFromRun();
            }
            else
            {
                Move2D(movement * (playerSettings.speed * delta * 100));
                _animations.PlayAnimation("CharacterRun");
            }
        }
        
        /// <summary>
        /// Moves the rigidbody across the xz plane, maintaining the current rigidbody y motion
        /// </summary>
        /// <param name="movement"></param>
        private void Move2D(Vector2 movement)
        {
            var currY = _rb.linearVelocity.y;
            _rb.linearVelocity = new Vector3(movement.x, currY, movement.y);

            if (Mathf.Abs(movement.x) <= float.Epsilon)
                return;
            
            _animations.FaceDirection(movement);
        }

        /// <summary>
        /// Dampens the velocity of the game object across the xz plane
        /// </summary>
        /// <param name="damping">Damping value. Clamped between 0 and 1</param>
        private void DampenMotion2D(float damping)
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
