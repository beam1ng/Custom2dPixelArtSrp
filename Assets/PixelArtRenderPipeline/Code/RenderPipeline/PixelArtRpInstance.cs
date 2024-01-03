using PixelArtRenderPipeline.Code.RenderPipeline.RenderPasses;
using UnityEditor;
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
            LightPass.SetupLights();
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }

        private void RenderCameras(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                mrt.SetupRenderTargets(camera.pixelWidth, camera.pixelHeight);
                mrt.CreateRenderTargets();

                context.SetupCameraProperties(camera);
                GBufferPass.DrawRenderers(context, camera, mrt);
                DeferredLightingPass.DeferredLighting(context, mrt);
                context.DrawSkybox(camera);

#if UNITY_EDITOR
                if (Application.isEditor && SceneView.lastActiveSceneView.drawGizmos)
                {
                    context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                    context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
                }
#endif

                context.Submit();
                mrt.DisposeRenderTargets();
            }
        }

        private void DisposeFrameData()
        {
        }
    }
}