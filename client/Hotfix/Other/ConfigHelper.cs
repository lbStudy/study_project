using Config;
using UnityEngine;

namespace Hotfix
{
    static class ConfigHelper
    {
        public static soundconfig FindSoundConfig(int key1)
        {
            soundconfig sc = null;
            if (ConfigDataManager.Instance.soundconfigs.TryGetValue(key1, out sc) == false)
            {
                Debug.Log($"not exist key({key1}) in soundconfig");
            }
            return sc;
        }
    }
}
