using NUnit.Framework;
using PixelArtRenderPipeline.Code.RenderPipeline;
using UnityEngine;


[TestFixture]
public class RtUtilityTests
{
    [Test]
    public void GetDescriptor_Albedo_ReturnsCorrectFormat()
    {
        var descriptor = RtUtility.GetDescriptor(RtUtility.RtType.Albedo, 100, 100);

        Assert.AreEqual(RenderTextureFormat.ARGB32, descriptor.colorFormat);
        Assert.AreEqual(0, descriptor.depthBufferBits);
    }

    [Test]
    public void GetDescriptor_Normal_ReturnsCorrectFormat()
    {
        var descriptor = RtUtility.GetDescriptor(RtUtility.RtType.Normal, 100, 100);

        Assert.AreEqual(RenderTextureFormat.ARGB32, descriptor.colorFormat);
        Assert.AreEqual(0, descriptor.depthBufferBits);
    }

    [Test]
    public void GetDescriptor_Depth_ReturnsCorrectFormat()
    {
        var descriptor = RtUtility.GetDescriptor(RtUtility.RtType.Depth, 100, 100);

        Assert.AreEqual(RenderTextureFormat.Depth, descriptor.colorFormat);
        Assert.AreEqual(16, descriptor.depthBufferBits);
    }
}