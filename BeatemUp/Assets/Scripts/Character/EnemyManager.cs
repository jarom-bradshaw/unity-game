using System;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(HealthComponent), typeof(MovementComponent))]
    public class EnemyManager : MonoBehaviour
    {
        private HealthComponent _healthComponent;
        private MovementComponent _movementComponent;

        [SerializeField] public EnemySettingsSO enemySettingsIn;
        private EnemySettings _enemySettings;

        private void Awake()
        {
            _healthComponent = GetComponent<HealthComponent>();
            _movementComponent = GetComponent<MovementComponent>();
        }

        private void Start()
        {

            _healthComponent.SetMaxHealth(_enemySettings.Health);
        }

        public void FixedUpdate()
        {
            var movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (movement.magnitude <= float.Epsilon)
            {
                _movementComponent.DampenMotion2D(0.8f);
                return;
            }

            _movementComponent.Move2D(movement * (_enemySettings.Speed * Time.fixedDeltaTime * 50));
        }
    }
}