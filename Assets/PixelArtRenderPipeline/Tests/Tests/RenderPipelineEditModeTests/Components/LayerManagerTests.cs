using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


public class LayerManagerTests
{
    private GameObject layerManagerGameObject;
    private LayerManager layerManager;
    private GameObject cameraGameObject;

    [SetUp]
    public void SetUp()
    {
        layerManagerGameObject = new GameObject();
        layerManager = layerManagerGameObject.AddComponent<LayerManager>();

        cameraGameObject = new GameObject();
        cameraGameObject.AddComponent<Camera>();
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);

        layerManager.pixelArtLayers = new List<GameObject>();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(layerManagerGameObject);
        GameObject.DestroyImmediate(cameraGameObject);
    }

    [Test]
    public void LayerManager_SimulatedCameraXPosition_ZeroOffsetOnZeroDistance()
    {
        var layer = new GameObject();
        layer.transform.position = new Vector3(0f, 0f, 0f);
        layerManager.pixelArtLayers.Add(layer);
        layerManager.simulatedCameraXPosition = 5f;

        layerManager.UpdateLayerPositions();

        Assert.AreEqual(0f, layer.transform.position.x);
    }

    [Test]
    public void LayerManager_SimulatedCameraXPosition_PositiveOffsetOnPositiveDistance()
    {
        var layer = new GameObject();
        layer.transform.position = new Vector3(0f, 0f, 10f);
        layerManager.pixelArtLayers.Add(layer);
        layerManager.simulatedCameraXPosition = 5f;

        layerManager.UpdateLayerPositions();

        Assert.Greater(layer.transform.position.x,0);
    }

    [Test]
    public void LayerManager_SimulatedCameraXPosition_NegativeOffsetOnNegativeDistance()
    {
        var layer = new GameObject();
        layer.transform.position = new Vector3(0f, 0f, -10f);
        layerManager.pixelArtLayers.Add(layer);
        layerManager.simulatedCameraXPosition = 5f;

        layerManager.UpdateLayerPositions();

        Assert.Less(layer.transform.position.x, 0);
    }
}