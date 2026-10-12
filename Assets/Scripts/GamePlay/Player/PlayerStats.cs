using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerStats
    {
        [Header("血量")]
        public float MaxHp;
        public float Hp;

        [Header("战斗")]
        public int Atk;
        public int Def;

        [Header("移动")]
        public float moveSpeed;
    }
}
