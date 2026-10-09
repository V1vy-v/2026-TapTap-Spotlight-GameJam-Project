using GamePlay.Enemy;
using UnityEngine;
/// <summary>
/// Åö×²Æ÷¹¦ÄÜÃ¶¾Ù
/// </summary>
public enum ZoneType { Detect, Attack }
/// <summary>
/// 
/// </summary>
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

        if (zoneType == ZoneType.Detect)
            _owner.playerInDetectRange = true;
        else
            _owner.playerInAttackRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType.Detect)
            _owner.playerInDetectRange = false;
        else
            _owner.playerInAttackRange = false;
    }
}