using Framework;
using GamePlay.Enemy;
using GamePlay.Liquid;
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
        private PlayerView _view;
        [SerializeField] private Vector2 _primaryMeleeAtkOffset;
        [SerializeField] private Vector2 _primaryRangedAtkOffset;
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private Transform _bulletRoot;

        private bool _isStrawGained = false;
        private AttackModule _currentAtkModule = AttackModule.Melee;
        private float _elapsed;
        public void Init(PlayerStats stats, PlayerView view, PlayerLiquids liquids)
        {
            _stats = stats;
            _view = view;
            _liquids = liquids;
        }
        private void Update()
        {
            _liquids?.CurrentLiquid?.TickSkill(Time.deltaTime); 
            _liquids?.StandbyLiquid?.TickSkill(Time.deltaTime);
            TickPrimaryAtk(Time.deltaTime);
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
            if (_elapsed > 0) return;
            _elapsed = _stats.PrimaryAtkInterval / (1f + _liquids.AttackSpeedBonus);

            if (_liquids?.CurrentLiquid?.Attack() == true) return;   // 形态接管普攻

            if (_currentAtkModule == AttackModule.Melee)
            {
                _view.PlayAttack(AttackModule.Melee);
                RangeAttack(_primaryMeleeAtkOffset, _stats.MeleeRange * (1f + _liquids.RangeBonus));
            }
            else
            {
                _view.PlayAttack(AttackModule.Ranged);
                Vector2 dir = (SearchForEnemy() - (Vector2)transform.position).normalized;
                DirectionalAttack(dir, (Vector2)transform.position + _primaryRangedAtkOffset, _liquids?.CurrentLiquid?.Config?.BulletPrefabRes, _stats.RangedDamage);
            }
        }
        /// <summary>
        /// 使用技能
        /// </summary>
        public void UseSkill()
        {
            var cur = _liquids?.CurrentLiquid;
            if (cur == null) return;

            if (cur.TryUseSkill()) return;

            // 临时诊断，接上 UI/音效后删掉
            string reason = cur.IsSkillActive ? "持续中"
                          : cur.CdRemain > 0f ? $"冷却 {cur.CdRemain:F1}s"
                          : cur is GuoJiao gj ? $"红温 {gj.RageRatio:P0}"
                          : "不可用";
            Debug.Log($"[技能] 释放失败：{cur.Config?.Name} —— {reason}");
        }
        /// <summary>
        /// 切换远程/近战攻击
        /// </summary>
        public void SwitchModule()
        {
            if (!_isStrawGained)
            {
                // 操作失败
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
        /// 指向性攻击：索敌+指向
        /// </summary>
        /// <param name="dmg"></param>
        private void DirectionalAttack(Vector2 dir, Vector2 pos, string projectileRes, float dmg)
        {
            if (string.IsNullOrEmpty(projectileRes)) return;

            //创建攻击物并初始化方向
            Global.Instantiate(projectileRes, new YooAsset.InstantiateOptions(true, _bulletRoot, false));
            //调用子弹方法

            //子弹初始化rotation：
            //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        /// <summary>
        /// 非指向性攻击：范围攻击
        /// </summary>
        /// <param name="dmg"></param>
        private void RangeAttack(Vector2 offset, float radius)
        {
            float dmg = _stats.MeleeDamage * (1f + _liquids.AtkBonus);
            Vector2 center = (Vector2)transform.position + offset;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, _enemyLayerMask);

            bool hit = false;
            foreach (var col in hits)
            {
                var enemy = col.GetComponentInParent<EnemyController>();
                if (enemy == null) continue;
                enemy.ApplyDamage(dmg);
                hit = true;
            }
            if (hit) 
                _liquids?.CurrentLiquid?.OnHitEnemy();
        }
        /// <summary>
        /// 普攻Tick
        /// </summary>
        /// <param name="delta"></param>
        private void TickPrimaryAtk(float delta)
        {
            _elapsed = Mathf.Max(0f, _elapsed - delta);
        }

    }
}
