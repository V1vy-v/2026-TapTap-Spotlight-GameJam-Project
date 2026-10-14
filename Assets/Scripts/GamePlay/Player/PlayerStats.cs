using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>
    /// 主角运行时数值。
    /// 对局开始时从 <see cref="PlayerConfig"/> 拷一份基础值，之后由状态与道具在此之上增删
    /// —— 配置是静态基础值，这里是当前值。
    /// </summary>
    public class PlayerStats
    {
        // 生命
        public float Hp;
        public float MaxHp;
        public float InvincibleTime;//无敌帧时间
        public float HurtStunTime;//僵直时间
        public bool IsDead;

        // 移动
        public float MoveSpeed;
        public float SprintSpeed;
        public float SprintCd;
        public float SprintMaxHold;
        public float SprintDuration;

        // 喷射累积伤害
        public float SprintDamagePerTick;
        public float SprintTickInterval;
        public float SprintCartExtraDamage;

        // 近战 J
        public float MeleeDamage;
        public float MeleeCd;
        public float MeleeRecovery;
        public float MeleeRange;

        // 远程 Q
        public float RangedDamage;
        public float RangedInterval;
        public float RangedRange;

        // 倍率基准
        public float AttackSpeed;

        // 液体切换 Tab
        public float ChangeLiquidCd;

        public void Init(PlayerConfig config)
        {
            MaxHp = config.MaxHp;
            Hp = MaxHp;
            InvincibleTime = config.InvincibleTime;
            HurtStunTime = config.HurtStunTime;

            MoveSpeed = config.MoveSpeed;
            SprintSpeed = config.SprintSpeed;
            SprintCd = config.SprintCd;
            SprintMaxHold = config.SprintMaxHold;
            SprintDuration = config.SprintDuration;

            SprintDamagePerTick = config.SprintDamagePerTick;
            SprintTickInterval = config.SprintTickInterval;
            SprintCartExtraDamage = config.SprintCartExtraDamage;

            MeleeDamage = config.MeleeDamage;
            MeleeCd = config.MeleeCd;
            MeleeRecovery = config.MeleeRecovery;
            MeleeRange = config.MeleeRange;

            RangedDamage = config.RangedDamage;
            RangedInterval = config.RangedInterval;
            RangedRange = config.RangedRange;

            AttackSpeed = config.AttackSpeed;

            ChangeLiquidCd = config.ChangeLiquidCd;

            IsDead = false;
        }

        /// <summary>
        /// 玩家回复血量
        /// </summary>
        /// <param name="heal"></param>
        public void Healing(int heal)
        {
            if (IsDead)
                return;

            Hp = Mathf.Clamp(Hp + heal, 0, MaxHp);

            //动画与音效
        }
        /// <summary>
        /// 玩家受到伤害
        /// </summary>
        /// <param name="dmg"></param>
        public void ApplyDamage(int dmg)
        {
            if (IsDead)
                return;

            Hp = Mathf.Clamp(Hp - dmg, 0, MaxHp);

            //动画与音效
            if (Hp <= 0)
            {
                IsDead = true;
                //死亡动画与音效

            }

        }
    }
}
