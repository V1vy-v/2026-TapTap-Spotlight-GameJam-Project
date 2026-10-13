using Framework;

namespace GamePlay.Session
{
    public sealed class GameplayBattleState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.Battle;
        public GameplayBattleState(GameplayRoot root, StateMachine<GameplayMode> sm) : base(root, sm)
        {
        }


    }
}