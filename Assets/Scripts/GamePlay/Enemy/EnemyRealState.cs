using UnityEngine;

/// <summary>
/// 怪物运行时状态
/// </summary>
public class EnemyRealState
{
    //基础值
    public float baseHP;    //基础最大血量
    public float baseAtk;   //基础攻击力
    public float baseMoveSpeed; //基础移动速度
    public float baseAttackRange;   //基础攻击范围
    public float baseAttackCooldown;    //基础攻击冷却时间
    public float baseHurtDuration;  //基础受击僵直时间

    //加成值
    public float bonusHP;
    public float bonusAtk;
    public float bonusMoveSpeed;
    public float bonusChaseSpeed;
    public float bonusAttackRange;
    public float bonusAttackCooldown;
    public float bonusHurtDuration;

    //最终值
    public float MaxHP => baseHP + bonusHP;
    public float Atk => baseAtk + bonusAtk;
    public float MoveSpeed => baseMoveSpeed + bonusMoveSpeed;
    public float AttackRange => baseAttackRange + bonusAttackRange;
    public float AttackCooldown => baseAttackCooldown + bonusAttackCooldown;
    public float HurtDuration => baseHurtDuration + bonusHurtDuration;

    //当前血量
    public float CurrentHP;

    public void ClampHP()
    {
        CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);
    }
}