using GamePlay.Configs;
using System.Collections;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(3);

        //测试逻辑
        ConfigManager.Instance.TryGetConfig<LiquidConfig>("liquid_001", out var liquid1);
        ConfigManager.Instance.TryGetConfig<LiquidConfig>("liquid_002", out var liquid2);
        ConfigManager.Instance.TryGetConfig<ItemConfig>("liquid_003", out var liquid3);
    }
}
