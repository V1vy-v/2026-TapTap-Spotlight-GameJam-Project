using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ø…∆∆ªµ’œ∞≠ŒÔ
/// </summary>
public class Obstacle_Breakable : Obstacle
{
    public override ObstacleType Type
    {
        get { return ObstacleType.Breakable; }
    }

    public override void OnPlayerHit()
    {
        
    }
}
