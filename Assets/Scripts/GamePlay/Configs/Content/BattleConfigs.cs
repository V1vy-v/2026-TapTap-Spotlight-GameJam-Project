using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace GamePlay.Session
{
    public static class BattleConfigs
    {
        private static readonly Dictionary<string, BattleConfig> _cache = new();

        public static BattleConfig Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (_cache.TryGetValue(id, out var cached)) return cached;

            // 测试期资产放在 Assets/Resources 下，它不在 YooAsset 采集范围内。
            var config = Resources.Load<BattleConfig>(id);

            if (config == null)
            {
                var handle = Global.Load<BattleConfig>(id);
                config = handle?.AssetObject as BattleConfig;
            }

            if (config != null) _cache[id] = config;
            return config;
        }
    }
}