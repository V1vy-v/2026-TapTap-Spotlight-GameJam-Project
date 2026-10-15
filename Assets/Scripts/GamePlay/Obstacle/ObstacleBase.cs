using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public AudioClip hitSound;//受击音效
    public GameObject destoryEffect;//销毁特效

    public virtual ObstacleType Type
    {
        get { return ObstacleType.None; }
    }
    /// <summary>
    /// 受击反馈
    /// </summary>
    public virtual void OnPlayerHit() { }
}