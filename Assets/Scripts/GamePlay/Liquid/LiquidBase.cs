using GamePlay.Configs;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Liquid
{
    /// <summary>
    /// 液体(武器)基类
    /// </summary>
    public abstract class LiquidBase : MonoBehaviour
    {
        public LiquidConfig liquidConfig;
        public AnimatorOverrideController AnimatorOverride;

        #region 技能：基类管时序，子类管效果
        private float _cdRemain;
        private float _elapsed;
        public bool IsSkillActive { get; private set; }

        /// <summary>非虚：所有主动形态共用的冷却与持续规则</summary>
        public bool TryUseSkill()
        {
            if (_cdRemain > 0f || IsSkillActive) return false;

            IsSkillActive = true;
            _elapsed = 0f;
            OnSkillBegin();                      // ← 子类实现
            return true;
        }

        /// <summary>由 PlayerCombat 每帧驱动</summary>
        public void TickSkill(float dt)
        {
            if (IsSkillActive)
            {
                _elapsed += dt;
                OnSkillTick(dt);
                if (OnSkillShouldEnd())
                {
                    IsSkillActive = false;
                    OnSkillEnd();
                    _cdRemain = liquidConfig.SkillCd;   // ← 结束后才开始计时
                }
            }
            else if (_cdRemain > 0f) _cdRemain -= dt;
        }

        protected abstract void OnSkillBegin();
        protected virtual void OnSkillTick(float dt) { }
        protected virtual void OnSkillEnd() { }
        protected virtual bool OnSkillShouldEnd() => _elapsed >= liquidConfig.SkillDuration;
        #endregion

        /// <summary>被动：装上即生效，与按键无关</summary>
        public virtual void ApplyPassive(PlayerStats stats) { }
        public virtual void RemovePassive(PlayerStats stats) { }

        /// <summary>普攻模组：三拳会把它换成三板斧</summary>
        public abstract void Attack();
    }
}
