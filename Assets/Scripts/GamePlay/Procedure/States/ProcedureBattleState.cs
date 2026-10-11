using Framework;

namespace GamePlay.Procedure
{
    public class ProcedureBattleState : ProcedureStateBase
    {
        public override ProcedureState StateKey => throw new System.NotImplementedException();

        public ProcedureBattleState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {

        }
        #endregion
    }
}