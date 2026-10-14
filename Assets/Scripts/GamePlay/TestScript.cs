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
        ConfigManager.Instance.TryGetConfig<LiquidConfig>("Liquid_001", out var liquid1);
        ConfigManager.Instance.TryGetConfig<LiquidConfig>("Liquid_002", out var liquid2);
        ConfigManager.Instance.TryGetConfig<ItemConfig>("Item_001", out var item1);
    }
}
