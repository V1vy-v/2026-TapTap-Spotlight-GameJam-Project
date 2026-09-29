using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// AruiPostEffect1.0 URP Renderer Feature。
/// 添加到 Universal Renderer Data 后，会在每台挂有 AruiPostEffect1.0 URP 控制器的相机上执行。
/// 支持 Unity 2019.4 URP 7.x 与 Unity 2020.3 URP 10.x 的传统 Blit 路径。
/// </summary>
[Obsolete("Obsolete")]
public sealed class AruiPostEffect10URPRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public sealed class 设置
    {
        [Tooltip("后效注入时机。AfterRenderingTransparents 可保证场景、角色和透明粒子都已渲染完成。")]
        public RenderPassEvent 注入时机 = RenderPassEvent.AfterRenderingTransparents;
        [Tooltip("开启后，Scene View 也可按相机组件的“编辑器实时预览”显示效果。")]
        public bool 支持Scene视图预览 = true;
    }

    public 设置 Feature设置 = new 设置();
    private AruiPostEffect10URPPass pass;

    public override void Create()
    {
        pass = new AruiPostEffect10URPPass();
        pass.renderPassEvent = Feature设置 != null ? Feature设置.注入时机 : RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pass == null)
        {
            Create();
        }

        Camera currentCamera = renderingData.cameraData.camera;
        if (currentCamera == null)
        {
            return;
        }

        if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection)
        {
            return;
        }

        AruiPostEffect10URP controller = currentCamera.GetComponent<AruiPostEffect10URP>();
        if (controller == null || !controller.开启后效)
        {
            return;
        }

        if (currentCamera.cameraType == CameraType.SceneView && (!(Feature设置 != null && Feature设置.支持Scene视图预览) || !controller.编辑器实时预览))
        {
            return;
        }

        pass.renderPassEvent = Feature设置 != null ? Feature设置.注入时机 : RenderPassEvent.AfterRenderingTransparents;
        pass.设置目标(renderer.cameraColorTarget, controller);
        renderer.EnqueuePass(pass);
    }

    [Obsolete("Obsolete")]
    private sealed class AruiPostEffect10URPPass : ScriptableRenderPass
    {
        private readonly RenderTargetHandle 临时颜色纹理;
        private RenderTargetIdentifier 相机颜色目标;
        private AruiPostEffect10URP controller;

        public AruiPostEffect10URPPass()
        {
            临时颜色纹理.Init("_AruiPostEffect10URP_TemporaryColor");
        }

        public void 设置目标(RenderTargetIdentifier cameraColorTarget, AruiPostEffect10URP targetController)
        {
            相机颜色目标 = cameraColorTarget;
            controller = targetController;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (controller == null || !controller.准备URP渲染())
            {
                return;
            }

            Material material = controller.获取URP运行材质();
            if (material == null)
            {
                return;
            }

            CommandBuffer commandBuffer = CommandBufferPool.Get("AruiPostEffect1.0 URP");
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.msaaSamples = 1;
            descriptor.bindMS = false;

            commandBuffer.GetTemporaryRT(临时颜色纹理.id, descriptor, FilterMode.Bilinear);
            Blit(commandBuffer, 相机颜色目标, 临时颜色纹理.Identifier(), material, 0);
            Blit(commandBuffer, 临时颜色纹理.Identifier(), 相机颜色目标);

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public override void FrameCleanup(CommandBuffer commandBuffer)
        {
            if (commandBuffer != null)
            {
                commandBuffer.ReleaseTemporaryRT(临时颜色纹理.id);
            }
        }
    }
}
