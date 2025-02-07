using UnityEngine;

namespace Character
{
    [CreateAssetMenu(menuName = "Settings/Player", fileName = "new Player Settings")]
    public class PlayerSettingsSO : ScriptableObject
    {
        public int health;
        public float speed;
    }
}
