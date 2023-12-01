using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class PixelArtRenderer : MonoBehaviour
{
    public static readonly List<PixelArtRenderer> PixelArtRenderers = new List<PixelArtRenderer>();
    public Material material;
    public Mesh mesh;
    
    [Range(1,20)]
    public int texelPixelSize = 10;

    public int boundsPixelWidth;
    public int boundsPixelHeight;

    [SerializeField]
    private Vector3 postPixelizationUpVector = Vector3.up;
    
    private Vector4 _rendererBoundsCs2d = Vector4.zero;

    private static readonly int RendererBoundsCs2d = Shader.PropertyToID("_RendererBoundsCs2d");
    private static readonly int PostPixelizationUpVector = Shader.PropertyToID("_PostPixelizationUpVector");

    private Camera _currentCamera;

    private void OnEnable()
    {
        PixelArtRenderers.Add(this);
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnDisable()
    {
        PixelArtRenderers.Remove(this);
    }

    public void UpdateRenderingData(Camera camera)
    {
        _currentCamera = camera;
        _rendererBoundsCs2d = CalculateRendererBounds();
        postPixelizationUpVector = Quaternion.AngleAxis(Time.time * 60, Vector3.forward) * Vector3.up;
        material.SetVector(PostPixelizationUpVector,postPixelizationUpVector);
        material.SetVector(RendererBoundsCs2d,_rendererBoundsCs2d);
    }

    private void OnDrawGizmosSelected()
    {
        Vector4 rendererBounds = _rendererBoundsCs2d;

        Vector3 leftUp = new Vector3(rendererBounds.x, rendererBounds.w, 0);
        Vector3 rightUp = new Vector3(rendererBounds.z, rendererBounds.w, 0);
        Vector3 leftDown = new Vector3(rendererBounds.x, rendererBounds.y, 0);
        Vector3 rightDown = new Vector3(rendererBounds.z, rendererBounds.y, 0);

        Gizmos.color = Color.yellow;
        Gizmos.matrix = _currentCamera.cameraToWorldMatrix * GL.GetGPUProjectionMatrix(_currentCamera.projectionMatrix,false).inverse;
        
        Gizmos.DrawLine(leftUp,rightUp);
        Gizmos.DrawLine(rightUp,rightDown);
        Gizmos.DrawLine(rightDown,leftDown);
        Gizmos.DrawLine(leftDown,leftUp);
    }

    private Vector4 CalculateRendererBounds()
    {
        var bounds = GeometryUtility.CalculateBounds(mesh.vertices, transform.localToWorldMatrix);
        Vector3 boundsMinWs = bounds.min;
        Vector3 boundsMaxWs = bounds.max;
        Vector3 boundsMinSs = _currentCamera.WorldToScreenPoint(boundsMinWs);
        Vector3 boundsMaxSs = _currentCamera.WorldToScreenPoint(boundsMaxWs);

        Vector2Int sizeExtension = new Vector2Int(
            texelPixelSize - (((int)boundsMaxSs.x - (int)boundsMinSs.x - 1) % texelPixelSize + 1),
            texelPixelSize - (((int)boundsMaxSs.y - (int)boundsMinSs.y - 1) % texelPixelSize + 1)
        );

        int sizeExtensionLeft = sizeExtension.y % 2 + sizeExtension.y / 2;
        int sizeExtensionRight = sizeExtension.y / 2;
        int sizeExtensionUp = sizeExtension.x % 2 + sizeExtension.x / 2;
        int sizeExtensionDown = sizeExtension.x / 2;

        boundsMinSs.x -= sizeExtensionLeft;
        boundsMinSs.y -= sizeExtensionDown;
        boundsMaxSs.x += sizeExtensionRight;
        boundsMaxSs.y += sizeExtensionUp;

        boundsPixelWidth = ((int)boundsMaxSs.x - (int)boundsMinSs.x) / texelPixelSize;
        boundsPixelHeight = ((int)boundsMaxSs.y - (int)boundsMinSs.y) / texelPixelSize;

        Vector3 boundsMinVs = _currentCamera.ScreenToViewportPoint(boundsMinSs);
        Vector3 boundsMaxVs = _currentCamera.ScreenToViewportPoint(boundsMaxSs);
        Vector3 boundsMinCs = boundsMinVs * 2 - new Vector3(1, 1, 0);
        Vector3 boundsMaxCs = boundsMaxVs * 2 - new Vector3(1, 1, 0);

        return new Vector4(boundsMinCs.x, boundsMinCs.y, boundsMaxCs.x, boundsMaxCs.y);
    }
}
