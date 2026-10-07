using Framework;
using UnityEngine;
using YooAsset;

namespace Core
{
    /// <summary>
    /// 全局游戏设置
    /// </summary>
    [CreateAssetMenu(fileName = "AppConfig", menuName = "GameCore/AppConfig")]
    public class AppConfig : ScriptableObjectSingleton<AppConfig>
    {
        [Header("游戏基础设置")] 
        public int targetFrame = 60;
        
        [Header("资源加载")] 
        public EPlayMode resourceMode = EPlayMode.EditorSimulateMode;
        public float clearUnusedIdleTime = 30f;
        public string defaultPackageName = "DefaultPackage";
    }
}