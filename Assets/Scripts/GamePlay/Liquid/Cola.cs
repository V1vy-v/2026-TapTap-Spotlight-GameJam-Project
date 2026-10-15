using UnityEngine;

namespace GamePlay.Liquid
{
    /// <summary>
    /// 液体(可乐)相关逻辑
    /// </summary>
    public class Cola : LiquidBase
    {
        public override void Attack()
        {
            //可乐攻击逻辑
            //比如发射气泡弹幕等
            //伤害计算由预设体上脚本根据武器数据初始化，在弹幕脚本里处理
        }

        protected override void OnSkillBegin()
        {
            
        }
    }
}
