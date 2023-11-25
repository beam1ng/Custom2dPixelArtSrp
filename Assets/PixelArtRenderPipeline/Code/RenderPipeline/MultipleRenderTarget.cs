using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public class MultipleRenderTarget
    {
        public RenderTexture _albedoRt;
        public RenderTexture _normalRt;
        public RenderTexture _depthRt;

        public bool isSetUp = false;
        
        // public RenderTargetIdentifier Albedo => (RenderTargetIdentifier)_albedoRt;
        // public RenderTargetIdentifier Normal => (RenderTargetIdentifier)_normalRt;
        // public RenderTargetIdentifier Depth => (RenderTargetIdentifier)_depthRt;

        public void SetupRenderTargets(int width, int height)
        {
            _albedoRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Albedo, width, height));
            _normalRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Normal, width, height));
            _depthRt = new RenderTexture(RtUtility.GetDescriptor(RtUtility.RtType.Depth, width, height));

            _albedoRt.name = "MRT_albedo";
            _normalRt.name = "MRT_normalRt";
            _depthRt.name = "MRT_depthRt";
            
            isSetUp = true;
        }
        
        public void CreateRenderTargets()
        {
            _albedoRt.Create();
            _normalRt.Create();
            _depthRt.Create();
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
