namespace GamePlay.Liquid
{
    /// <summary>
    /// 纯备用槽被动：料酒与冰美式共用。
    /// 效果全部来自 LiquidConfig 字段（PassiveMissChance / EnemyMissChance / AttackSpeedBonus），
    /// 由 PlayerLiquids 聚合读取，本身不需要任何代码。
    /// 单独建类是因为 LiquidBase 是抽象类；两者当前无差异，将来某一方要加行为时再拆。
    /// </summary>
    public class StandbyPassiveLiquid : LiquidBase
    {
        protected override void OnSkillBegin()
        {

        }
    }
}