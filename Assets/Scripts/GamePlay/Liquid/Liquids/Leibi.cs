namespace GamePlay.Liquid
{
    /// <summary>
    /// 雷碧：「你跑不过我」。
    /// 持续期内的移速增益由 LiquidConfig.MoveSpeedBonus 提供，PlayerLiquids 已聚合，无需代码即可生效。
    /// 待补：穿过敌人时的 PassThroughDamage 判定（等索敌/敌人列表就绪后再接）。
    /// </summary>
    public class LeiBi : LiquidBase
    {
        protected override void OnSkillBegin()
        {
            
        }
    }
}