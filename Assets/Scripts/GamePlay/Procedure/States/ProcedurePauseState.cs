using Framework;

namespace GamePlay.Procedure
{
    public class ProcedurePauseState : ProcedureStateBase
    {
        public override ProcedureState StateKey => ProcedureState.Pause;

        public ProcedurePauseState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {

        }
        #endregion
    }
}