using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 可破坏障碍物
/// </summary>
public class Obstacle_UnBreakable : Obstacle
{
    public override ObstacleType Type
    {
        get { return ObstacleType.UnBreakable; }
    }

    public override void OnPlayerHit()
    {

    }
}
