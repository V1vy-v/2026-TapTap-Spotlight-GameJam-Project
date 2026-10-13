using Framework;

namespace GamePlay.Procedure
{
    /// <summary>已进入关卡。局内细分交给 GameplayRoot 的 GameplayMode 状态机。</summary>
    public sealed class ProcedurePlayingState : ProcedureStateBase
    {
        public override ProcedureState StateKey => ProcedureState.Playing;

        public ProcedurePlayingState(StateMachine<ProcedureState> sm, ProcedureCore procedure)
            : base(sm, procedure) { }

        public override void Enter()
        {
            if (Procedure.IsNewGame)
            {
                GameplayRoot.StartNewGame();
                Procedure.ClearNewGameTag();
            }
            else GameplayRoot.ContinueGame();
        }

        public override void Exit()
        {
            // 离开关卡的这条边 = 拆对局（内层状态机 / 玩家 / Session 数据）
            GameplayRoot.EndGame();
        }
    }
}