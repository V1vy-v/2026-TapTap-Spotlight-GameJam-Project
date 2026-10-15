using GamePlay.Configs;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Liquid
{
    public abstract class LiquidBase
    {
        public LiquidConfig Config;
        public PlayerStats Stats;

        #region 技能时序
        private float _cdRemain;
        private float _elapsed;
        public bool IsSkillActive { get; private set; }

        public bool TryUseSkill()
        {
            if (_cdRemain > 0f || IsSkillActive || !CanUseSkill()) return false;
            IsSkillActive = true;
            _elapsed = 0f;
            OnSkillBegin();
            return true;
        }
        public void TickSkill(float dt)
        {
            if (IsSkillActive)
            {
                _elapsed += dt;
                OnSkillTick(dt);
                if (OnSkillShouldEnd(_elapsed))          // ← 传参，子类才拿得到已过时间
                {
                    IsSkillActive = false;
                    OnSkillEnd();
                    _cdRemain = Config.SkillCd;
                }
            }
            else if (_cdRemain > 0f)
                _cdRemain = Mathf.Max(0f, _cdRemain - dt);
        }
        public void ForceEndSkill()
        {
            if (!IsSkillActive) return;
            IsSkillActive = false;
            OnSkillEnd();
            _cdRemain = Config.SkillCd;
        }

        protected virtual bool CanUseSkill() => true;
        protected abstract void OnSkillBegin();
        protected virtual void OnSkillTick(float dt) { }
        protected virtual void OnSkillEnd() { }
        protected virtual bool OnSkillShouldEnd(float elapsed) => elapsed >= Config.SkillDuration;
        #endregion

        public virtual void Init(LiquidConfig cfg, PlayerStats stats) { Config = cfg; Stats = stats; }

        public virtual void OnHitEnemy() { }
        public virtual bool Attack() => false;
    }
}