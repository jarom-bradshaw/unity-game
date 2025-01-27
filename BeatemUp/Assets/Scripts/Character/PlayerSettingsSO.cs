using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "new Player Settings", menuName = "Settings / Player", order = 0)]
    public class PlayerSettingsSO : ScriptableObject
    {
        public int Health;
        public float Speed;
        public float Damage;

        public PlayerSettings AsStruct()
        {
            return new PlayerSettings
            {
                Health = Health,
                Speed = Speed,
                Damage = Damage
            };
        }
    }
    
    public struct PlayerSettings
    {
        public int Health;
        public float Speed;
        public float Damage;
    }
}