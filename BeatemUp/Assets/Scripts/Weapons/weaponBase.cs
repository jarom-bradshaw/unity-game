using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "new Weapon Settings", menuName = "Settings/Weapon", order = 0)]
    public class WeaponSettingsSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName;
        [TextArea]
        public string description;

        [Header("Stats")]
        public int Attack;
        public float Damage;
        public float Speed;
        public float range;
        public float attackDelay;
        public float knockback;
        public float critChance;
        public float critMultiplier = 2f;

        [Header("Attack Behavior")]
        public WeaponType weaponType;
        // If Melee...
        //public MeleeAttackType meleeAttackType;


        [Header("Misc")]
        public bool hasSecondaryAttack;

        public WeaponSettings AsStruct()
        {
            return new WeaponSettings
            {
                Attack = Attack,
                Damage = Damage,
                Speed = Speed,
                Range = range,
                AttackDelay = attackDelay,
                Knockback = knockback,
                CritChance = critChance,
                CritMultiplier = critMultiplier,
                //WeaponType = weaponType,
            };
        }
    }

    // Enums
    public enum WeaponType // Is this descriptive enough. Should we add more types?
    {
        Melee,
        Axe,
        Spear,
        MatchStick,
    }

    [System.Serializable]
    public struct WeaponSettings
    {
        public int Attack;
        public float Damage;
        public float Speed;
        public float Range;
        public float AttackDelay;
        public float Knockback;
        public float CritChance;
        public float CritMultiplier;
    }
}
