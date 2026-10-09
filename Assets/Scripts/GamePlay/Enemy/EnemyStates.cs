using Framework;
using UnityEngine;

namespace GamePlay.Enemy
{
    /// <summary>
    /// 敌人待机状态：原地不动，等待条件触发
    /// </summary>
    public class EnemyIdleState : EnemyStateBase
    {
        public override EnemyState StateKey => EnemyState.Idle;

        public EnemyIdleState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
            // 播放待机动画
        }

        protected override void Tick(float deltaTime)
        {
            // 待机时什么都不做
        }

        protected override void CheckStateChange()
        {
            // 玩家进入视野，切换到追击
            _stateMachine.ChangeState(EnemyState.Chase);

        }
    }

    /// <summary>
    /// 敌人巡逻状态：沿路径点移动
    /// </summary>
    public class EnemyPatrolState : EnemyStateBase
    {
        public override EnemyState StateKey => EnemyState.Patrol;

        public EnemyPatrolState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
        }

        protected override void Tick(float deltaTime)
        {
            // 沿巡逻路径移动
        }

        protected override void CheckStateChange()
        {
            // 玩家进入视野，切换到追击
            _stateMachine.ChangeState(EnemyState.Chase);
            //或者巡逻到终点，切换到idle
            _stateMachine.ChangeState(EnemyState.Idle);

        }
    }

    /// <summary>
    /// 敌人追击状态：朝玩家移动
    /// </summary>
    public class EnemyChaseState : EnemyStateBase
    {
        public override EnemyState StateKey => EnemyState.Chase;

        public EnemyChaseState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
        }

        protected override void Tick(float deltaTime)
        {
            // 朝玩家移动
        }

        protected override void CheckStateChange()
        {
            // 玩家跑出视野，回到待机
            _stateMachine.ChangeState(EnemyState.Idle);
            // 进入攻击距离，切换到攻击
            _stateMachine.ChangeState(EnemyState.Attack);
        }

    }

    /// <summary>
    /// 敌人攻击状态：对玩家造成伤害
    /// </summary>
    public class EnemyAttackState : EnemyStateBase
    {
        private float _attackTimer;

        public override EnemyState StateKey => EnemyState.Attack;

        public EnemyAttackState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
        }

        protected override void Tick(float deltaTime)
        {
        }

        protected override void CheckStateChange()
        {
            
        }
    }

    /// <summary>
    /// 敌人受击状态：被击中时短暂僵直
    /// </summary>
    public class EnemyHurtState : EnemyStateBase
    {
        private float _hurtTimer;

        public override EnemyState StateKey => EnemyState.Hurt;

        public EnemyHurtState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
        }

        protected override void Tick(float deltaTime)
        {
            _hurtTimer += deltaTime;
        }

        protected override void CheckStateChange()
        {
        }
    }

    /// <summary>
    /// 敌人死亡状态：播放死亡动画后销毁
    /// </summary>
    public class EnemyDeadState : EnemyStateBase
    {
        public override EnemyState StateKey => EnemyState.Dead;

        public EnemyDeadState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        protected override void Tick(float deltaTime)
        {
        }

        protected override void CheckStateChange()
        {
        }
    }
}