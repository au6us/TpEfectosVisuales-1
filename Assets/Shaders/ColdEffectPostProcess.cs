using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColdEffectPostProcess : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material postProcessMaterial;

        private RenderTargetIdentifier currentTarget;

        public void Setup(RenderTargetIdentifier source)
        {
            this.currentTarget = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (postProcessMaterial == null)
            {
                Debug.LogWarning("No post-process material assigned.");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("ColdEffectPostProcess");

            // Apply the post-processing material to the current render target
            Blit(cmd, currentTarget, currentTarget, postProcessMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    CustomRenderPass m_ScriptablePass;
    public Material postProcessMaterial;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configura el paso de renderización para que se ejecute después de todo el renderizado normal.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (postProcessMaterial != null)
        {
            m_ScriptablePass.Setup(renderer.cameraColorTarget);
            m_ScriptablePass.postProcessMaterial = postProcessMaterial;
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
