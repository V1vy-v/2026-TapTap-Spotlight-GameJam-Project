using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class AruiShader10KFrameFloatTrackURP
{
    [Tooltip("启用后，当前数值会在每一帧写入对应的 Shader 属性。")]
    public bool 启用 = true;

    [Tooltip("例如：_DissolveAmount、_Opacity、_FresnelIntensity、_UseFresnelModule。Toggle 使用 0 或 1。")]
    public string Shader属性名 = "_DissolveAmount";

    [Tooltip("此值可以在 Animation 或 Timeline 中直接 K 帧。")]
    public float 值;
}

[Serializable]
public class AruiShader10KFrameColorTrackURP
{
    [Tooltip("启用后，当前颜色会在每一帧写入对应的 Shader 属性。")]
    public bool 启用 = true;

    [Tooltip("例如：_SingleColor、_FresnelColor、_FresnelInnerColor。")]
    public string Shader属性名 = "_SingleColor";

    [ColorUsage(true, true)]
    [Tooltip("此值可以在 Animation 或 Timeline 中直接 K 帧。")]
    public Color 颜色 = Color.white;
}

[Serializable]
public class AruiShader10KFrameVectorTrackURP
{
    [Tooltip("启用后，当前 Vector4 会在每一帧写入对应的 Shader 属性。")]
    public bool 启用 = true;

    [Tooltip("常用：_MainTex_ST、_MaskTex_ST、_DissolveTex_ST、_DissolveExtraTex_ST、_DistortMaskTex_ST。")]
    public string Shader属性名 = "_MainTex_ST";

    [Tooltip("贴图 ST：X=Tiling X，Y=Tiling Y，Z=Offset X，W=Offset Y。此值可 K 帧。")]
    public Vector4 值 = new Vector4(1f, 1f, 0f, 0f);
}

[Serializable]
public class AruiShader10KFrameTextureTrackURP
{
    [Tooltip("启用后，当前贴图会在每一帧写入对应的 Shader 贴图属性。")]
    public bool 启用 = false;

    [Tooltip("例如：_MainTex、_SubTex、_MaskTex、_DissolveTex、_DissolveExtraTex、_DistortTex、_DistortMaskTex。")]
    public string Shader属性名 = "_MainTex";

    [Tooltip("此贴图引用可以在 Animation 或 Timeline 中直接 K 帧替换。留空会把该 Shader 贴图替换为 None。")]
    public Texture 贴图;
}

[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("AruiShader1.0/URP K帧控制器")]
public class AruiShader10KFrameControllerURP : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("不指定时，优先自动获取当前物体上的 ParticleSystemRenderer，再读取普通 Renderer。")]
    public Renderer 目标渲染器;

    [Min(0)]
    [Tooltip("目标 Renderer 的材质槽。粒子系统通常使用 0。")]
    public int 材质槽序号 = 0;

    [Tooltip("未指定目标渲染器时自动获取当前物体或子物体上的 Renderer。")]
    public bool 自动查找渲染器 = true;

    [Header("K帧运行设置")]
    [Tooltip("总开关。关闭后会把本组件管理的参数还原为材质原值。")]
    public bool 启用K帧控制 = true;

    [Tooltip("推荐开启：通过 MaterialPropertyBlock 做单对象材质参数覆盖，不会复制材质，也不影响其他共用材质的物体。")]
    public bool 使用独立材质覆盖 = true;

    [Tooltip("非播放状态也实时预览当前 K帧数值。")]
    public bool 编辑器实时预览 = true;

    [Tooltip("组件禁用或关闭 K帧控制时，恢复由本组件控制的属性到材质原值。")]
    public bool 关闭时恢复材质原值 = true;

    [Header("控制台直接 K帧录制")]
    [Tooltip("开启后，完整控制台调整数值、颜色、贴图、Tiling、Offset 时会直接写入下方动画片段。")]
    public bool 控制台直接记录K帧 = true;

    [Tooltip("完整控制台要写入的 Animation Clip。Timeline 可直接使用这份动画片段。")]
    public AnimationClip 控制台目标动画片段;

    [Min(0f)]
    [Tooltip("完整控制台写入关键帧的时间，单位：秒。")]
    public float 控制台K帧时间 = 0f;

    [Min(1f)]
    [Tooltip("完整控制台时间步进使用的帧率。")]
    public float 控制台K帧帧率 = 30f;

    [Header("Timeline 直接K帧")]
    [Tooltip("拖入当前 Timeline 所在物体上的 PlayableDirector。控制台会读取 Timeline 播放头位置写关键帧。")]
    public PlayableDirector Timeline导演;

    [Tooltip("开启后，控制台写关键帧时优先使用 Timeline 播放头时间；关闭时使用控制台K帧时间。")]
    public bool 优先使用Timeline播放头 = true;

    [Header("浮点 K帧轨道")]
    [Tooltip("Float、Range、Toggle 属性都可以填写。Toggle 填 0 或 1。")]
    public List<AruiShader10KFrameFloatTrackURP> 浮点轨道 = new List<AruiShader10KFrameFloatTrackURP>();

    [Header("颜色 K帧轨道")]
    public List<AruiShader10KFrameColorTrackURP> 颜色轨道 = new List<AruiShader10KFrameColorTrackURP>();

    [Header("向量 / Tiling Offset K帧轨道")]
    public List<AruiShader10KFrameVectorTrackURP> 向量轨道 = new List<AruiShader10KFrameVectorTrackURP>();

    [Header("贴图替换 K帧轨道")]
    [Tooltip("贴图引用可直接在 Animation / Timeline 中 K 帧替换。建议先“从当前材质读取到轨道”，再开始打帧。")]
    public List<AruiShader10KFrameTextureTrackURP> 贴图轨道 = new List<AruiShader10KFrameTextureTrackURP>();

    private MaterialPropertyBlock 属性块;

    private void Reset()
    {
        自动寻找目标渲染器();
        初始化常用K帧轨道();
        应用当前K帧值();
    }

    private void OnEnable()
    {
        确保列表有效();
        自动寻找目标渲染器();
        if(启用K帧控制)
        {
            应用当前K帧值();
        }
    }

    private void OnDisable()
    {
        if(关闭时恢复材质原值)
        {
            恢复组件覆盖到材质原值();
        }
    }

    private void OnValidate()
    {
        确保列表有效();
        材质槽序号 = Mathf.Max(0, 材质槽序号);
        自动寻找目标渲染器();

        if(!Application.isPlaying && 编辑器实时预览)
        {
            if(启用K帧控制)
            {
                应用当前K帧值();
            }
            else if(关闭时恢复材质原值)
            {
                恢复组件覆盖到材质原值();
            }
        }
    }

    private void Update()
    {
        if(!启用K帧控制) return;
        if(!Application.isPlaying && !编辑器实时预览) return;
        应用当前K帧值();
    }

    private void OnDidApplyAnimationProperties()
    {
        if(启用K帧控制)
        {
            应用当前K帧值();
        }
    }

    [ContextMenu("初始化常用 K帧轨道")]
    public void 初始化常用K帧轨道()
    {
        确保列表有效();
        浮点轨道.Clear();
        颜色轨道.Clear();
        向量轨道.Clear();
        贴图轨道.Clear();

        添加浮点("_Opacity", 1f);
        添加浮点("_MainAlphaStrength", 1f);
        添加浮点("_MainColorMix", 1f);
        添加浮点("_DissolveAmount", 0f);
        添加浮点("_DissolveExtraMode", 0f);
        添加浮点("_DissolveExtraStrength", 1f);
        添加浮点("_DistortMaskStrength", 1f);
        添加浮点("_VertexUseAnimMode", 0f);
        添加浮点("_EdgeHeatScaleX", 26f);
        添加浮点("_EdgeHeatScaleY", 26f);
        添加浮点("_DissolveSoftPreserveSolid", 1f);
        添加浮点("_DissolveEdgeIntensity", 1f);
        添加浮点("_FresnelIntensity", 1f);
        添加浮点("_DissolveAffectFresnel", 1f);
        添加浮点("_FresnelInnerIntensity", 1f);
        添加浮点("_DistortStrength", 0f);
        添加浮点("_FlowMapStrength", 0f);
        添加浮点("_VertexStrength", 0f);
        添加浮点("_RadialWaveRadius", 0f);
        添加浮点("_RadialWaveIntensity", 1f);
        添加浮点("_SpeedLineIntensity", 1f);
        添加浮点("_SpeedLineAlpha", 1f);

        添加颜色("_SingleColor", Color.white);
        添加颜色("_FresnelColor", Color.white);
        添加颜色("_FresnelInnerColor", Color.white);
        添加颜色("_DissolveEdgeColor", Color.white);

        添加向量("_MainTex_ST", new Vector4(1f, 1f, 0f, 0f));
        添加向量("_MaskTex_ST", new Vector4(1f, 1f, 0f, 0f));
        添加向量("_DissolveTex_ST", new Vector4(1f, 1f, 0f, 0f));
        添加向量("_DissolveExtraTex_ST", new Vector4(1f, 1f, 0f, 0f));
        添加向量("_DistortMaskTex_ST", new Vector4(1f, 1f, 0f, 0f));
        添加向量("_VertexOffsetMaskTex_ST", new Vector4(1f, 1f, 0f, 0f));

        // 默认创建常用贴图轨道但保持关闭，初始化时不会覆盖当前材质贴图。
        添加贴图("_MainTex", null, false);
        添加贴图("_SubTex", null, false);
        添加贴图("_MaskTex", null, false);
        添加贴图("_AuxMaskTex", null, false);
        添加贴图("_DissolveTex", null, false);
        添加贴图("_DissolveExtraTex", null, false);
        添加贴图("_DistortTex", null, false);
        添加贴图("_DistortMaskTex", null, false);
        添加贴图("_RampTex", null, false);
        添加贴图("_EdgeRampTex", null, false);
        添加贴图("_FresnelRampTex", null, false);
        添加贴图("_FlowMapTex", null, false);
        添加贴图("_VertexTex", null, false);
        添加贴图("_VertexOffsetMaskTex", null, false);
        添加贴图("_VATTex", null, false);
    }

    [ContextMenu("从当前材质读取到轨道")]
    public void 从当前材质读取到轨道()
    {
        Material material = 获取当前材质();
        if(material == null) return;

        确保列表有效();

        foreach(AruiShader10KFrameFloatTrackURP track in 浮点轨道)
        {
            if(track != null && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                track.值 = material.GetFloat(track.Shader属性名);
            }
        }

        foreach(AruiShader10KFrameColorTrackURP track in 颜色轨道)
        {
            if(track != null && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                track.颜色 = material.GetColor(track.Shader属性名);
            }
        }

        foreach(AruiShader10KFrameVectorTrackURP track in 向量轨道)
        {
            if(track != null && !string.IsNullOrEmpty(track.Shader属性名))
            {
                track.值 = 读取材质向量或贴图ST(material, track.Shader属性名, track.值);
            }
        }

        foreach(AruiShader10KFrameTextureTrackURP track in 贴图轨道)
        {
            if(track != null && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                track.贴图 = material.GetTexture(track.Shader属性名);
            }
        }

        应用当前K帧值();
    }

    [ContextMenu("立即应用当前 K帧值")]
    public void 应用当前K帧值()
    {
        Renderer renderer = 获取目标渲染器();
        Material material = 获取当前材质();
        if(renderer == null || material == null) return;

        确保列表有效();

        if(使用独立材质覆盖)
        {
            if(属性块 == null)
            {
                属性块 = new MaterialPropertyBlock();
            }

            int index = 获取安全材质槽(renderer);
            renderer.GetPropertyBlock(属性块, index);
            写入到属性块(material, 属性块);
            renderer.SetPropertyBlock(属性块, index);
        }
        else
        {
            写入到材质(material);
#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(material);
            }
#endif
        }
    }

    [ContextMenu("恢复组件覆盖到材质原值")]
    public void 恢复组件覆盖到材质原值()
    {
        if(!使用独立材质覆盖) return;

        Renderer renderer = 获取目标渲染器();
        Material material = 获取当前材质();
        if(renderer == null || material == null) return;

        if(属性块 == null)
        {
            属性块 = new MaterialPropertyBlock();
        }

        int index = 获取安全材质槽(renderer);
        renderer.GetPropertyBlock(属性块, index);

        foreach(AruiShader10KFrameFloatTrackURP track in 浮点轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                属性块.SetFloat(track.Shader属性名, material.GetFloat(track.Shader属性名));
            }
        }

        foreach(AruiShader10KFrameColorTrackURP track in 颜色轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                属性块.SetColor(track.Shader属性名, material.GetColor(track.Shader属性名));
            }
        }

        foreach(AruiShader10KFrameVectorTrackURP track in 向量轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名))
            {
                属性块.SetVector(track.Shader属性名, 读取材质向量或贴图ST(material, track.Shader属性名, track.值));
            }
        }

        foreach(AruiShader10KFrameTextureTrackURP track in 贴图轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                属性块.SetTexture(track.Shader属性名, material.GetTexture(track.Shader属性名));
            }
        }

        renderer.SetPropertyBlock(属性块, index);
    }

    public Renderer 获取目标渲染器()
    {
        自动寻找目标渲染器();
        return 目标渲染器;
    }

    public Material 获取当前材质()
    {
        Renderer renderer = 获取目标渲染器();
        if(renderer == null) return null;

        Material[] materials = renderer.sharedMaterials;
        if(materials == null || materials.Length == 0) return null;
        return materials[获取安全材质槽(renderer)];
    }

    private void 写入到属性块(Material material, MaterialPropertyBlock block)
    {
        foreach(AruiShader10KFrameFloatTrackURP track in 浮点轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                block.SetFloat(track.Shader属性名, track.值);
            }
        }

        foreach(AruiShader10KFrameColorTrackURP track in 颜色轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                block.SetColor(track.Shader属性名, track.颜色);
            }
        }

        foreach(AruiShader10KFrameVectorTrackURP track in 向量轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名))
            {
                block.SetVector(track.Shader属性名, track.值);
            }
        }

        foreach(AruiShader10KFrameTextureTrackURP track in 贴图轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                // Texture 引用支持 Animation / Timeline 的对象引用 K帧。
                block.SetTexture(track.Shader属性名, track.贴图);
            }
        }
    }

    private void 写入到材质(Material material)
    {
        foreach(AruiShader10KFrameFloatTrackURP track in 浮点轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                material.SetFloat(track.Shader属性名, track.值);
            }
        }

        foreach(AruiShader10KFrameColorTrackURP track in 颜色轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                material.SetColor(track.Shader属性名, track.颜色);
            }
        }

        foreach(AruiShader10KFrameVectorTrackURP track in 向量轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名))
            {
                写入材质向量或贴图ST(material, track.Shader属性名, track.值);
            }
        }

        foreach(AruiShader10KFrameTextureTrackURP track in 贴图轨道)
        {
            if(track != null && track.启用 && !string.IsNullOrEmpty(track.Shader属性名) && material.HasProperty(track.Shader属性名))
            {
                material.SetTexture(track.Shader属性名, track.贴图);
            }
        }
    }

    private void 自动寻找目标渲染器()
    {
        if(目标渲染器 != null || !自动查找渲染器) return;

        ParticleSystemRenderer particleRenderer = GetComponent<ParticleSystemRenderer>();
        if(particleRenderer != null)
        {
            目标渲染器 = particleRenderer;
            return;
        }

        Renderer renderer = GetComponent<Renderer>();
        if(renderer != null)
        {
            目标渲染器 = renderer;
            return;
        }

        ParticleSystemRenderer childParticleRenderer = GetComponentInChildren<ParticleSystemRenderer>(true);
        if(childParticleRenderer != null)
        {
            目标渲染器 = childParticleRenderer;
            return;
        }

        目标渲染器 = GetComponentInChildren<Renderer>(true);
    }

    private int 获取安全材质槽(Renderer renderer)
    {
        if(renderer == null) return 0;
        Material[] materials = renderer.sharedMaterials;
        if(materials == null || materials.Length == 0) return 0;
        return Mathf.Clamp(材质槽序号, 0, materials.Length - 1);
    }

    private static Vector4 读取材质向量或贴图ST(Material material, string propertyName, Vector4 fallback)
    {
        if(material == null || string.IsNullOrEmpty(propertyName)) return fallback;

        string textureProperty = 获取贴图属性名(propertyName);
        if(!string.IsNullOrEmpty(textureProperty) && material.HasProperty(textureProperty))
        {
            Vector2 scale = material.GetTextureScale(textureProperty);
            Vector2 offset = material.GetTextureOffset(textureProperty);
            return new Vector4(scale.x, scale.y, offset.x, offset.y);
        }

        return material.HasProperty(propertyName) ? material.GetVector(propertyName) : fallback;
    }

    private static void 写入材质向量或贴图ST(Material material, string propertyName, Vector4 value)
    {
        if(material == null || string.IsNullOrEmpty(propertyName)) return;

        string textureProperty = 获取贴图属性名(propertyName);
        if(!string.IsNullOrEmpty(textureProperty) && material.HasProperty(textureProperty))
        {
            material.SetTextureScale(textureProperty, new Vector2(value.x, value.y));
            material.SetTextureOffset(textureProperty, new Vector2(value.z, value.w));
            return;
        }

        if(material.HasProperty(propertyName))
        {
            material.SetVector(propertyName, value);
        }
    }

    private static string 获取贴图属性名(string propertyName)
    {
        if(string.IsNullOrEmpty(propertyName) || !propertyName.EndsWith("_ST", StringComparison.Ordinal)) return null;
        return propertyName.Substring(0, propertyName.Length - 3);
    }

    private void 添加浮点(string propertyName, float value)
    {
        浮点轨道.Add(new AruiShader10KFrameFloatTrackURP { 启用 = true, Shader属性名 = propertyName, 值 = value });
    }

    private void 添加颜色(string propertyName, Color color)
    {
        颜色轨道.Add(new AruiShader10KFrameColorTrackURP { 启用 = true, Shader属性名 = propertyName, 颜色 = color });
    }

    private void 添加向量(string propertyName, Vector4 value)
    {
        向量轨道.Add(new AruiShader10KFrameVectorTrackURP { 启用 = true, Shader属性名 = propertyName, 值 = value });
    }

    private void 添加贴图(string propertyName, Texture texture, bool enabled)
    {
        贴图轨道.Add(new AruiShader10KFrameTextureTrackURP { 启用 = enabled, Shader属性名 = propertyName, 贴图 = texture });
    }

    private void 确保列表有效()
    {
        if(浮点轨道 == null) 浮点轨道 = new List<AruiShader10KFrameFloatTrackURP>();
        if(颜色轨道 == null) 颜色轨道 = new List<AruiShader10KFrameColorTrackURP>();
        if(向量轨道 == null) 向量轨道 = new List<AruiShader10KFrameVectorTrackURP>();
        if(贴图轨道 == null) 贴图轨道 = new List<AruiShader10KFrameTextureTrackURP>();
    }
}
