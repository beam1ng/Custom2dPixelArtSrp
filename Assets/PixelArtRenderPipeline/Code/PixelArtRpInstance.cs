using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public class PixelArtRpInstance : RenderPipeline
{
    private PixelArtRpAsset _rpAsset;
    private CommandBuffer cmd;
    
    public PixelArtRpInstance(PixelArtRpAsset rpAsset)
    {
        _rpAsset = rpAsset;
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        InitializeData();
        cmd.ClearRenderTarget(true,true, new Color(0,0,0));
        context.ExecuteCommandBuffer(cmd);
        context.Submit();

        foreach (Camera camera in cameras)
        {
            RenderCamera(context, camera);
        }
        
        DisposeData();
    }

    private void RenderCamera(ScriptableRenderContext context, Camera camera)
    {
        context.SetupCameraProperties(camera);
        Vector3 dlDir = GameObject.FindObjectOfType<Light>().transform.forward;
        Shader.SetGlobalVector("_DirectionalLightDir", dlDir);
        
        camera.TryGetCullingParameters(out var cullingParameters);
        CullingResults cr = context.Cull(ref cullingParameters);
        ShaderTagId shaderTagId = new ShaderTagId("PixelArtRp");
        SortingSettings ss = new SortingSettings(camera);
        DrawingSettings ds = new DrawingSettings(shaderTagId, ss);
        FilteringSettings fs = FilteringSettings.defaultValue;
        
        context.DrawRenderers(cr, ref ds, ref fs);
        context.Submit();
    }

    private void InitializeData()
    {
        cmd = CommandBufferPool.Get("PixelArtRpRender");
    }

    private void DisposeData()
    {
        cmd.Release();
    }
}
