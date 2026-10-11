using Framework;

namespace GamePlay.Procedure
{
    public class ProcedureDialogueState : ProcedureStateBase
    {
        public override ProcedureState StateKey => ProcedureState.Dialogue;

        public ProcedureDialogueState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {

        }
        #endregion
    }
}