using System;
using UnityEngine;

namespace Character
{
    /// <summary>
    /// A component to track the health of a game object
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [Min(1)] [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        public event Action OnDeath;
        private bool _maxHealthManuallySet = false;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Sets the max health of the component manually
        /// </summary>
        /// <param name="health"></param>
        public void SetMaxHealth(int health)
        {
            if (_maxHealthManuallySet)
                Debug.LogWarning($"{gameObject.name} is setting maxHealth more than once");
        
            maxHealth = health;
            currentHealth = health;
            _maxHealthManuallySet = true;
        }

        /// <summary>
        /// Deals damage to the health component
        /// </summary>
        /// <param name="damage">Damage to deal. Only has an effect if the damage value is positive.</param>
        public void Damage(int damage)
        {
            if (damage < 0 || currentHealth <= 0)
                return;
        
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth <= 0)
                OnDeath?.Invoke();
        }

        /// <summary>
        /// Heals the health component
        /// </summary>
        /// <param name="heal">Amount of health to restore. Only has an effect if the heal value is positive</param>
        public void Heal(int heal)
        {
            if (heal < 0)
                return;
        
            currentHealth += heal;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        /// <summary>
        /// Heals the health component to max value
        /// </summary>
        public void MaxHeal()
        {
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Sets the health component health to 0, immediately killing it
        /// </summary>
        public void Kill()
        {
            if (currentHealth <= 0)
            {
                Debug.LogWarning($"{gameObject.name} is already dead");
                return;
            }
        
            currentHealth = 0;
            OnDeath?.Invoke();
        }
    }
}
