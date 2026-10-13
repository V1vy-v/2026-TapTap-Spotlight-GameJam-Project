using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayCutsceneState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.Cutscene;
        protected override LocalInputType InputMap => LocalInputType.UI;
        public GameplayCutsceneState(GameplayRoot root, StateMachine<GameplayMode> sm) : base(root, sm)
        {
        }


    }
}