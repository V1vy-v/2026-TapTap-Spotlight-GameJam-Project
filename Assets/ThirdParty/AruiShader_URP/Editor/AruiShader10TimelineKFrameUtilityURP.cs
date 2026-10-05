using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public static class AruiShader10TimelineKFrameUtilityURP
{
    public static TimelineClip FindTimelineClip(PlayableDirector director, AnimationClip animationClip)
    {
        if(director == null || animationClip == null) return null;
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if(timeline == null) return null;

        foreach(TrackAsset track in timeline.GetOutputTracks())
        {
            TimelineClip clip = FindTimelineClipRecursive(track, animationClip);
            if(clip != null) return clip;
        }
        return null;
    }

    private static TimelineClip FindTimelineClipRecursive(TrackAsset track, AnimationClip animationClip)
    {
        if(track == null) return null;

        foreach(TimelineClip clip in track.GetClips())
        {
            AnimationPlayableAsset animationAsset = clip.asset as AnimationPlayableAsset;
            if(animationAsset != null && animationAsset.clip == animationClip)
            {
                return clip;
            }
        }

        foreach(TrackAsset child in track.GetChildTracks())
        {
            TimelineClip result = FindTimelineClipRecursive(child, animationClip);
            if(result != null) return result;
        }

        return null;
    }

    public static void CreateAndBindTrack(AruiShader10KFrameControllerURP controller)
    {
        if(controller == null) return;

        PlayableDirector director = controller.Timeline导演;
        if(director == null)
        {
            director = Object.FindObjectOfType<PlayableDirector>();
            if(director != null)
            {
                Undo.RecordObject(controller, "绑定 Timeline 导演");
                controller.Timeline导演 = director;
                EditorUtility.SetDirty(controller);
            }
        }

        if(director == null)
        {
            EditorUtility.DisplayDialog("AruiShader1.0 Timeline K帧", "场景中没有找到 PlayableDirector。请先创建 Timeline 并把 PlayableDirector 拖入 K帧控制器。", "确定");
            return;
        }

        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if(timeline == null)
        {
            EditorUtility.DisplayDialog("AruiShader1.0 Timeline K帧", "当前 PlayableDirector 没有绑定 TimelineAsset。", "确定");
            return;
        }

        if(controller.控制台目标动画片段 != null)
        {
            TimelineClip existing = FindTimelineClip(director, controller.控制台目标动画片段);
            if(existing != null)
            {
                EditorUtility.DisplayDialog("AruiShader1.0 Timeline K帧", "当前 K帧控制器已经绑定了 Timeline 动画片段，无需重复创建。", "确定");
                return;
            }
        }

        string timelinePath = AssetDatabase.GetAssetPath(timeline);
        string folder = string.IsNullOrEmpty(timelinePath) ? "Assets" : Path.GetDirectoryName(timelinePath).Replace("\\", "/");
        if(string.IsNullOrEmpty(folder)) folder = "Assets";

        string clipPath = AssetDatabase.GenerateUniqueAssetPath(
            folder + "/" + controller.gameObject.name + "_AruiShaderKFrame.anim"
        );

        AnimationClip animationClip = new AnimationClip();
        animationClip.name = controller.gameObject.name + "_AruiShaderKFrame";
        animationClip.frameRate = Mathf.Max(1f, controller.控制台K帧帧率);
        AssetDatabase.CreateAsset(animationClip, clipPath);

        Undo.RegisterCompleteObjectUndo(timeline, "创建 AruiShader Timeline K帧轨道");
        AnimationTrack track = timeline.CreateTrack<AnimationTrack>(null, "AruiShader1.0 K帧 - " + controller.gameObject.name);
        TimelineClip timelineClip = track.CreateClip<AnimationPlayableAsset>();
        AnimationPlayableAsset asset = timelineClip.asset as AnimationPlayableAsset;
        asset.clip = animationClip;
        timelineClip.displayName = "AruiShader K帧";
        timelineClip.start = 0d;
        timelineClip.duration = 10d;

        director.SetGenericBinding(track, controller.gameObject);

        Undo.RecordObject(controller, "绑定 AruiShader Timeline K帧动画");
        controller.Timeline导演 = director;
        controller.控制台目标动画片段 = animationClip;
        controller.控制台直接记录K帧 = true;
        controller.优先使用Timeline播放头 = true;
        controller.控制台K帧帧率 = animationClip.frameRate;

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(director);
        EditorUtility.SetDirty(timeline);
        AssetDatabase.SaveAssets();
        director.Evaluate();

        Selection.activeObject = animationClip;
        EditorGUIUtility.PingObject(animationClip);
    }
}
