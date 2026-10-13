using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayGameOverState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.GameOver;
        protected override LocalInputType InputMap => LocalInputType.UI;
        public GameplayGameOverState(GameplayRoot root, StateMachine<GameplayMode> sm) : base(root, sm)
        {
        }


    }
}