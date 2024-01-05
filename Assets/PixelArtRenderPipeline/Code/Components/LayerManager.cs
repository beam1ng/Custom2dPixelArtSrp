using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LayerManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> pixelArtLayers;

    [SerializeField]
    private float simulatedCameraXPosition;

    private float CalculateLayerOffset(GameObject layer)
    {
        float paralaxDistance = layer.transform.position.z;
        //0 is no offset, 1 is offset of cameraXPosition (at infinite distance)
        float paralaxStrength = paralaxDistance / (Mathf.Abs(paralaxDistance) + 1);
        return paralaxStrength * simulatedCameraXPosition;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            // simulatedCameraXPosition = Camera.main.transform.position.x;
            Camera.main.transform.position = new Vector3(
                simulatedCameraXPosition,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z
            );
        }
        
        for (int i = 0; i < pixelArtLayers.Count; i++)
        {
            var position = pixelArtLayers[i].transform.position;
            pixelArtLayers[i].transform.position = new Vector3(CalculateLayerOffset(pixelArtLayers[i]), position.y, position.z);
        }
    }
}
