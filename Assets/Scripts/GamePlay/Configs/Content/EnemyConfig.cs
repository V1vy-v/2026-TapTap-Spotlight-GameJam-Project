using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
    public class EnemyConfig : ConfigBase
    {
        [Header("基础数值")]
        public float maxHP = 100;
        public float atk = 10;
        public float moveSpeed = 2f;
        public float attackRange = 1f;
        public float attackCooldown = 1.5f;
        public float hurtDuration = 0.3f;

        [Header("AI 参数")]
        public float idleDuration = 2f;
        public float patrolDuration = 5f;
        public float fadeDuration = 1f;
    }
}
