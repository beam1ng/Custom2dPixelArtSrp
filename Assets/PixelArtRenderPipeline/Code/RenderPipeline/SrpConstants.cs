using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public static class SrpConstants
    {
        private const string DeferredShadingTag = "PixelArtDeferred";
        public static readonly ShaderTagId DeferredShadingTagId = new ShaderTagId(DeferredShadingTag);

        public const int MaxDirectionalLightsCount = 4;
        public const int MaxPointLightsCount = 8;
    }
}