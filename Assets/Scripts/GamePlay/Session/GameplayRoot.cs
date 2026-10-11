using Framework;
using UnityEngine;

/// <summary>
/// Gameplay子系统，负责接管本局状态
/// </summary>
public sealed class GameplayRoot : SubSystemBase
{
    public override int Priority => (int)SubSystemPriority.GameplayRoot;

    private StoryContext _storyContext;
    private BattleContext _battleContext;

    public override void Init()
    {
        
    }

    public void StartNewGame()
    {
        // 实例化主角并初始化状态
        Global.Instantiate("Prefabs/Player/test_player");
        // 新建剧情状态
        _storyContext = new();
        // 进入初始剧情

    }
    public void ContinueGame()
    {

    }
    public void EndGame()
    {

    }
    public void SaveProgress()
    {

    }
    public void LoadProgress()
    {

    }
    public void EnterSegment()
    {
        // 进入剧情状态

    }
    public void AdvanceSegment()
    {

    }
    public void ExitSegment()
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