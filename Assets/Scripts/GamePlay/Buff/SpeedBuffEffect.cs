using UnityEngine;

/// <summary>
/// 移速加成效果(示例)
/// </summary>
[CreateAssetMenu(fileName = "SpeedBuffEffect", menuName = "Configs/BuffEffect/SpeedBuffEffect")]
public class SpeedBuffEffect : BuffEffect
{
    public float value = 2f;

    public override void OnApply(EnemyRealState state)
    {
        state.bonusMoveSpeed += value;
    }

    public override void OnRemove(EnemyRealState state)
    {
        state.bonusMoveSpeed -= value;
    }
}