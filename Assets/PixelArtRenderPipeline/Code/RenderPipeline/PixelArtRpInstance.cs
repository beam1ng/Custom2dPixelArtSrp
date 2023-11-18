using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public class PixelArtRpInstance : UnityEngine.Rendering.RenderPipeline
    {
        private PixelArtRpAsset _rpAsset;
        private CommandBuffer cmd;
        private MultipleRenderTarget mrt;

        public PixelArtRpInstance(PixelArtRpAsset rpAsset)
        {
            _rpAsset = rpAsset;
            mrt = new MultipleRenderTarget();
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            InitializeFrameData();
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0));
            context.ExecuteCommandBuffer(cmd);
            context.Submit();


            foreach (Camera camera in cameras)
            {
                mrt.SetupRenderTargets(cmd, camera.pixelWidth, camera.pixelHeight);

                context.SetupCameraProperties(camera);
                context.ExecuteCommandBuffer(cmd);
                RenderCamera(context, camera);
                context.Submit();

                cmd.Clear();
                mrt.DisposeRenderTargets();

                if (camera == Camera.current)
                {
                    camera.targetTexture = mrt._albedoRt;
                }
            }

            DisposeFrameData();
        }

        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            camera.TryGetCullingParameters(out ScriptableCullingParameters cp);
            CullingResults cr = context.Cull(ref cp);
            DrawingSettings ds = new DrawingSettings(SrpConstants.DeferredShadingTagId, new SortingSettings(camera));
            FilteringSettings fs = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(cr, ref ds, ref fs);
        }

        private void InitializeFrameData()
        {
            cmd = CommandBufferPool.Get("PixelArtRpRender");
            SetupLights();
        }

        private void DisposeFrameData()
        {
            cmd.Release();
        }

        private void SetupLights()
        {
            Vector3 dlDir = GameObject.FindObjectOfType<Light>().transform.forward;
            Shader.SetGlobalVector("_DirectionalLightDir", dlDir);
        }
    }
}