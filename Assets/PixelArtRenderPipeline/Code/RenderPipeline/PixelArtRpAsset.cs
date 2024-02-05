using PixelArtRenderPipeline.Code.RenderPipeline;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// A ScriptableObject defining the asset for the custom Pixel Art Render Pipeline (PixelArtRp). i
/// </summary>
[CreateAssetMenu(menuName = "PixelArtRp/RpAsset")]
public class PixelArtRpAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new PixelArtRpInstance(this);
    }
}