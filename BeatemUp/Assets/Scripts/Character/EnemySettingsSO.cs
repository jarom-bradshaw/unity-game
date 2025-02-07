using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "new Enemy Settings", menuName = "Settings / Enemy", order = 0)]
    public class EnemySettingsSO : ScriptableObject
    {
        public int Health;
        public float Speed;
    }

    public struct EnemySettings
    {
        public int Health;
        public float Speed;
    }

}
