using UnityEngine;

namespace GamePlay.Liquid
{
    /// <summary>
    /// 国窖：红温。
    /// 命中积攒红温（每次 +RagePerHit），RageDecayDelay 秒内无命中则每秒衰减 RageDecayPerSec；
    /// 满 RageMax 才能按 K。释放即清空红温，进入「冷静」期（攻速与命中减速由 LiquidConfig 提供）。
    /// </summary>
    public class GuoJiao : LiquidBase
    {
        private float _rage;
        private float _idleTime;        // 距上次命中的秒数

        /// <summary>当前红温值</summary>
        public float Rage => _rage;

        /// <summary>红温比例 0~1，供 UI 读</summary>
        public float RageRatio => Config != null && Config.RageMax > 0f ? _rage / Config.RageMax : 0f;

        /// <summary>红温未满不允许释放</summary>
        protected override bool CanUseSkill() => Config != null && _rage >= Config.RageMax;

        public override void OnHitEnemy()
        {
            if (Config == null) return;

            _idleTime = 0f;
            _rage = Mathf.Min(_rage + Config.RagePerHit, Config.RageMax);
        }

        protected override void OnPassiveTick(float dt)
        {
            if (Config == null) return;

            _idleTime += dt;
            if (_idleTime <= Config.RageDecayDelay) return;         // 宽限期内不衰减

            _rage = Mathf.Max(0f, _rage - Config.RageDecayPerSec * dt);
        }

        protected override void OnSkillBegin()
        {
            _rage = 0f;                 // 释放红温攻击，红温归零
            _idleTime = 0f;
        }
    }
}