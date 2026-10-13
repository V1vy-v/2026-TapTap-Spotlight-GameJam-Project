using Framework;
using GamePlay.Procedure;

namespace GamePlay.Session
{
    public abstract class GameplayModeStateBase : StateBase<GameplayMode>
    {
        protected readonly GameplayRoot Root;
        protected ProcedureCore Procedure => ProcedureCore.Instance;

        protected GameplayModeStateBase(GameplayRoot root, StateMachine<GameplayMode> stateMachine)
            : base(stateMachine) 
        { 
            Root = root; 
        }

        /// <summary>该模式启用的 ActionMap。子类覆写为 UI 即可自动切换。</summary>
        protected virtual LocalInputType InputMap => LocalInputType.Player;

        public override void Enter()
        {
            Global.Get<LocalInputManager>().SwitchMap(InputMap);
        }
    }
}