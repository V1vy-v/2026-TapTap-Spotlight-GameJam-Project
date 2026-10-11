using Framework;

namespace GamePlay.Procedure
{
    public class ProcedureGameOverState : ProcedureStateBase
    {
        public override ProcedureState StateKey => ProcedureState.GameOver;

        public ProcedureGameOverState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {

        }
        #endregion
    }
}