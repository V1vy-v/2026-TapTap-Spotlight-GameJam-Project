namespace GamePlay.Enemy
{
    /// <summary>
    /// 怪物行为状态
    /// </summary>
    public enum EnemyState
    {
        Idle = 0,      // 待机
        Patrol = 1,    // 巡逻
        Chase = 2,     // 追击
        Attack = 3,    // 攻击
        Hurt = 4,      // 受击
        Dead = 5,      // 死亡
    }
}