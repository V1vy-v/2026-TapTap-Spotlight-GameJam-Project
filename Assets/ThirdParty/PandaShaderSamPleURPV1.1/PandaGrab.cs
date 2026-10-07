using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PandaGrab : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        const string rt_name = "_PandaGrabTex";
        static readonly int rt_ID = Shader.PropertyToID(rt_name);

        RTHandle m_GrabTexture;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = new RenderTextureDescriptor(2560, 1440, RenderTextureFormat.DefaultHDR, 0);
            RenderingUtils.ReAllocateIfNeeded(ref m_GrabTexture, descriptor, FilterMode.Point, TextureWrapMode.Clamp, name: rt_name);
            cmd.SetGlobalTexture(rt_ID, m_GrabTexture);
            ConfigureTarget(m_GrabTexture);
            ConfigureClear(ClearFlag.Color, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("PandaPass");
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, m_GrabTexture);
            cmd.SetGlobalTexture(rt_ID, m_GrabTexture);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            m_GrabTexture?.Release();
            m_GrabTexture = null;
        }
    }

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
    }
}
