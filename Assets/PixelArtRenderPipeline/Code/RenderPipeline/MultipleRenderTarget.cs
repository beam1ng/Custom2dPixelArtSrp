using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    /// <summary>
    /// Manages multiple render targets for rendering operations, including albedo, normal, and depth textures.
    /// </summary>
    public class MultipleRenderTarget
    {
        public RenderTexture AlbedoRt = null;
        public RenderTexture NormalRt = null;
        public RenderTexture DepthRt = null;

        /// <summary>
        /// Initializes or resizes the render targets.
        /// </summary>
        /// <param name="width">The width of the render targets.</param>
        /// <param name="height">The height of the render targets.</param>
        public void SetupRenderTargets(int width, int height)
        {
            if (AlbedoRt == null || NormalRt == null || DepthRt == null)
            {
                AlbedoRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Albedo, width, height));
                NormalRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Normal, width, height));
                DepthRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Depth, width, height));
                AlbedoRt.name = "MRT_albedo";
                NormalRt.name = "MRT_normalRt";
                DepthRt.name = "MRT_depthRt";

                return;
            }

            AlbedoRt.width = width;
            NormalRt.width = width;
            DepthRt.width = width;

            AlbedoRt.height = height;
            NormalRt.height = height;
            DepthRt.height = height;
        }

        /// <summary>
        /// Allocates graphics resources for the render targets.
        /// </summary>
        public void CreateRenderTargets()
        {
            AlbedoRt.Create();
            NormalRt.Create();
            DepthRt.Create();
        }

        /// <summary>
        /// Releases the graphics resources used by the render targets.
        /// </summary>
        public void DisposeRenderTargets()
        {
            AlbedoRt.Release();
            NormalRt.Release();
            DepthRt.Release();
        }

        /// <summary>
        /// Allows implicit conversion to an array of RenderTargetIdentifier for ease of use with the Unity rendering API.
        /// This method doesn't return the RenderTargetIdentifier of the Depth render texture to make it better fit Unity
        /// graphics API.
        /// </summary>
        /// <param name="mrt">The MultipleRenderTarget instance to convert.</param>
        /// <returns>An array of RenderTargetIdentifier representing the Albedo and Normal render textures.</returns>
        public static implicit operator RenderTargetIdentifier[](MultipleRenderTarget mrt)
        {
            return new RenderTargetIdentifier[] { mrt.AlbedoRt, mrt.NormalRt };
        }
    }
}