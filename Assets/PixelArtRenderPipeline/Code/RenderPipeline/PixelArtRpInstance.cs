using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public class PixelArtRpInstance : UnityEngine.Rendering.RenderPipeline
    {
        private PixelArtRpAsset _rpAsset;
        private MultipleRenderTarget mrt;

        public PixelArtRpInstance(PixelArtRpAsset rpAsset)
        {
            _rpAsset = rpAsset;
            mrt = new MultipleRenderTarget();
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            SetUpFrameData(context);
            RenderCameras(context, cameras);
            DisposeFrameData();
        }

        private void SetUpFrameData(ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            SetupLights();
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0,0));
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }

        private void RenderCameras(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                // if (camera.cameraType != CameraType.SceneView) continue;
                if (!mrt.isSetUp)
                {
                    mrt.SetupRenderTargets(camera.pixelWidth,camera.pixelHeight);
                }
                
                mrt.CreateRenderTargets();

                context.SetupCameraProperties(camera);
                DrawRenderers(context, camera);
                DeferredLighting(context);

                mrt.DisposeRenderTargets();
            }
        }

        private void SetupLights()
        {
            Vector3 dlDir = GameObject.FindObjectOfType<Light>().transform.forward;
            Shader.SetGlobalVector("_DirectionalLightDir", dlDir);
        }

        private void DisposeFrameData()
        {
            
        }

        private void DrawRenderers(ScriptableRenderContext context, Camera camera)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.SetRenderTarget(mrt,mrt._depthRt);
            cmd.ClearRenderTarget(true,true,new Color(0,0,0,0));
            context.ExecuteCommandBuffer(cmd);
            
            camera.TryGetCullingParameters(out ScriptableCullingParameters cp);
            CullingResults cr = context.Cull(ref cp);
            DrawingSettings ds = new DrawingSettings(SrpConstants.DeferredShadingTagId, new SortingSettings(camera));
            FilteringSettings fs = new FilteringSettings(RenderQueueRange.all);
            
            context.DrawRenderers(cr, ref ds, ref fs);
            context.Submit();
            cmd.Release();
        }

        private void DeferredLighting(ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            Material mat = new Material(Shader.Find("PixelArtRp/DeferredLighting"));
            cmd.Blit(mrt._normalRt,BuiltinRenderTextureType.CameraTarget);//,mat);
            
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }
    }
}