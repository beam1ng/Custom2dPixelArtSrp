using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline.RenderPasses
{
    public static class GBufferPass
    {
        public static void DrawRenderers(ScriptableRenderContext context, Camera camera, MultipleRenderTarget gBuffer)
        {
            CommandBuffer cmd = CommandBufferPool.Get("_GBufferPass");
            cmd.SetRenderTarget(gBuffer, gBuffer.DepthRt);
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            foreach (var renderer in PixelArtRenderer.PixelArtRenderers)
            {
                renderer.UpdateRenderingData(camera);
                int width = renderer.boundsProxyTexelWidth;
                int height = renderer.boundsProxyTexelHeight;

                if (width <= 0 || height <= 0)
                {
                    continue;
                }

                cmd.GetTemporaryRT(Shader.PropertyToID("_Albedo_proxy"), width, height, 0, FilterMode.Point,
                    gBuffer.AlbedoRt.descriptor.graphicsFormat);
                cmd.GetTemporaryRT(Shader.PropertyToID("_Normal_proxy"), width, height, 0, FilterMode.Point,
                    gBuffer.NormalRt.descriptor.graphicsFormat);
                cmd.GetTemporaryRT(Shader.PropertyToID("_Depth_proxy"), width, height,
                    gBuffer.DepthRt.descriptor.depthBufferBits, FilterMode.Point,
                    gBuffer.DepthRt.descriptor.graphicsFormat);

                cmd.SetRenderTarget(new RenderTargetIdentifier[]
                {
                    Shader.PropertyToID("_Albedo_proxy"),
                    Shader.PropertyToID("_Normal_proxy"),
                }, Shader.PropertyToID("_Depth_proxy"));

                cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
                cmd.DrawMesh(renderer.mesh,
                    Matrix4x4.Translate(Vector3.Scale(renderer.transform.parent.position, Vector3.forward)) *
                    renderer.localToProxyWs, renderer.material, 0, 1);

                cmd.SetGlobalTexture(Shader.PropertyToID("_Albedo_proxy"), Shader.PropertyToID("_Albedo_proxy"));
                cmd.SetGlobalTexture(Shader.PropertyToID("_Normal_proxy"), Shader.PropertyToID("_Normal_proxy"));
                cmd.SetGlobalTexture(Shader.PropertyToID("_Depth_proxy"), Shader.PropertyToID("_Depth_proxy"));

                Mesh mesh = MeshUtility.GetQuad(width, height);

                cmd.SetRenderTarget(gBuffer, gBuffer.DepthRt);
                cmd.DrawMesh(mesh, Matrix4x4.identity, renderer.material, 0, 0);

                cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Albedo_proxy"));
                cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Normal_proxy"));
                cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Depth_proxy"));

                context.ExecuteCommandBuffer(cmd);
                context.Submit();
                cmd.Clear();
                Object.DestroyImmediate(mesh);
            }

            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }
    }
}