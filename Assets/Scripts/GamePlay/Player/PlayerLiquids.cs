

namespace GamePlay.Player
{
    /// <summary>
    /// 主角液体组件
    /// </summary>
    public class PlayerLiquids
    {
        // 液体槽
        public LiquidBase CurrentLiquid;
        public LiquidBase StandbyLiquid;

        /// <summary>
        /// 切换液体，备用槽液体必须为主动型
        /// </summary>
        public bool ChangeLiquid()
        {
            if(!StandbyLiquid || !StandbyLiquid.liquidConfig.IsActive)
                return false;

            LiquidBase tmp = CurrentLiquid;
            CurrentLiquid = StandbyLiquid;
            StandbyLiquid = tmp;
            return true;
        }
        /// <summary>
        /// 吸取液体，备用槽必须为空
        /// </summary>
        /// <param name="liquid"></param>
        public bool GetLiquid(LiquidBase liquid)
        {
            if(StandbyLiquid) 
                return false;

            StandbyLiquid = liquid;
            return true;
        }
        /// <summary>
        /// 倾倒液体，备用槽必须为非空
        /// </summary>
        public bool PoorOutLiquid()
        {
            if (!StandbyLiquid)
                return false;

            StandbyLiquid = null;
            return true;
        }
    }
}
