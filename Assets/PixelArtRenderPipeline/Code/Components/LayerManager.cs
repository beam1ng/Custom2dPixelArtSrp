using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LayerManager : MonoBehaviour
{
    [field: SerializeField]
    public List<GameObject> pixelArtLayers { get; set; }

    [field: SerializeField]
    public float simulatedCameraXPosition { get; set; }

    private float CalculateLayerOffset(GameObject layer)
    {
        float paralaxDistance = layer.transform.position.z;
        //0 is no offset, 1 is offset of cameraXPosition (at infinite distance)
        float paralaxStrength = paralaxDistance / (Mathf.Abs(paralaxDistance) + 1);
        return paralaxStrength * simulatedCameraXPosition;
    }

    private void Update()
    {
        //for testing purposes
        if (Application.isPlaying)
        {
            simulatedCameraXPosition = 10 * Mathf.Sin(2 * Mathf.PI * Time.time / 10f);
            // simulatedCameraXPosition = Camera.main.transform.position.x;
            Camera.main.transform.position = new Vector3(
                simulatedCameraXPosition,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z
            );
            
            UpdateLayerPositions();
        }
    }

    public void UpdateLayerPositions()
    {
        for (int i = 0; i < pixelArtLayers.Count; i++)
        {
            var position = pixelArtLayers[i].transform.position;
            pixelArtLayers[i].transform.position = new Vector3(CalculateLayerOffset(pixelArtLayers[i]), position.y, position.z);
        }
    }
}
