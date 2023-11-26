using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class PixelArtRenderer : MonoBehaviour
{
    public static readonly List<PixelArtRenderer> PixelArtRenderers = new List<PixelArtRenderer>();
    public Material material;
    public Mesh mesh;
    public Vector2Int texelResolution = new Vector2Int(32,32);

    private static readonly int TexelResolutionShaderId = Shader.PropertyToID("_TexelResolution");

    private void OnEnable()
    {
        PixelArtRenderers.Add(this);
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnDisable()
    {
        PixelArtRenderers.Remove(this);
    }
    private void Update()
    {
        material.SetVector(TexelResolutionShaderId,new Vector4(texelResolution.x,texelResolution.y,0,0));
    }

    private void OnDrawGizmosSelected()
    {
        var bounds = GeometryUtility.CalculateBounds(mesh.vertices, transform.localToWorldMatrix);
        Vector3 boundsMinWs = bounds.min;
        Vector3 boundsMaxWs = bounds.max;
        Vector3 boundsMinSs = Camera.current.WorldToScreenPoint(boundsMinWs);
        Vector3 boundsMaxSs = Camera.current.WorldToScreenPoint(boundsMaxWs);
        Vector3 boundsMinVs = Camera.current.ScreenToViewportPoint(boundsMinSs);
        Vector3 boundsMaxVs = Camera.current.ScreenToViewportPoint(boundsMaxSs);
        Vector3 boundsMinCs = boundsMinVs * 2 - new Vector3(1, 1, 0);
        Vector3 boundsMaxCs = boundsMaxVs * 2 - new Vector3(1, 1, 0);

        Vector3 leftUp = new Vector3(boundsMinCs.x, boundsMaxCs.y, 0);
        Vector3 rightUp = new Vector3(boundsMaxCs.x, boundsMaxCs.y, 0);
        Vector3 leftDown = new Vector3(boundsMinCs.x, boundsMinCs.y, 0);
        Vector3 rightDown = new Vector3(boundsMaxCs.x, boundsMinCs.y, 0);


        Gizmos.color = Color.yellow;
        Gizmos.matrix = Camera.current.cameraToWorldMatrix * GL.GetGPUProjectionMatrix(Camera.current.projectionMatrix,false).inverse;
        
        Gizmos.DrawLine(leftUp,rightUp);
        Gizmos.DrawLine(rightUp,rightDown);
        Gizmos.DrawLine(rightDown,leftDown);
        Gizmos.DrawLine(leftDown,leftUp);
    }
}
