using GamePlay.Configs;
using GamePlay.Player;

namespace GamePlay.Liquid
{
    public static class LiquidFactory
    {
        public static LiquidBase Create(LiquidConfig cfg, PlayerStats stats)
        {
            if (cfg == null) return null;
            LiquidBase liquid = cfg.Type switch
            {
                LiquidType.Cola => new Cola(),
                LiquidType.GuoJiao => new GuoJiao(),
                _ => null,
            };
            liquid?.Init(cfg, stats);      // 创建和装配绑在一起，调用方不会漏
            return liquid;
        }
    }
}