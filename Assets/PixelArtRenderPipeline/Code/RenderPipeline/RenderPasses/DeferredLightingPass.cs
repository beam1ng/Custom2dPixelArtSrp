using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline.RenderPasses
{
    public static class DeferredLightingPass
    {
        public static void DeferredLighting(ScriptableRenderContext context, MultipleRenderTarget gBuffer)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            Material mat = new Material(Shader.Find("PixelArtRp/DeferredLighting"));
            mat.SetTexture("_Albedo", gBuffer.AlbedoRt);
            mat.SetTexture("_Normal", gBuffer.NormalRt);
            mat.SetTexture("_Depth", gBuffer.DepthRt);
            cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, mat);
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }
    }
}