using UnityEngine;

namespace GamePlay.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ConfigBase
    {
        [Header("血量")]
        public float MaxHp;

        [Header("战斗")]
        public int Atk;
        public int Def;

        [Header("移动")]
        public float MoveSpeed;
    }
}
