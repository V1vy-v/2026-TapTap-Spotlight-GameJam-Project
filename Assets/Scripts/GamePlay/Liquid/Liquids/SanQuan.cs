namespace GamePlay.Liquid
{
    /// <summary>
    /// 三拳：主形态被动（MainPassive）—— 装备在主槽即生效，不按 K、无冷却。
    /// 待补：用 Attack() 接管普攻，实现三段拳（蓄力突进 / 跳跃下砸）。
    /// </summary>
    public class SanQuan : LiquidBase
    {
        protected override void OnSkillBegin()
        {
            
        }
        public override bool Attack()
        {
            return base.Attack();
        }
    }
}