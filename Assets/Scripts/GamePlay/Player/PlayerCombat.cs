using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerStats _stats;
        private CapsuleCollider2D _hitbox;
        [SerializeField] private LayerMask enemyLayerMask;
        private void Awake()
        {
            _hitbox = transform.Find("Hitbox").GetComponent<CapsuleCollider2D>();
        }
        public void Init(PlayerStats stats)
        {
            _stats = stats;
        }
        /// <summary>
        /// 搜索最近敌人/精英Boss
        /// </summary>
        private Vector2 SearchForEnemy()
        {

            return Vector2.zero;
        }

        /// <summary>
        /// 非指向性攻击：普通/范围攻击
        /// </summary>
        /// <param name="dmg"></param>
        public void RangeAttack(Vector2 offset, float radius)
        {
            //攻击动画与音效


            Vector2 center = (Vector2)transform.position + offset;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, enemyLayerMask);
            foreach(var colk in hits)
            {
                //敌人受伤

            }
        }
        /// <summary>
        /// 指向性攻击：索敌+指向
        /// </summary>
        /// <param name="dmg"></param>
        public void DirectionalAttack(float dmg)
        {
            //攻击动画与音效

            Vector2 dir = (SearchForEnemy() - (Vector2)transform.position).normalized;
            //创建攻击物并初始化方向

        }

        /// <summary>
        /// 受伤
        /// </summary>
        /// <param name="dmg"></param>
        public void PlayerWound(float dmg)
        {

        }
    }
}