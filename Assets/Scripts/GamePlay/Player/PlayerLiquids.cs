using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>
    /// 主角液体组件
    /// </summary>
    public class PlayerLiquids
    {
        // 液体槽
        public Dictionary<int, LiquidBase> Liquids = new(2);

        /// <summary>
        /// 吸取液体
        /// </summary>
        /// <param name="liquid"></param>
        /// <param name="pos"></param>
        public void ChangeLiquid(LiquidBase liquid, int pos)
        {
            if (Liquids.ContainsKey(pos))
            {
                Liquids[pos] = liquid;

                // 槽位状态变更

                // 技能更换

            }
        }
    }
}
