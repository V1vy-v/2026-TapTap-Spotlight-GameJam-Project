using System.Collections.Generic;
using Framework;

namespace GamePlay.Session
{
    public static class BattleConfigs
    {
        private static readonly Dictionary<string, BattleConfig> _cache = new();

        public static BattleConfig Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            // 缓存命中直接返回
            if (_cache.TryGetValue(id, out var cached)) return cached;

            // 按需加载
            var handle = Global.Load<BattleConfig>(id);
            var config = handle.AssetObject as BattleConfig;
            if (config != null) _cache[id] = config;
            return config;
        }
    }
}