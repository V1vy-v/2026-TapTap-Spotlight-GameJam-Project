using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayDialogueState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.Dialogue;
        protected override LocalInputType InputMap => LocalInputType.UI;
        public GameplayDialogueState(GameplayRoot root, StateMachine<GameplayMode> sm) : base(root, sm)
        {
        }


    }
}