using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayFreeRoamState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.FreeRoam;

        /// <summary>完成后自动进入下一节点</summary>
        protected override bool AutoAdvanceOnFinish => true;

        public GameplayFreeRoamState(GameplayRoot root, StateMachine<GameplayMode> sm)
            : base(root, sm) { }

        public override void Enter()
        {
            base.Enter();
            Root.OnGoalReached += HandleGoalReached;
        }

        public override void Exit()
        {
            Root.OnGoalReached -= HandleGoalReached;
            base.Exit();
        }

        /// <summary>目标达成 → 交给基类 CheckStateChange 推进</summary>
        private void HandleGoalReached(string goalId)
        {
            var node = Root.Story.CurrentNode;

            // 节点声明了 goalId 就只认匹配的报告；没声明则接受任何报告
            if (node != null && !string.IsNullOrEmpty(node.goalId) && node.goalId != goalId) return;

            MarkFinished();
        }
    }
}