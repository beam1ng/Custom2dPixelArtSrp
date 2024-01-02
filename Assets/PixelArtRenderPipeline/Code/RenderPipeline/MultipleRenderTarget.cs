using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public class MultipleRenderTarget
    {
        public RenderTexture AlbedoRt = null;
        public RenderTexture NormalRt = null;
        public RenderTexture DepthRt = null;

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

        public void CreateRenderTargets()
        {
            AlbedoRt.Create();
            NormalRt.Create();
            DepthRt.Create();
        }

        public void DisposeRenderTargets()
        {
            AlbedoRt.Release();
            NormalRt.Release();
            DepthRt.Release();
        }

        public static implicit operator RenderTargetIdentifier[](MultipleRenderTarget mrt)
        {
            return new RenderTargetIdentifier[] { mrt.AlbedoRt, mrt.NormalRt };
        }
    }
}