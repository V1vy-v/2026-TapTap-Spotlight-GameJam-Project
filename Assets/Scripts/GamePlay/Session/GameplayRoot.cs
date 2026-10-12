using Framework;
using GamePlay.Session;

namespace GamePlay
{
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
            //Global.Instantiate("Prefabs/Player/player_default");
            // 新建剧情状态
            _storyContext = new();
            // 新建战斗状态（空）
            _battleContext = new();
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
            //更新战斗状态

        }
        public void EndBattle()
        {

        }
    }
}