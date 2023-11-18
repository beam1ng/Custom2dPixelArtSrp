using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public class MultipleRenderTarget
    {
        public RenderTexture _albedoRt;
        public RenderTexture _normalRt;
        public RenderTexture _depthRt;
        
        // public RenderTargetIdentifier Albedo => (RenderTargetIdentifier)_albedoRt;
        // public RenderTargetIdentifier Normal => (RenderTargetIdentifier)_normalRt;
        // public RenderTargetIdentifier Depth => (RenderTargetIdentifier)_depthRt;
        
        public void SetupRenderTargets(CommandBuffer cmd, int width, int height)
        {
            _albedoRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Albedo, width, height));
            _normalRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Normal, width, height));
            _depthRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Depth, width, height));
        }

        public void DisposeRenderTargets()
        {
            _albedoRt.Release();
            _normalRt.Release();
            _depthRt.Release();
        }

        public static implicit operator RenderTargetIdentifier[](MultipleRenderTarget mrt)
        {
            return new RenderTargetIdentifier[] { mrt._albedoRt, mrt._normalRt};
        }
    }
}
