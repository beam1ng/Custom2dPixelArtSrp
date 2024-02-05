using NUnit.Framework;
using UnityEngine;

namespace PixelArtRenderPipeline.Tests.Tests.RenderPipelineEditModeTests.Components
{
    public class PixelArtRendererTests
    {
        private GameObject _testObject;
        private PixelArtRenderer _pixelArtRenderer;
        private Camera _testCamera;

        [SetUp]
        public void Setup()
        {
            _testObject = new GameObject();
            _testObject.AddComponent<MeshFilter>().sharedMesh = CreateCube();
            _pixelArtRenderer = _testObject.AddComponent<PixelArtRenderer>();
            _pixelArtRenderer.material = new Material(Shader.Find("PixelArtRp/PixelArtLit"));

            GameObject cameraObject = new GameObject();
            _testCamera = cameraObject.AddComponent<Camera>();
        }

        [TearDown]
        public void Teardown()
        {
            GameObject.DestroyImmediate(_testObject);
            GameObject.DestroyImmediate(_testCamera.gameObject);
        }

        [Test]
        public void PixelArtRenderer_InitializedCorrectly()
        {
            Assert.IsNotNull(_pixelArtRenderer.mesh);
            Assert.IsNotNull(_pixelArtRenderer.material);
            Assert.AreEqual(Matrix4x4.identity, _pixelArtRenderer.localToProxyWs);
            Assert.AreEqual(Vector3.up, _pixelArtRenderer.postPixelizationUpVectorWs);
        }

        [Test]
        public void PixelArtRenderer_AddedAndRemovedFromListCorrectly()
        {
            Assert.True(PixelArtRenderer.PixelArtRenderers.Contains(_pixelArtRenderer));

            _pixelArtRenderer.OnDisable();

            Assert.False(PixelArtRenderer.PixelArtRenderers.Contains(_pixelArtRenderer));
        }

        [Test]
        public void PixelArtRenderer_UpdateRenderingDataUpdatesMaterialProperties()
        {
            _pixelArtRenderer.UpdateRenderingData(_testCamera);

            Assert.Greater(_pixelArtRenderer.material.shader.GetPropertyCount(), 0);

            Assert.IsTrue(_pixelArtRenderer.material.HasVector("_PostPixelizationUpVectorWs"));
            Assert.IsTrue(_pixelArtRenderer.material.HasVector("_ProxyBoundsCs2d"));
            Assert.IsTrue(_pixelArtRenderer.material.HasVector("_ProxyBoundsWs2d"));
        }

        [Test]
        public void PixelArtRenderer_UpdatesProxyRotationCorrectly()
        {
            Quaternion expectedRotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
            _pixelArtRenderer.UpdateRenderingData(_testCamera);

            Assert.AreEqual(expectedRotation, _pixelArtRenderer.ProxyRotation);
        }

        [Test]
        public void PixelArtRenderer_AdjustsBoundsBasedOnTexelPixelSize()
        {
            var initialTexelPixelSize = _pixelArtRenderer.texelPixelSize;

            _pixelArtRenderer.texelPixelSize = 5;
            _pixelArtRenderer.UpdateRenderingData(_testCamera);

            Assert.AreNotEqual(initialTexelPixelSize, _pixelArtRenderer.texelPixelSize);
        }

        private Mesh CreateCube()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
            };

            int[] triangles =
            {
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                2, 4, 5,
                1, 2, 5,
                1, 5, 6,
                0, 7, 4,
                0, 4, 3,
                5, 4, 7,
                5, 7, 6,
                0, 6, 7,
                0, 1, 6
            };

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.Optimize();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}