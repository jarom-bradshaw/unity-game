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
        public MeleeAttackType meleeAttackType;
        // If Ranged...
        public bool usesAmmo;
        public int magazineSize;
        public float reloadTime;
        public AmmoType ammoType;
        public RangedAttackType rangedAttackType;

        [Header("Visual / Audio / Effects")]
        //public Texture2D weaponTexture; // your 2D texture if needed
        public Sprite icon;
        public GameObject weaponPrefab;
        public AudioClip attackSFX;
        public AudioClip reloadSFX;

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
                WeaponType = weaponType,
                UsesAmmo = usesAmmo,
                MagazineSize = magazineSize,
                ReloadTime = reloadTime,
                AmmoType = ammoType,
                HasSecondaryAttack = hasSecondaryAttack,
            };
        }
    }

    // Enums
    public enum WeaponType // Is this descriptive enough. Should we add more types?
    {
        Melee,
        Ranged,
        Magic
    }

    public enum AmmoType
    {
        None,
        Bullets,
        Arrows,
        Energy
    }

    public enum MeleeAttackType
    {
        None,
        LightSlash,
        HeavySlash,
        LightStab,
        HeavyStab,
        LightSlam,
        HeavySlam,
        Spin,
        RisingAttack,
        LeapingAttack,
        DashAttack,
        Block,
        Parry,
        Special
    }

    public enum RangedAttackType
    {
        None,
        SingleShot,
        Burst,
        Spray,
        Charge,
        Snipe,
        AOE,
        Grenade,
        Rocket,
        Special
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

        public WeaponType WeaponType;
        public bool UsesAmmo;
        public int MagazineSize;
        public float ReloadTime;
        public AmmoType AmmoType;

        public bool HasSecondaryAttack;
    }
}
