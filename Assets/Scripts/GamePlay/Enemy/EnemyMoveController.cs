using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 怪物移动组件，封装 NavMeshAgent
/// </summary>
public class EnemyMoveController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }
    }

    public void MoveTo(Vector2 targetPos)
    {
        if (_agent == null || !_agent.isActiveAndEnabled) return;
        _agent.SetDestination(targetPos);
    }

    public void SetSpeed(float speed = 0)
    {
        if (_agent != null)
        {
            _agent.speed = speed;
            _agent.isStopped = (speed <= 0.01f);
        }
    }

    public void Stop()
    {
        if (_agent != null)
            _agent.isStopped = true;
    }

    public bool HasReachedDestination()
    {
        if (_agent == null) return false;
        if (_agent.pathPending) return false;
        return _agent.remainingDistance <= _agent.stoppingDistance;
    }
}