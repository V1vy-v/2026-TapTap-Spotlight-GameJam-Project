using Framework;
/// <summary>
/// µĞÈË×´Ì¬»ùÀà
/// </summary>

namespace GamePlay.Enemy
{
    public abstract class EnemyStateBase : StateBase<EnemyState>
    {
        protected readonly EnemyController Enemy;

        protected EnemyStateBase(EnemyController enemy, StateMachine<EnemyState> stateMachine)
            : base(stateMachine)
        {
            Enemy = enemy;
        }
    }
}