using Framework;

namespace GamePlay.Procedure
{
    public class ProcedureExploreState : ProcedureStateBase
    {
        public override ProcedureState StateKey => throw new System.NotImplementedException();

        public ProcedureExploreState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {

        }
        #endregion
    }
}