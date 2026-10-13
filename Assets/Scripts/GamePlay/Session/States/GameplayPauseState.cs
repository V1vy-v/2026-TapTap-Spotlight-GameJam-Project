using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayPauseState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.Pause;
        protected override LocalInputType InputMap => LocalInputType.UI;
        public GameplayPauseState(GameplayRoot root, StateMachine<GameplayMode> sm) : base(root, sm)
        {
        }


    }
}