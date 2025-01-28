using System;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(HealthComponent), typeof(MovementComponent))]
    public class PlayerManager : MonoBehaviour
    {
        private HealthComponent _healthComponent;
        private MovementComponent _movementComponent;

        [SerializeField] public PlayerSettingsSO playerSettingsIn;
        private PlayerSettings _playerSettings;
        
        [SerializeField] private float characterHeight = 2f;

        private void Awake()
        {
            _healthComponent = GetComponent<HealthComponent>();
            _movementComponent = GetComponent<MovementComponent>();
        }

        private void Start()
        {
            _playerSettings = playerSettingsIn.AsStruct();
            
            _healthComponent.SetMaxHealth(_playerSettings.Health);
        }

        public void FixedUpdate()
        {
            var movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (movement.magnitude <= float.Epsilon)
                _movementComponent.DampenMotion2D(0.8f);
            
            _movementComponent.Move2D(movement * (_playerSettings.Speed * Time.fixedDeltaTime * 100));
        }
    }
}
