using UnityEngine;

/// <summary>
/// Buff 效果基类，不同效果buff继承实现
/// </summary>
public abstract class BuffEffect : ScriptableObject
{
    /// <summary>
    /// 加 Buff 时调用
    /// </summary>
    /// <param name="state"></param>
    public abstract void OnApply(EnemyRealState state);

    /// <summary>
    /// 移除 Buff 时调用
    /// </summary>
    /// <param name="state"></param>
    public abstract void OnRemove(EnemyRealState state);
}