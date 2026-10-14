using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerStats
    {
        public float MaxHp;
        public float Hp;

        public int Atk;
        public int Def;

        public float MoveSpeed;

        public void Init(PlayerConfig config)
        {
            Hp = MaxHp = config.MaxHp;
            Atk = config.Atk; 
            Def = config.Def;
            MoveSpeed = config.MoveSpeed;
        }
    }
}
