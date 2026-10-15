using GamePlay.Configs;
using GamePlay.Liquid;
using System;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>
    /// 主角液体组件
    /// </summary>
    public class PlayerLiquids
    {
        private PlayerStats _stats;
        private float _cdRemain;
        private float _elapsed;
        // 液体槽
        public LiquidBase CurrentLiquid;
        public LiquidBase StandbyLiquid;

        #region 事件
        public event Action OnChangeLiquid;
        public event Action OnGainLiquid;
        public event Action OnPourOutLiquid;
        #endregion

        public void Init(PlayerStats stats)
        {
            _stats = stats;
        }

        /// <summary>
        /// 切换液体，备用液体必须可作主形态
        /// </summary>
        public bool ChangeLiquid()
        {
            if (StandbyLiquid == null || !StandbyLiquid.Config.CanBeMain)
                return false;
            if (_elapsed > 0)
                return false;

            _elapsed = _stats.ChangeLiquidCd;
            LiquidBase tmp = CurrentLiquid;
            CurrentLiquid = StandbyLiquid;
            StandbyLiquid = tmp;

            tmp?.ForceEndSkill();
            OnChangeLiquid?.Invoke();
            return true;
        }
        /// <summary>
        /// 吸取液体，备用槽必须为空
        /// </summary>
        /// <param name="liquid"></param>
        public bool GetLiquid(LiquidConfig cfg)
        {
            if (StandbyLiquid != null) return false;

            StandbyLiquid = LiquidFactory.Create(cfg, _stats);
            if (StandbyLiquid == null) { Debug.LogError($"[PlayerLiquids] 没有对应液体类: {cfg?.Id}"); return false; }

            OnGainLiquid?.Invoke();
            return true;
        }
        /// <summary>
        /// 倾倒液体，备用槽必须为非空
        /// </summary>
        public bool PourOutLiquid()
        {
            if (StandbyLiquid == null)
                return false;

            StandbyLiquid = null;
            OnPourOutLiquid?.Invoke();
            return true;
        }
        /// <summary>
        /// 切换液体CD Tick
        /// </summary>
        /// <param name="delta"></param>
        public void TickChangeLiquid(float delta)
        {
            _elapsed = Mathf.Max(0f, _elapsed - delta);
        }
    }
}
