using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerStats
    {
        // 移动
        public float MoveSpeed;
        public float SprintSpeed;
        public float SprintDuration;
        public float SprintCd;

        // 生命/战斗
        public float Hp;
        public float MaxHp;
        public float BaseAttack;
        public float AttackSpeed;
        public float AttackRange;
        public bool IsDead;

        // 液体相关
        public float ChangeLiquidCd;
        public float ChangeLiquidCdInBattle;

        public void Init(PlayerConfig config)
        {
            MoveSpeed = config.MoveSpeed;
            SprintSpeed = config.SprintSpeed;
            SprintDuration = config.SprintDuration;
            SprintCd = config.SprintCd;

            MaxHp = config.MaxHp;
            Hp = MaxHp;
            BaseAttack = config.BaseAttack;
            AttackSpeed = config.AttackSpeed;
            AttackRange = config.AttackRange;

            ChangeLiquidCd = config.ChangeLiquidCd;
            ChangeLiquidCdInBattle = config.ChangeLiquidCdInBattle;

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
