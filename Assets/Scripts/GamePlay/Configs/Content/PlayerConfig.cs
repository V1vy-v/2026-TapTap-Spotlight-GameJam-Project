using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ConfigBase
    {
        // Id / Name 继承自 ConfigBase

        [Header("移动")]
        public float MoveSpeed = 15f;
        public float SprintSpeed = 40f;
        public float SprintDuration = 0.2f;
        public float SprintCd = 1f;

        [Header("生命/战斗")]
        public float MaxHp = 100f;
        public float BaseAttack = 10f;
        public float AttackSpeed = 1f;
        public float AttackRange = 1.5f;

        [Header("液体吸收")]
        public float ChangeLiquidCd = 1f;
        public float ChangeLiquidCdInBattle = 10f;
    }
}
