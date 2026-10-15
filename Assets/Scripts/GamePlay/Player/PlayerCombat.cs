using Framework;
using GamePlay.Enemy;
using UnityEngine;

namespace GamePlay.Player
{
    public enum AttackModule
    {
        Melee = 0,
        Ranged = 1,
    }
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerStats _stats;
        private PlayerLiquids _liquids;
        [SerializeField] private Vector2 _primaryMeleeAtkOffset;
        [SerializeField] private Vector2 _primaryRangedAtkOffset;
        [SerializeField] private LayerMask enemyLayerMask;

        private bool _isStrawGained = false;
        private AttackModule _currentAtkModule = AttackModule.Melee;
        private float _elapsed;//普攻CD相关
        public void Init(PlayerStats stats, PlayerLiquids liquids)
        {
            _stats = stats;
            _liquids = liquids;
        }
        private void Update()
        {

        }

        /// <summary>
        /// 搜索最近敌人/精英Boss
        /// </summary>
        private Vector2 SearchForEnemy()
        {

            return Vector2.zero;
        }
        /// <summary>
        /// 普通攻击
        /// </summary>
        /// <param name="dmg"></param>
        public void PrimaryAttack()
        {
            // CD判断

            if(_currentAtkModule == AttackModule.Melee)
            {
                //播放动画和音效
                
                RangeAttack(_primaryMeleeAtkOffset, _stats.MeleeRange);
            }
            else
            {
                //播放动画和音效

                Vector2 dir = (SearchForEnemy() - (Vector2)transform.position).normalized;
                //获取射出子弹对象
                DirectionalAttack(dir, _primaryRangedAtkOffset, _liquids.CurrentLiquid.liquidConfig.BulletPrefabRes, _stats.RangedDamage);

            }

        }

        /// <summary>
        /// 指向性攻击：索敌+指向
        /// </summary>
        /// <param name="dmg"></param>
        private void DirectionalAttack(Vector2 dir,Vector2 offset, string projectileRes, float dmg)
        {
            //创建攻击物并初始化方向
            Global.Instantiate(projectileRes);
            //调用子弹方法

        }
        /// <summary>
        /// 使用技能
        /// </summary>
        public void UseSkill()
        {
            _liquids.CurrentLiquid?.TryUseSkill();
        }
        /// <summary>
        /// 切换远程/近战攻击
        /// </summary>
        public void SwitchModule()
        {
            if (!_isStrawGained)
            {
                // 操作失败表现

                return;
            }

            if (_currentAtkModule == AttackModule.Melee)
                _currentAtkModule = AttackModule.Ranged;
            else
                _currentAtkModule = AttackModule.Melee;
        }
        /// <summary>
        /// 解锁远程攻击模块
        /// </summary>
        public void UnlockRangeModule()
        {
            _isStrawGained |= true;
        }

        /// <summary>
        /// 非指向性攻击：范围攻击
        /// </summary>
        /// <param name="dmg"></param>
        private void RangeAttack(Vector2 offset, float radius)
        {
            Vector2 center = (Vector2)transform.position + offset;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, enemyLayerMask);
            foreach (var col in hits)
            {
                //敌人受伤
                col.GetComponent<EnemyController>().ApplyDamage(_stats.MeleeDamage);
            }
        }
    }
}
