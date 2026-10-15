using GamePlay.Enemy;
using UnityEngine;
/// <summary>
/// Åö×²Æ÷¹¦ÄÜÃ¶¾Ù
/// </summary>
public enum ZoneType { Detect, Attack }

public class EnemyDetectZone : MonoBehaviour
{
    public ZoneType zoneType;

    private EnemyController _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        switch (zoneType)
        {
            case ZoneType.Detect:
                _owner.playerInDetectRange = true;
                _owner.SetPlayerTarget(other.transform);
                break;
            case ZoneType.Attack:
                _owner.playerInAttackRange = true;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        switch (zoneType)
        {
            case ZoneType.Detect:
                _owner.playerInDetectRange = false;
                _owner.MissPlayer();
                break;
            case ZoneType.Attack:
                _owner.playerInAttackRange = false;
                break;
        }
    }
}