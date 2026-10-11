using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ø…∆∆ªµ’œ∞≠ŒÔ
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
