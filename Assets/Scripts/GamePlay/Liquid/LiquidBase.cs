using GamePlay.Configs;
using GamePlay.Player;

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
        if (_cdRemain > 0f || IsSkillActive || !CanUseSkill()) return false;   // ← 多一个条件
        IsSkillActive = true;
        _elapsed = 0f;
        OnSkillBegin();
        return true;
    }
    public void TickSkill(float dt) { /* 原样 */ }
    public void ForceEndSkill() { /* 原样 */ }

    /// <summary>额外释放条件：国窖 override 成「红温满」（它没有时间 CD）</summary>
    protected virtual bool CanUseSkill() => true;

    protected abstract void OnSkillBegin();
    protected virtual void OnSkillTick(float dt) { }
    protected virtual void OnSkillEnd() { }
    protected virtual bool OnSkillShouldEnd() => _elapsed >= Config.SkillDuration;
    #endregion

    public virtual void Init(LiquidConfig cfg, PlayerStats stats) { Config = cfg; Stats = stats; }

    /// <summary>命中敌人时回调：红温累积 / 曼妥思续时</summary>
    public virtual void OnHitEnemy() { }

    /// <summary>普攻模组：返回 true = 形态接管了本次普攻（三拳）</summary>
    public virtual bool Attack() => false;
}