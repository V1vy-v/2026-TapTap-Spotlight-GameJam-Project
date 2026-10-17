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
                LiquidType.LeiBi => new LeiBi(),
                LiquidType.SanQuan => new SanQuan(),
                LiquidType.LiaoJiu => new StandbyPassiveLiquid(),
                LiquidType.BingMeiShi => new StandbyPassiveLiquid(),
                _ => null,
            };
            liquid?.Init(cfg, stats);
            return liquid;
        }
    }
}