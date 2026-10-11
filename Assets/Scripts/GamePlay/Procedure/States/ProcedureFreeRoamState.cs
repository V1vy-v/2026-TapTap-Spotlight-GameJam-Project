using Framework;

namespace GamePlay.Procedure
{
    public class ProcedureFreeRoamState : ProcedureStateBase
    {
        public override ProcedureState StateKey => ProcedureState.FreeRoam;

        public ProcedureFreeRoamState(StateMachine<ProcedureState> stateMachine, ProcedureCore procedure) : base(stateMachine, procedure)
        {

        }

        #region 状态周期
        public override void Enter()
        {
            //解锁输入系统


            //根据ProcedureCore的_isNewGame选择开新游戏还是读存档继续游戏
            if (Procedure.IsNewGame)
            {
                GameplayRoot.StartNewGame();
                Procedure.ClearNewGameTag();
            }
            else
                GameplayRoot.ContinueGame();
        }
        #endregion
    }
}