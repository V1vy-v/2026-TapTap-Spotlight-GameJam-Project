using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ConfigBase
    {
        // Id / Name 继承自 ConfigBase

        [Header("生命")]
        public float MaxHp = 100f;
        public float InvincibleTime = 0.5f;     // 受击无敌帧
        public float HurtStunTime = 0.2f;       // 受击硬直

        [Header("移动")]
        public float MoveSpeed = 15f;
        public float SprintSpeed = 40f;         // 空格喷射
        public float SprintCd = 1f;             // 喷射冷却
        public float SprintMaxHold = 6f;        // 喷射长按上限
        // TODO: 待定 —— 07 数值里喷射只有「冷却 1 秒 / 长按上限 6 秒 / 每 0.5 秒累积」，没有对应项
        public float SprintDuration = 0.2f;

        [Header("喷射累积伤害")]
        public float SprintDamagePerTick = 4f;      // 每 SprintTickInterval 累积
        public float SprintTickInterval = 0.5f;
        public float SprintCartExtraDamage = 2f;    // 购物车每秒额外累积

        [Header("近战 J")]
        public float MeleeDamage = 10f;         // 单段伤害
        public float MeleeCd = 0.8f;            // 两次普攻之间的冷却
        public float MeleeRecovery = 0.2f;      // 后摇
        public float MeleeRange = 1.5f;         // 判定半径

        [Header("远程 Q")]
        public float RangedDamage = 14f;
        public float RangedInterval = 0.8f;
        public float RangedRange = 4.5f;        // 约近战 3 倍

        [Header("倍率基准")]
        public float AttackSpeed = 1f;          // 攻速倍率，道具与状态在此之上乘算

        [Header("液体切换 Tab")]
        public float ChangeLiquidCd = 10f;      // 冷却
    }
}
