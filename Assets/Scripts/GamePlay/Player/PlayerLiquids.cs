using GamePlay.Liquid;
using System;

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
        public event Action OnLiquidChanged;

        /// <summary>
        /// 切换液体，备用液体必须可作主形态
        /// </summary>
        public bool ChangeLiquid()
        {
            if(!StandbyLiquid || !StandbyLiquid.liquidConfig.CanBeMain)
                return false;

            LiquidBase tmp = CurrentLiquid;
            CurrentLiquid = StandbyLiquid;
            StandbyLiquid = tmp;

            OnLiquidChanged?.Invoke();
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
        public bool PourOutLiquid()
        {
            if (!StandbyLiquid)
                return false;

            StandbyLiquid = null;
            return true;
        }
    }
}
