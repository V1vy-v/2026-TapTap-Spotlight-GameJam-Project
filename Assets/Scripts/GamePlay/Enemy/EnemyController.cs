using Framework;
using UnityEngine;

namespace GamePlay.Enemy
{
    /// <summary>
    /// 怪物总控制器
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        //敌人数据
        public float moveSpeed = 2f;        // 移动速度
        public float chaseSpeed = 4f;       // 追击速度
        public float attackRange = 1.5f;    // 攻击范围
        public float attackCooldown = 1f;   // 攻击冷却时间
        public float hurtDuration = 0.3f;   // 受击僵直时间
        public int Health = 100;            // 生命值
        public float fadeDuration = 1f;     // 渐隐时长

        //AI
        public float idleDuration = 2f;     // 待机持续时间
        public float patrolDuration = 5f;   // 巡逻持续时间

        //检测状态
        [HideInInspector] public bool playerInDetectRange;
        [HideInInspector] public bool playerInAttackRange;

        public Transform player;            // 持有的玩家对象

        private SpriteRenderer _sprite;
        private Animator animator;
        private EnemyMoveController moveController;

        private StateMachine<EnemyState> _fsm;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
            moveController = GetComponent<EnemyMoveController>();

            if (moveController == null) return;
            if (animator == null) return;
        }

        private void Start()
        {
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
            _fsm?.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _fsm?.FixedUpdate(Time.fixedDeltaTime);
        }

        //给状态类调用的工具方法

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            if (animator == null) return;
            animator.CrossFade(Animator.StringToHash(animationName), 0.1f);
        }

        /// <summary>移动至目标点</summary>
        public void MoveToTarget(Vector2 targetPos)
        {
            moveController?.MoveTo(targetPos);
        }

        public void ChangeSpeed(float speed = 0)
        {
            moveController?.SetSpeed(speed);
        }

        /// <summary>
        /// 改变透明度，用于死亡渐隐
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha)
        {
            if (_sprite == null) return;
            var c = _sprite.color;
            c.a = alpha;
            _sprite.color = c;
        }

        public void ApplyDamage(int damage)
        {
            if (_fsm == null) return;
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