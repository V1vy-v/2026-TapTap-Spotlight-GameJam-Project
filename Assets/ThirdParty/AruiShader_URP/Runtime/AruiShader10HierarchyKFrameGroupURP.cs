using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("AruiShader1.0/URP 主层级 Timeline K帧组")]
public class AruiShader10HierarchyKFrameGroupURP : MonoBehaviour
{
    [Header("主层级 Timeline 动画系统")]
    [Tooltip("主层级的 PlayableDirector。Timeline Animation Track 将绑定到本物体。")]
    public PlayableDirector Timeline导演;

    [Tooltip("所有子层级 K帧控制器共享的一条主动画片段。")]
    public AnimationClip 主层级动画片段;

    [Tooltip("开启后，控制台修改任意子粒子参数时，会记录到这一条主动画片段。")]
    public bool 控制台直接记录K帧 = true;

    [Tooltip("优先使用 Timeline 播放头位置作为当前写帧时间。")]
    public bool 优先使用Timeline播放头 = true;

    [Min(0f)]
    [Tooltip("未使用 Timeline 播放头时采用的备用写帧时间，单位秒。")]
    public float 备用K帧时间 = 0f;

    [Min(1f)]
    [Tooltip("主动画片段与控制台写帧使用的帧率。")]
    public float K帧帧率 = 30f;

    [Header("子层级自动管理")]
    [Tooltip("扫描与自动添加时包含未激活子物体。")]
    public bool 包含未激活子物体 = true;

    [Tooltip("自动补齐 K帧控制器时，自动初始化常用参数轨道。")]
    public bool 自动初始化常用轨道 = true;

    [Tooltip("当前受主动画片段统一控制的子层级 K帧控制器。")]
    public List<AruiShader10KFrameControllerURP> 子层级K帧控制器 = new List<AruiShader10KFrameControllerURP>();

    public void 扫描子层级K帧控制器()
    {
        if(子层级K帧控制器 == null)
        {
            子层级K帧控制器 = new List<AruiShader10KFrameControllerURP>();
        }

        子层级K帧控制器.Clear();

        AruiShader10KFrameControllerURP[] controllers = GetComponentsInChildren<AruiShader10KFrameControllerURP>(包含未激活子物体);
        for(int index = 0; index < controllers.Length; index++)
        {
            AruiShader10KFrameControllerURP controller = controllers[index];
            if(controller == null) continue;
            if(!子层级K帧控制器.Contains(controller))
            {
                子层级K帧控制器.Add(controller);
            }
        }
    }

    public bool 包含控制器(AruiShader10KFrameControllerURP controller)
    {
        if(controller == null) return false;

        if(子层级K帧控制器 == null || 子层级K帧控制器.Count == 0)
        {
            扫描子层级K帧控制器();
        }

        return 子层级K帧控制器.Contains(controller);
    }

    public void 同步配置到全部子层级()
    {
        if(子层级K帧控制器 == null || 子层级K帧控制器.Count == 0)
        {
            扫描子层级K帧控制器();
        }

        for(int index = 0; index < 子层级K帧控制器.Count; index++)
        {
            AruiShader10KFrameControllerURP controller = 子层级K帧控制器[index];
            if(controller == null) continue;

            controller.控制台直接记录K帧 = 控制台直接记录K帧;
            controller.控制台目标动画片段 = 主层级动画片段;
            controller.控制台K帧帧率 = K帧帧率;
            controller.Timeline导演 = Timeline导演;
            controller.优先使用Timeline播放头 = 优先使用Timeline播放头;
        }
    }

    private void OnValidate()
    {
        K帧帧率 = Mathf.Max(1f, K帧帧率);
        备用K帧时间 = Mathf.Max(0f, 备用K帧时间);

        if(!Application.isPlaying)
        {
            扫描子层级K帧控制器();
        }
    }

    [ContextMenu("扫描全部子层级 K帧控制器")]
    private void ContextScan()
    {
        扫描子层级K帧控制器();
    }

    [ContextMenu("同步主层级配置到全部子层级")]
    private void ContextSync()
    {
        同步配置到全部子层级();
    }
}
