using NUnit.Framework;
using PixelArtRenderPipeline.Code.RenderPipeline;
using UnityEngine;

public class MeshUtilityTests
{
    [Test]
    public void Mesh_QuadHasCorrectVertexCount()
    {
        Mesh quad = MeshUtility.GetQuad();
        Assert.AreEqual(4, quad.vertices.Length);
    }

    [Test]
    public void Mesh_QuadHasCorrectTriangleCount()
    {
        Mesh quad = MeshUtility.GetQuad();
        Assert.AreEqual(6, quad.triangles.Length);
    }

    [Test]
    public void Mesh_QuadHasCorrectNormalsCount()
    {
        Mesh quad = MeshUtility.GetQuad();
        Assert.AreEqual(4, quad.normals.Length);
    }

    [Test]
    public void Mesh_QuadHasCorrectUVCount()
    {
        Mesh quad = MeshUtility.GetQuad();
        Assert.AreEqual(4, quad.uv.Length);
    }

    [Test]
    public void Mesh_QuadVerticesAreCorrect()
    {
        Mesh quad = MeshUtility.GetQuad();
        Vector3[] expectedVertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };
        Assert.AreEqual(expectedVertices, quad.vertices);
    }

    [Test]
    public void Mesh_QuadTrianglesAreCorrect()
    {
        Mesh quad = MeshUtility.GetQuad();
        int[] expectedTriangles = new int[] { 0, 2, 1, 2, 3, 1 };
        Assert.AreEqual(expectedTriangles, quad.triangles);
    }

    [Test]
    public void Mesh_QuadUVsAreCorrect()
    {
        Mesh quad = MeshUtility.GetQuad();
        Vector2[] expectedUVs = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        Assert.AreEqual(expectedUVs, quad.uv);
    }

    [Test]
    public void Mesh_QuadNormalsAreCorrect()
    {
        Mesh quad = MeshUtility.GetQuad();
        Vector3[] expectedNormals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        Assert.AreEqual(expectedNormals, quad.normals);
    }

    // Additional tests here could include checks for different sizes of quads,
    // ensuring that the method handles invalid dimensions gracefully, etc.
}
