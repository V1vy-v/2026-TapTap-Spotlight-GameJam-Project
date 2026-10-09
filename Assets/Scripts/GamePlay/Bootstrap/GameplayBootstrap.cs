using Core;
using Framework;
using GamePlay.Procedure;
using UnityEngine;

namespace GamePlay
{
    /// <summary>
    /// 每局游戏启动入口
    /// </summary>
    public sealed class GameplayBootstrap : MonoSingleton<GameplayBootstrap>
    {
        protected override void Init()
        {
            AppCore.OnAppReady += OnAppReady;
        }

        private void OnAppReady()
        {
            Global.Register<GameplayRoot>();
        }
    }
}
