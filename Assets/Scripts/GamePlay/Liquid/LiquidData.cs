using UnityEngine;

/// <summary>
/// 液体(武器）数据配置
/// </summary>
[CreateAssetMenu(fileName = "LiquidData", menuName = "Liquid/液体武器数据")]
public class LiquidData : ScriptableObject
{
    [Header("基础信息")]
    public int id;
    public string liquidName; 

    [Header("数值")]
    public int damage;           // 伤害
    public float attackInterval; // 攻击间隔
}