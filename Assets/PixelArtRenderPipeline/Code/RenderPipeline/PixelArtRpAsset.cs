using PixelArtRenderPipeline.Code.RenderPipeline;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "PixelArtRp/RpAsset")]
public class PixelArtRpAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new PixelArtRpInstance(this);
    }
}