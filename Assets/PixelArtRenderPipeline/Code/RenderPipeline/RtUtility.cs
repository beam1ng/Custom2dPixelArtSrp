using System;
using UnityEngine;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public static class RtUtility
    {
        public enum RtType
        {
            Albedo,
            Normal,
            Depth
        }

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