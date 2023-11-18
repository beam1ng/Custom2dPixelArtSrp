using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public static class SrpConstants
    {
        //mrt
        // public const string AlbedoRtName = "_Albedo";
        // public const string NormalRtName = "_Normal";
        // public const string DepthRtName = "_Depth";
        //
        // public static readonly int AlbedoRtId = Shader.PropertyToID(AlbedoRtName);
        // public static readonly int NormalRtId = Shader.PropertyToID(NormalRtName);
        // public static readonly int DepthRtId = Shader.PropertyToID(DepthRtName);
        
        //tags
        public const string DeferredShadingTag = "PixelArtDeferred";
        public static readonly ShaderTagId DeferredShadingTagId = new ShaderTagId(DeferredShadingTag);
    }
}