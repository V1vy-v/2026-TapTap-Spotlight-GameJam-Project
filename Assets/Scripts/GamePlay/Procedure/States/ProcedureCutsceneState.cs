using Framework;

namespace GamePlay.Procedure
{
    public class ProcedureCutsceneState : ProcedureStateBase
    {
        public override ProcedureState StateKey => ProcedureState.Cutscene;

        public ProcedureCutsceneState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {

        }
        #endregion
    }
}