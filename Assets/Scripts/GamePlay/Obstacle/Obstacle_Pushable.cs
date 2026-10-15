using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 可破坏障碍物
/// </summary>
public class Obstacle_Pushable : Obstacle
{
    public override ObstacleType Type
    {
        get { return ObstacleType.Pushable; }
    }

    public override void OnPlayerHit()
    {

    }
}
