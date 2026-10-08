using System;
using System.Collections.Generic;
using AK.Wwise;
using UnityEngine;
using WwiseEvent = AK.Wwise.Event;

namespace Framework.SubSystems
{
    [Serializable]
    public struct AudioBankEntry
    {
        public Bank bank;
        public bool loadOnStart;
    }

    [Serializable]
    public struct AudioEventEntry
    {
        public string key;
        public WwiseEvent evt;
    }

    [CreateAssetMenu(fileName = "AudioConfig", menuName = "GameCore/AudioConfig")]
    public class AudioConfig : ScriptableObjectSingleton<AudioConfig>
    {
        [SerializeField] private List<AudioBankEntry> banks = new();
        [SerializeField] private List<AudioEventEntry> events = new();

        [Header("Volume RTPC")]
        [SerializeField] private RTPC masterVolume;
        [SerializeField] private RTPC musicVolume;
        [SerializeField] private RTPC sfxVolume;

        public IReadOnlyList<AudioBankEntry> Banks => banks;
        public IReadOnlyList<AudioEventEntry> Events => events;
        public RTPC MasterVolume => masterVolume;
        public RTPC MusicVolume => musicVolume;
        public RTPC SfxVolume => sfxVolume;
    }
}