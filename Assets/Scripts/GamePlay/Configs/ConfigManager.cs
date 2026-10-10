using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "ConfigManager", menuName = "Configs/ConfigManager")]
    public class ConfigManager : ScriptableObjectSingleton<ConfigManager>
    {
        [SerializeField]
        private List<ConfigBase> _configs = new();
        private Dictionary<string, ConfigBase> _allConfigs;

        private void OnEnable()
        {
            _allConfigs = new();

            foreach(var config in _configs)
            {
                if (!_allConfigs.TryAdd(config.Id, config))
                    Debug.LogError($"[ConfigManager] 重复 ID: {config.Id}");
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool TryGetConfig<T>(string id, out T config) where T : ConfigBase
        {
            config = null;

            if (_allConfigs.TryGetValue(id, out ConfigBase value) && value is T target)
            {
                config = target;
                return true;
            }

            Debug.LogWarning($"[ConfigManager] 找不到配置：{id}");
            return false;
        }
    }
}
