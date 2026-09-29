using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Serialization;

/// <summary>
/// AruiPostEffect1.0 URP Timeline 后效片段。
/// 每个 Clip 单独选择一种后效；可使用自身独立参数，或读取相机控制器面板参数。
/// </summary>
[Serializable]
public sealed class AruiPostEffect10URPTimelineClip : PlayableAsset, ITimelineClipAsset
{
    [Header("Timeline 片段控制")]
    public AruiPostEffect10URP.后效功能 后效类型 = AruiPostEffect10URP.后效功能.震屏;
    [Tooltip("开启后，当前 Clip 使用下方独立参数；关闭后，读取相机上 AruiPostEffect1.0 控制器的同名功能参数。无论开关状态，Clip 都会在自身时间范围内触发效果。")]
    public bool 使用Timeline独立参数 = true;
    [Tooltip("控制 Clip 内后效的总强度。Clip 左边缘 = 触发时间，Clip 长度 = 持续时间。")]
    public AnimationCurve 播放强度曲线 = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

    [Header("各功能独立参数")]
    public AruiPostEffect10URP.震屏设置 震屏参数 = new AruiPostEffect10URP.震屏设置();
    public AruiPostEffect10URP.色相分离设置 色相分离参数 = new AruiPostEffect10URP.色相分离设置();
    public AruiPostEffect10URP.全屏模糊设置 模糊参数 = new AruiPostEffect10URP.全屏模糊设置();
    [FormerlySerializedAs("线条参数")]
    public AruiPostEffect10URP.电影黑边框设置 电影黑边框参数 = new AruiPostEffect10URP.电影黑边框设置();
    public AruiPostEffect10URP.全屏放射线设置 放射线参数 = new AruiPostEffect10URP.全屏放射线设置();
    public AruiPostEffect10URP.广角设置 广角参数 = new AruiPostEffect10URP.广角设置();
    public AruiPostEffect10URP.暗角设置 暗角参数 = new AruiPostEffect10URP.暗角设置();
    public AruiPostEffect10URP.技能黑白闪设置 技能黑白闪参数 = new AruiPostEffect10URP.技能黑白闪设置();
    public AruiPostEffect10URP.漫画滤镜设置 漫画滤镜参数 = new AruiPostEffect10URP.漫画滤镜设置();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<AruiPostEffect10URPTimelineBehaviour> playable = ScriptPlayable<AruiPostEffect10URPTimelineBehaviour>.Create(graph);
        AruiPostEffect10URPTimelineBehaviour behaviour = playable.GetBehaviour();
        behaviour.片段资产 = this;
        return playable;
    }

    public void 应用当前功能预设()
    {
        switch (后效类型)
        {
            case AruiPostEffect10URP.后效功能.震屏:
                AruiPostEffect10URP.应用震屏预设(震屏参数);
                break;
            case AruiPostEffect10URP.后效功能.色相分离:
                AruiPostEffect10URP.应用色相分离预设(色相分离参数);
                break;
            case AruiPostEffect10URP.后效功能.全屏模糊:
                AruiPostEffect10URP.应用全屏模糊预设(模糊参数);
                break;
            case AruiPostEffect10URP.后效功能.电影黑边框:
                AruiPostEffect10URP.应用电影黑边框预设(电影黑边框参数);
                break;
            case AruiPostEffect10URP.后效功能.全屏放射线:
                AruiPostEffect10URP.应用全屏放射线预设(放射线参数);
                break;
            case AruiPostEffect10URP.后效功能.广角:
                AruiPostEffect10URP.应用广角预设(广角参数);
                break;
            case AruiPostEffect10URP.后效功能.暗角:
                AruiPostEffect10URP.应用暗角预设(暗角参数);
                break;
            case AruiPostEffect10URP.后效功能.技能黑白闪:
                AruiPostEffect10URP.应用技能黑白闪预设(技能黑白闪参数);
                // 技能黑白闪通常只在命中瞬间存在。应用预设时同步生成一个
                // 0 → 1 → 1 → 0 的短促强度曲线，Clip 长度由 Timeline 自行决定。
                播放强度曲线 = new AnimationCurve(
                    new Keyframe(0f, 0f),
                    new Keyframe(0.12f, 1f),
                    new Keyframe(0.82f, 1f),
                    new Keyframe(1f, 0f));
                break;
            case AruiPostEffect10URP.后效功能.漫画滤镜:
                AruiPostEffect10URP.应用漫画滤镜预设(漫画滤镜参数);
                播放强度曲线 = new AnimationCurve(
                    new Keyframe(0f, 0f),
                    new Keyframe(0.10f, 1f),
                    new Keyframe(0.86f, 1f),
                    new Keyframe(1f, 0f));
                break;
        }
    }
}

/// <summary>
/// 仅保存 Clip 资产引用，具体混合与参数写入由 Mixer 统一处理。
/// </summary>
public sealed class AruiPostEffect10URPTimelineBehaviour : PlayableBehaviour
{
    public AruiPostEffect10URPTimelineClip 片段资产;
}
