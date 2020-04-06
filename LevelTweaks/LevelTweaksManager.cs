using UnityEngine;

namespace LevelTweaks
{
    public class LevelTweaksManager : MonoBehaviour
    {
        private static LevelTweaksManager _instance;
        public static LevelTweaksManager Instance
        {
            get
            {
                if (!_instance)
                    DontDestroyOnLoad(_instance = new GameObject("Level Tweaks Manager").AddComponent<LevelTweaksManager>());
                return _instance;
            }
        }
    }
}
