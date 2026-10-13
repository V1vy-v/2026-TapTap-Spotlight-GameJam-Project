using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayFreeRoamState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.FreeRoam;
        public GameplayFreeRoamState(GameplayRoot root, StateMachine<GameplayMode> sm) : base(root, sm)
        {
        }


    }
}