using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// AruiPostEffect1.0 Timeline 混合器。
/// 同一功能出现重叠 Clip 时，使用当前强度更高的 Clip 参数，避免参数相互叠加。
/// </summary>
public sealed class AruiPostEffect10URPTimelineMixer : PlayableBehaviour
{
    private AruiPostEffect10URP boundController;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        AruiPostEffect10URP controller = playerData as AruiPostEffect10URP;
        if (controller == null)
        {
            return;
        }

        boundController = controller;
        controller.开始Timeline帧(playable.GetTime());

        int inputCount = playable.GetInputCount();
        for (int index = 0; index < inputCount; index++)
        {
            float inputWeight = playable.GetInputWeight(index);
            if (inputWeight <= 0.0001f)
            {
                continue;
            }

            Playable inputPlayable = playable.GetInput(index);
            ScriptPlayable<AruiPostEffect10URPTimelineBehaviour> clipPlayable = (ScriptPlayable<AruiPostEffect10URPTimelineBehaviour>)inputPlayable;
            AruiPostEffect10URPTimelineBehaviour behaviour = clipPlayable.GetBehaviour();
            AruiPostEffect10URPTimelineClip clip = behaviour != null ? behaviour.片段资产 : null;
            if (clip == null)
            {
                continue;
            }

            double duration = clipPlayable.GetDuration();
            double localTime = clipPlayable.GetTime();
            float normalizedProgress = duration > 0.000001d ? Mathf.Clamp01((float)(localTime / duration)) : 0f;
            float curveWeight = clip.播放强度曲线 != null ? Mathf.Clamp01(clip.播放强度曲线.Evaluate(normalizedProgress)) : 1f;
            controller.设置Timeline功能(clip.后效类型, inputWeight * curveWeight, normalizedProgress, clip);
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        if (boundController != null)
        {
            boundController.清空Timeline输入();
            boundController = null;
        }
    }
}
