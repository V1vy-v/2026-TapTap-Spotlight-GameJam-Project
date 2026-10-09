using Framework;
using UnityEngine;

namespace GamePlay.Enemy
{
    /// <summary>
    /// 敌人待机状态：原地不动，等待条件触发
    /// </summary>
    public class EnemyIdleState : EnemyStateBase
    {
        private float _idleTimer;

        public override EnemyState StateKey => EnemyState.Idle;

        public EnemyIdleState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
            Enemy.ChangeSpeed();
            Enemy.PlayAnimation("Idle");
            _idleTimer = 0f; // 进入时清零
        }

        protected override void Tick(float deltaTime)
        {
            _idleTimer += deltaTime;
        }

        protected override void CheckStateChange()
        {
            if (Enemy.playerInAttackRange)
                _stateMachine.ChangeState(EnemyState.Attack);
            else if (Enemy.playerInDetectRange)
                _stateMachine.ChangeState(EnemyState.Chase);
            else if (_idleTimer >= Enemy.idleDuration)
                _stateMachine.ChangeState(EnemyState.Patrol); 
        }
    }

    /// <summary>
    /// 敌人巡逻状态：沿路径点移动
    /// </summary>
    public class EnemyPatrolState : EnemyStateBase
    {
        private float _patrolTimer;

        public override EnemyState StateKey => EnemyState.Patrol;

        public EnemyPatrolState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
            Enemy.ChangeSpeed(Enemy.moveSpeed);
            Enemy.PlayAnimation("Walk");
            _patrolTimer = 0f; // 进入时清零
        }

        protected override void Tick(float deltaTime)
        {
            _patrolTimer += deltaTime;
        }

        protected override void CheckStateChange()
        {
            if (Enemy.playerInAttackRange)
                _stateMachine.ChangeState(EnemyState.Attack);
            else if (Enemy.playerInDetectRange)
                _stateMachine.ChangeState(EnemyState.Chase);
            else if (_patrolTimer >= Enemy.patrolDuration)
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
            Enemy.ChangeSpeed(Enemy.chaseSpeed);
        }

        protected override void Tick(float deltaTime)
        {
            // 朝玩家移动
            //Enemy.MoveToTarget();
        }

        protected override void CheckStateChange()
        {
            if (Enemy.playerInAttackRange)
                _stateMachine.ChangeState(EnemyState.Attack);
            else if (!Enemy.playerInDetectRange)
                _stateMachine.ChangeState(EnemyState.Patrol);
        }

    }

    /// <summary>
    /// 敌人攻击状态：对玩家造成伤害
    /// </summary>
    public class EnemyAttackState : EnemyStateBase
    {
        private float _attackTimer;//上一次攻击时间

        public override EnemyState StateKey => EnemyState.Attack;

        public EnemyAttackState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
            Enemy.ChangeSpeed();
            _attackTimer = 0f; 
            Enemy.PlayAnimation("Attack");
        }

        protected override void Tick(float deltaTime)
        {
            _attackTimer += deltaTime;

            if (_attackTimer >= Enemy.attackCooldown)
            {
                _attackTimer = 0f;
                Enemy.PlayAnimation("Attack");
            }
        }

        protected override void CheckStateChange()
        {
            //如果玩家离开攻击范围,根据索敌范围回到巡逻状态或者是追击状态
            //_stateMachine.ChangeState(EnemyState.Patrol);
            //_stateMachine.ChangeState(EnemyState.Chase);
            //如果玩家没死且仍在攻击范围内
            //无需操作
            if (!Enemy.playerInAttackRange && Enemy.playerInDetectRange)
            {
                _stateMachine.ChangeState(EnemyState.Chase);
            }else if (!Enemy.playerInDetectRange)
            {
                _stateMachine.ChangeState(EnemyState.Patrol);
            }

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
            Enemy.ChangeSpeed();     
            _hurtTimer = 0f;         
            Enemy.PlayAnimation("Hurt");
        }

        protected override void Tick(float deltaTime)
        {
            _hurtTimer += deltaTime; 
        }

        protected override void CheckStateChange()
        {
            if (_hurtTimer < Enemy.hurtDuration) return; 

            if (Enemy.Health <= 0)
                _stateMachine.ChangeState(EnemyState.Dead);
            else if (Enemy.playerInAttackRange)
                _stateMachine.ChangeState(EnemyState.Attack);
            else if (Enemy.playerInDetectRange)
                _stateMachine.ChangeState(EnemyState.Chase);
            else
                _stateMachine.ChangeState(EnemyState.Patrol);
        }
    }

    /// <summary>
    /// 敌人死亡状态：播放死亡动画后销毁
    /// </summary>
    public class EnemyDeadState : EnemyStateBase
    {
        private float _deadTimer;

        public override EnemyState StateKey => EnemyState.Dead;

        public EnemyDeadState(EnemyController enemy, StateMachine<EnemyState> sm)
            : base(enemy, sm) { }

        public override void Enter()
        {
            Enemy.ChangeSpeed();
            Enemy.PlayAnimation("Dead");
            _deadTimer = 0f;
            //散布死亡事件通知外部死亡
        }

        protected override void Tick(float deltaTime)
        {
            _deadTimer += deltaTime;
            float t = _deadTimer / Enemy.fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, t);
            Enemy.SetAlpha(alpha);
        }

        protected override void CheckStateChange()
        {
            if (_deadTimer >= Enemy.fadeDuration)
            {
                //由对象池销毁
            }
        }
    }
}