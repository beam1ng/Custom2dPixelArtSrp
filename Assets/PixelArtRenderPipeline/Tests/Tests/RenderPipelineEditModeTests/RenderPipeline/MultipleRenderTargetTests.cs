using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PixelArtRenderPipeline.Code.RenderPipeline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

[TestFixture]
public class MultipleRenderTargetTests
{
    private MultipleRenderTarget mrt;

    [SetUp]
    public void SetUp()
    {
        mrt = new MultipleRenderTarget();
    }

    [Test]
    public void ShouldInitializeRenderTargetsProperly()
    {
        int width = 1024;
        int height = 768;

        mrt.SetupRenderTargets(width, height);

        Assert.NotNull(mrt.AlbedoRt);
        Assert.NotNull(mrt.NormalRt);
        Assert.NotNull(mrt.DepthRt);

        Assert.AreEqual(width, mrt.AlbedoRt.width);
        Assert.AreEqual(height, mrt.AlbedoRt.height);

        Assert.AreEqual(width, mrt.NormalRt.width);
        Assert.AreEqual(height, mrt.NormalRt.height);

        Assert.AreEqual(width, mrt.DepthRt.width);
        Assert.AreEqual(height, mrt.DepthRt.height);

        // Names
        Assert.AreEqual("MRT_albedo", mrt.AlbedoRt.name);
        Assert.AreEqual("MRT_normalRt", mrt.NormalRt.name);
        Assert.AreEqual("MRT_depthRt", mrt.DepthRt.name);
    }

    [Test]
    public void ShouldCreateAndDisposeRenderTargets()
    {
        mrt.SetupRenderTargets(128, 128);
        mrt.CreateRenderTargets();

        Assert.IsTrue(mrt.AlbedoRt.IsCreated());
        Assert.IsTrue(mrt.NormalRt.IsCreated());
        Assert.IsTrue(mrt.DepthRt.IsCreated());

        mrt.DisposeRenderTargets();

        Assert.IsFalse(mrt.AlbedoRt.IsCreated());
        Assert.IsFalse(mrt.NormalRt.IsCreated());
        Assert.IsFalse(mrt.DepthRt.IsCreated());
    }

    [Test]
    public void ShouldImplicitlyConvertToRenderTargetIdentifiers()
    {
        mrt.SetupRenderTargets(128, 128);
        RenderTargetIdentifier[] ids = mrt;
        
        Assert.AreEqual(2, ids.Length);
        Assert.AreEqual((RenderTargetIdentifier)mrt.AlbedoRt, ids[0]);
        Assert.AreEqual((RenderTargetIdentifier)mrt.NormalRt, ids[1]);
    }

    [TearDown]
    public void TearDown()
    {
        if(mrt != null)
        {
            mrt.DisposeRenderTargets();
            mrt = null;
        }
    }
}