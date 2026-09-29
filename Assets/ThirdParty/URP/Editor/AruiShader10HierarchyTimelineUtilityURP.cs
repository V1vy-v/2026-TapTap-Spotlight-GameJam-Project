using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public static class AruiShader10HierarchyTimelineUtilityURP
{
    public static TimelineClip FindTimelineClip(PlayableDirector director, AnimationClip animationClip)
    {
        if(director == null || animationClip == null) return null;

        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if(timeline == null) return null;

        foreach(TrackAsset track in timeline.GetOutputTracks())
        {
            TimelineClip result = FindTimelineClipRecursive(track, animationClip);
            if(result != null) return result;
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

    public static void 自动补齐子粒子K帧控制器(AruiShader10HierarchyKFrameGroupURP hierarchy)
    {
        if(hierarchy == null) return;

        ParticleSystemRenderer[] renderers = hierarchy.GetComponentsInChildren<ParticleSystemRenderer>(hierarchy.包含未激活子物体);

        Undo.RegisterCompleteObjectUndo(hierarchy, "扫描子粒子 K帧控制器");

        for(int index = 0; index < renderers.Length; index++)
        {
            ParticleSystemRenderer renderer = renderers[index];
            if(renderer == null) continue;

            AruiShader10KFrameControllerURP childController = renderer.GetComponent<AruiShader10KFrameControllerURP>();
            if(childController == null)
            {
                childController = Undo.AddComponent<AruiShader10KFrameControllerURP>(renderer.gameObject);
            }

            Undo.RecordObject(childController, "设置子粒子 K帧控制器");
            childController.目标渲染器 = renderer;
            childController.自动查找渲染器 = false;
            childController.使用独立材质覆盖 = true;
            childController.编辑器实时预览 = true;

            if(hierarchy.自动初始化常用轨道 &&
               childController.浮点轨道 != null &&
               childController.浮点轨道.Count == 0)
            {
                childController.初始化常用K帧轨道();
            }

            EditorUtility.SetDirty(childController);
        }

        hierarchy.扫描子层级K帧控制器();
        hierarchy.同步配置到全部子层级();
        EditorUtility.SetDirty(hierarchy);
    }

    public static void 创建或绑定主层级动画轨道(AruiShader10HierarchyKFrameGroupURP hierarchy)
    {
        if(hierarchy == null) return;

        自动补齐子粒子K帧控制器(hierarchy);

        PlayableDirector director = hierarchy.Timeline导演;
        if(director == null)
        {
            director = hierarchy.GetComponent<PlayableDirector>();
        }

        if(director == null)
        {
            director = Object.FindObjectOfType<PlayableDirector>();
        }

        if(director == null)
        {
            EditorUtility.DisplayDialog(
                "AruiShader1.0 主层级 Timeline K帧",
                "没有找到 PlayableDirector。请先在主层级或场景中创建 Timeline，再把 PlayableDirector 拖入主层级 K帧组。",
                "确定"
            );
            return;
        }

        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if(timeline == null)
        {
            EditorUtility.DisplayDialog(
                "AruiShader1.0 主层级 Timeline K帧",
                "当前 PlayableDirector 没有绑定 TimelineAsset。",
                "确定"
            );
            return;
        }

        if(hierarchy.主层级动画片段 != null)
        {
            TimelineClip existing = FindTimelineClip(director, hierarchy.主层级动画片段);
            if(existing != null)
            {
                Undo.RecordObject(hierarchy, "绑定已有主层级 Timeline K帧动画");
                hierarchy.Timeline导演 = director;
                hierarchy.控制台直接记录K帧 = true;
                hierarchy.优先使用Timeline播放头 = true;
                hierarchy.同步配置到全部子层级();
                EditorUtility.SetDirty(hierarchy);
                director.Evaluate();
                return;
            }
        }

        string timelinePath = AssetDatabase.GetAssetPath(timeline);
        string folder = string.IsNullOrEmpty(timelinePath)
            ? "Assets"
            : Path.GetDirectoryName(timelinePath).Replace("\\", "/");
        if(string.IsNullOrEmpty(folder)) folder = "Assets";

        string clipPath = AssetDatabase.GenerateUniqueAssetPath(
            folder + "/" + hierarchy.gameObject.name + "_AruiShader主层级K帧.anim"
        );

        AnimationClip animationClip = new AnimationClip();
        animationClip.name = hierarchy.gameObject.name + "_AruiShader主层级K帧";
        animationClip.frameRate = Mathf.Max(1f, hierarchy.K帧帧率);
        AssetDatabase.CreateAsset(animationClip, clipPath);

        Undo.RegisterCompleteObjectUndo(timeline, "创建 AruiShader 主层级 Timeline K帧轨道");

        AnimationTrack animationTrack = timeline.CreateTrack<AnimationTrack>(
            null,
            "AruiShader1.0 主层级K帧 - " + hierarchy.gameObject.name
        );

        TimelineClip timelineClip = animationTrack.CreateClip<AnimationPlayableAsset>();
        AnimationPlayableAsset asset = timelineClip.asset as AnimationPlayableAsset;
        asset.clip = animationClip;
        timelineClip.displayName = "AruiShader 子层级统一K帧";
        timelineClip.start = 0d;
        timelineClip.duration = 10d;

        director.SetGenericBinding(animationTrack, hierarchy.gameObject);

        Undo.RecordObject(hierarchy, "绑定主层级 Timeline K帧动画");
        hierarchy.Timeline导演 = director;
        hierarchy.主层级动画片段 = animationClip;
        hierarchy.控制台直接记录K帧 = true;
        hierarchy.优先使用Timeline播放头 = true;
        hierarchy.K帧帧率 = animationClip.frameRate;
        hierarchy.同步配置到全部子层级();

        EditorUtility.SetDirty(hierarchy);
        EditorUtility.SetDirty(director);
        EditorUtility.SetDirty(timeline);
        AssetDatabase.SaveAssets();
        director.Evaluate();

        Selection.activeObject = animationClip;
        EditorGUIUtility.PingObject(animationClip);
    }
}
