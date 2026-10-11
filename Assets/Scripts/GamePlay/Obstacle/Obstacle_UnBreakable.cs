using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ø…∆∆ªµ’œ∞≠ŒÔ
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
