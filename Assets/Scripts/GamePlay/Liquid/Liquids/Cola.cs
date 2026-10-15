using GamePlay.Liquid;
using UnityEngine;

public class Cola : LiquidBase
{
    private float _extended;        // 曼妥思累计延长

    protected override void OnSkillBegin()
    {
        _extended = 0f;             // 每次释放重置
    }

    /// <summary>曼妥思质变：命中续时</summary>
    public override void OnHitEnemy()
    {
        if (!IsSkillActive) return;
        if (!Stats.HasMantos) return;                                    // 质变未获得
        _extended = Mathf.Min(_extended + Config.ExtendPerHit, Config.ExtendMax);
    }

    protected override bool OnSkillShouldEnd(float elapsed)
        => elapsed >= Config.SkillDuration + _extended;
}