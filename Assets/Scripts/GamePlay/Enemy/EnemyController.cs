using Framework;
using UnityEngine;
namespace GamePlay.Enemy
{
    /// <summary>
    /// 怪物控制器，用于控制怪物行为
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Header("敌人数据")]
        public float moveSpeed = 2f;//移动速度
        public float chaseSpeed = 4f;//追击速度
        public float attackRange = 1.5f;//攻击范围
        public float attackCooldown = 1f;//攻击冷却时间
        public float hurtDuration = 0.3f;//受击僵直时间
        public int Health = 100;//生命值

        private float _currentSpeed;//当前速度
        private Vector2 _moveDir;//移动方向

        public Transform player;//持有的玩家对象
        private Animator animator;//动画对象
        private Rigidbody2D rb;

        private StateMachine<EnemyState> _fsm;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            _fsm = new StateMachine<EnemyState>();
            _fsm.RegisterState(new EnemyIdleState(this, _fsm));
            _fsm.RegisterState(new EnemyPatrolState(this, _fsm));
            _fsm.RegisterState(new EnemyChaseState(this, _fsm));
            _fsm.RegisterState(new EnemyAttackState(this, _fsm));
            _fsm.RegisterState(new EnemyHurtState(this, _fsm));
            _fsm.RegisterState(new EnemyDeadState(this, _fsm));

            _fsm.ChangeState(EnemyState.Idle);
        }
        private void Update()
        {
            _fsm.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate(Time.fixedDeltaTime);

            if (rb != null)
            {
                rb.velocity = _moveDir * _currentSpeed;
            }
        }

        //给状态类调用的工具方法
        /// <summary>
        /// 设置动画
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            if (animator == null) return;
            animator.CrossFade(Animator.StringToHash(animationName), 0.1f);
        }
        public void MoveToTarget(Vector2 targetPos, float speed)
        {
            _moveDir = (targetPos - (Vector2)transform.position).normalized;
            _currentSpeed = speed;
        }

        //受击
        public void ApplyDamage(int damage)
        {
            if (_fsm.CurrentState == EnemyState.Dead) return;

            Health -= damage;
            if (Health <= 0)
            {
                _fsm.ChangeState(EnemyState.Dead);
            }
            else
            {
                _fsm.ChangeState(EnemyState.Hurt);
            }
        }
    }
}