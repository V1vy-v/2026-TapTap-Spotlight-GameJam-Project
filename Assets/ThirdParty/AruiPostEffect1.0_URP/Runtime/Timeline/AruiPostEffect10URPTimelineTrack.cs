using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// AruiPostEffect1.0 URP 后效轨道。
/// 将相机上的 AruiPostEffect1.0 控制器组件拖到轨道绑定槽，再添加后效片段即可使用。
/// </summary>
[TrackColor(0.30f, 0.70f, 1.00f)]
[TrackClipType(typeof(AruiPostEffect10URPTimelineClip))]
[TrackBindingType(typeof(AruiPostEffect10URP))]
public sealed class AruiPostEffect10URPTimelineTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<AruiPostEffect10URPTimelineMixer>.Create(graph, inputCount);
    }
}
