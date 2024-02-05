using System;
using UnityEngine;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    /// <summary>
    /// Utility class providing methods related to Render Textures.
    /// </summary>
    public static class RtUtility
    {
        /// <summary>
        /// Enum defining types of render textures supported by the pipeline.
        /// </summary>
        public enum RtType
        {
            Albedo,
            Normal,
            Depth
        }

        /// <summary>
        /// Creates a RenderTextureDescriptor based on the type of render texture and specified dimensions.
        /// </summary>
        /// <param name="rtType">The type of render texture.</param>
        /// <param name="width">The width of the render texture.</param>
        /// <param name="height">The height of the render texture.</param>
        /// <returns>A configured RenderTextureDescriptor for the specified render texture type.</returns>
        public static RenderTextureDescriptor GetDescriptor(RtType rtType, int width, int height)
        {
            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(width, height);
            switch (rtType)
            {
                case RtType.Albedo:
                    descriptor.colorFormat = RenderTextureFormat.ARGB32;
                    descriptor.depthBufferBits = 0;
                    break;
                case RtType.Normal:
                    descriptor.colorFormat = RenderTextureFormat.ARGB32;
                    descriptor.depthBufferBits = 0;
                    break;
                case RtType.Depth:
                    descriptor.colorFormat = RenderTextureFormat.Depth;
                    descriptor.depthBufferBits = 16;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rtType), rtType, null);
            }

            return descriptor;
        }
    }
}