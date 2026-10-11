using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "LiquidConfig", menuName = "Configs/LiquidConfig")]
    public class LiquidConfig : ConfigBase
    {
        [Header("数值")]
        public int damage;           // 伤害
        public float attackInterval; // 攻击间隔
    }
}
