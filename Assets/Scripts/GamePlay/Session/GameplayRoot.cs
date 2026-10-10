using Framework;

/// <summary>
/// Gameplay控制器，负责接管本局状态
/// </summary>
public sealed class GameplayRoot : SubSystemBase
{
    public override int Priority => (int)SubSystemPriority.GameplayRoot;

    private StroyContext stroyContext;
    private LevelContext levelContext;
    private BattleContext battleContext;

    public override void Init()
    {
        
    }

    public void StartRun()
    {
        //创建主角状态

    }
    public void EndRun()
    {

    }
    public void EnterLevel()
    {
        //创建关卡状态

    }
    public void ExitLevel()
    {

    }
    public void StartBattle()
    {
        //创建战斗状态

    }
    public void EndBattle()
    {

    }
}