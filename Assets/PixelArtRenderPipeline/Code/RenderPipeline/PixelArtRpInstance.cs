using PixelArtRenderPipeline.Code.RenderPipeline.RenderPasses;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    /// <summary>
    /// Custom render pipeline instance for rendering pixel art style graphics.
    /// </summary>
    public class PixelArtRpInstance : UnityEngine.Rendering.RenderPipeline
    {
        private PixelArtRpAsset _rpAsset;
        private MultipleRenderTarget mrt;
        private LightPass _lightPass = new LightPass();

        /// <summary>
        /// Initializes a new instance of the pixel art render pipeline with a specified asset.
        /// </summary>
        /// <param name="rpAsset">The render pipeline asset to configure settings.</param>
        public PixelArtRpInstance(PixelArtRpAsset rpAsset)
        {
            _rpAsset = rpAsset;
            mrt = new MultipleRenderTarget();
            _lightPass.Initialize();
        }

        /// <summary>
        /// Destructor to clean up resources when the instance is destroyed.
        /// </summary>
        ~PixelArtRpInstance()
        {
            _lightPass.Dispose();
        }

        /// <summary>
        /// Main rendering loop to execute per-frame rendering for each camera.
        /// </summary>
        /// <param name="context">Context to perform rendering.</param>
        /// <param name="cameras">Array of cameras to render this frame.</param>
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            SetUpFrameData(context);
            RenderCameras(context, cameras);
            DisposeFrameData();
        }

        /// <summary>
        /// Sets up the frame-level data before rendering begins.
        /// </summary>
        /// <param name="context">Current rendering context.</param>
        private void SetUpFrameData(ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            _lightPass.SetupLights();
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }

        /// <summary>
        /// Renders all the cameras in the scene.
        /// </summary>
        /// <param name="context">Current rendering context.</param>
        /// <param name="cameras">Cameras to render this frame.</param>
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

        /// <summary>
        /// Cleans up frame-level data after rendering is complete.
        /// </summary>
        private void DisposeFrameData()
        {
            
        }
    }
}