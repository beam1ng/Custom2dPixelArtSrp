using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    /// <summary>
    /// Contains constants and static values used throughout the Render Pipeline.
    /// </summary>
    public static class SrpConstants
    {
        /// <summary>
        /// Tag used for identifying materials or shaders partaking in deferred shading.
        /// </summary>
        private const string DeferredShadingTag = "PixelArtDeferred";
    
        /// <summary>
        /// ShaderTagId associated with deferred shading.
        /// </summary>
        public static readonly ShaderTagId DeferredShadingTagId = new ShaderTagId(DeferredShadingTag);

        /// <summary>
        /// Maximum number of directional lights that the pipeline supports simultaneously in a scene.
        /// </summary>
        public const int MaxDirectionalLightsCount = 4;

        /// <summary>
        /// Maximum number of point lights that the pipeline supports simultaneously in a scene.
        /// </summary>
        public const int MaxPointLightsCount = 8;
    }
}