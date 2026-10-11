using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 液体(武器)基类，用于
/// </summary>
public abstract class LiquidBase : MonoBehaviour
{
    public LiquidData data;//液体数据

    /// <summary>
    /// 具体攻击逻辑，由基类续写
    /// </summary>
    public abstract void Attack();
}
