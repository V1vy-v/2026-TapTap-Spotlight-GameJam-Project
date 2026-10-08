using AK.Wwise;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using WwiseEvent = AK.Wwise.Event;

namespace Framework.SubSystems
{
    /// <summary>
    /// 全局音频管理器，提供音频控制功能
    /// </summary>
    public class AudioManager : SubSystemBase
    {
        private readonly Dictionary<string, WwiseEvent> _events = new();

        private Transform _container;
        private AudioConfig _config;
        private bool _banksLoaded;

        #region 属性
        public override int Priority => (int)SubSystemPriority.AudioManager;
        #endregion

        #region 音量管理
        /// <summary>
        /// 检查Wwise是否初始化，并延迟加载配置中的 Bank
        /// </summary>
        /// <returns></returns>
        private bool EnsureBanksLoaded()
        {
            if (_banksLoaded) return true;
            if (!AkSoundEngine.IsInitialized()) return false;

            foreach (AudioBankEntry entry in _config.Banks)
            {
                if (entry.loadOnStart && entry.bank != null)
                    entry.bank.Load();
            }

            _banksLoaded = true;
            return true;
        }

        /// <summary>
        /// 根据配置键找到 AK.Wwise.Event，在指定 GameObject 上发送 Event，返回 Playing ID
        /// </summary>
        /// <param name="key"></param>
        /// <param name="emitter"></param>
        /// <returns></returns>
        public uint Play(string key, GameObject emitter = null)
        {
            if (!EnsureBanksLoaded())
                return AkSoundEngine.AK_INVALID_PLAYING_ID;

            if (!_events.TryGetValue(key, out WwiseEvent evt) || !evt.IsValid())
            {
                Debug.LogError($"[AudioManager] Event 未配置或无效: {key}");
                return AkSoundEngine.AK_INVALID_PLAYING_ID;
            }

            GameObject target = emitter ? emitter : _container.gameObject;
            return evt.Post(target);
        }

        /// <summary>
        /// 设置全局 Wwise State，例如战斗、探索、暂停状态
        /// </summary>
        /// <param name="key"></param>
        /// <param name="emitter"></param>
        /// <param name="fadeMs"></param>
        public void Stop(string key, GameObject emitter = null, int fadeMs = 0)
        {
            if (!AkSoundEngine.IsInitialized()) return;
            if (!_events.TryGetValue(key, out WwiseEvent evt) || !evt.IsValid()) return;

            GameObject target = emitter ? emitter : _container.gameObject;
            evt.Stop(target, fadeMs);
        }

        /// <summary>
        /// 设置全局 Wwise State
        /// </summary>
        /// <param name="state"></param>
        public void SetState(State state)
        {
            if (state != null && state.IsValid())
                state.SetValue();
        }

        /// <summary>
        /// 在指定 GameObject 上设置 Switch，例如地面材质、角色形态或武器类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="emitter"></param>
        public void SetSwitch(Switch value, GameObject emitter)
        {
            if (value == null || !value.IsValid()) return;

            GameObject target = emitter ? emitter : _container.gameObject;
            value.SetValue(target);
        }

        /// <summary>
        /// 设置 Game Parameter, 传入 emitter 时作用于该对象，否则设置全局值
        /// </summary>
        /// <param name="rtpc"></param>
        /// <param name="value"></param>
        /// <param name="emitter"></param>
        public void SetRTPC(RTPC rtpc, float value, GameObject emitter = null)
        {
            if (rtpc == null || !rtpc.IsValid()) return;

            if (emitter)
                rtpc.SetValue(emitter, value);
            else
                rtpc.SetGlobalValue(value);
        }

        /// <summary>
        /// 调整主音量大小
        /// </summary>
        public void ChangeMasterVolume(float volume)
        {
            SetRTPC(_config.MasterVolume, volume);
        }

        /// <summary>
        /// 调整音乐大小
        /// </summary>
        public void ChangeMusicVolume(float volume)
        {
            SetRTPC(_config.MusicVolume, volume);
        }

        /// <summary>
        /// 调整音效大小
        /// </summary>
        public void ChangeSfxVolume(float volume)
        {
            SetRTPC(_config.SfxVolume, volume);
        }

        #endregion

        #region 生命周期
        public override void Init()
        {
            _config = AudioConfig.Instance;
            if (_config == null)
            {
                Debug.LogError("[AudioManager] 缺少 Resources/AudioConfig.asset");
                return;
            }

            foreach (AudioEventEntry entry in _config.Events)
            {
                if (!string.IsNullOrEmpty(entry.key) && entry.evt != null)
                    _events[entry.key] = entry.evt;
            }

            _container = new GameObject("[AudioManager]").transform;
            Object.DontDestroyOnLoad(_container.gameObject);
            _container.gameObject.AddComponent<AkGameObj>();
        }

        public override void Destroy()
        {
            if (AkSoundEngine.IsInitialized() && _container)
                AkSoundEngine.StopAll(_container.gameObject);

            if (_banksLoaded && _config != null)
            {
                foreach (AudioBankEntry entry in _config.Banks)
                {
                    if (entry.loadOnStart && entry.bank != null)
                        entry.bank.Unload();
                }
            }

            if (_container)
                Object.Destroy(_container.gameObject);

            _events.Clear();
            _container = null;
        }

        #endregion
    }
}