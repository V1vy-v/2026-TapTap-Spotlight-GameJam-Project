using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "LiquidConfig", menuName = "Configs/LiquidConfig")]
    public class LiquidConfig : ConfigBase
    {
        [Header("战斗")]
        public int damage;              // 伤害
        public float attackInterval;    // 攻击间隔
        public bool IsActive;           //是否为主动型
    }
}
