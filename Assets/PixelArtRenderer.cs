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

    public int boundsProxyTexelWidth;
    public int boundsProxyTexelHeight;

    public Quaternion proxyRotation = Quaternion.identity;

    [Header("Post pix. up vector")]
    [SerializeField]
    private bool pPUVisBoundToRotation = true;
    
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
        // postPixelizationUpVector = Quaternion.AngleAxis(Time.time * 0, Vector3.forward) * (Vector3.up + Vector3.right);
        postPixelizationUpVector = Vector3.Cross(Vector3.back,Vector3.Cross(transform.rotation * Vector3.up,Vector3.back)).normalized;
        Debug.DrawRay(transform.position,postPixelizationUpVector);
        proxyRotation = Quaternion.FromToRotation(postPixelizationUpVector, Vector3.up);
        Debug.DrawRay(transform.position,proxyRotation*postPixelizationUpVector);
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
        //this doesn't work properly - doesn't take the proxy rotation into consideration

        Matrix4x4 trasnsformation = transform.localToWorldMatrix * Matrix4x4.Rotate(proxyRotation);

        Matrix4x4 t = Matrix4x4.TRS(transform.position, Quaternion.identity, transform.lossyScale);
        // Matrix4x4 trasnsformation = Matrix4x4.Rotate(proxyRotation) * transform.localToWorldMatrix;
        var bounds = GeometryUtility.CalculateBounds(mesh.vertices,  t);
        Debug.DrawLine(bounds.min,bounds.max);
        

        Vector4 vertex1 = mesh.vertices[0];
        Vector4 vertex2 = mesh.vertices[1];
        Vector4 vertex3 = mesh.vertices[2];
        Vector4 vertex4 = mesh.vertices[3];

        vertex1.w = 1;
        vertex2.w = 1;
        vertex3.w = 1;
        vertex4.w = 1;
        
        Debug.DrawLine(Matrix4x4.Rotate(proxyRotation) * vertex1,Matrix4x4.Rotate(proxyRotation) * vertex2);
        Debug.DrawLine(Matrix4x4.Rotate(proxyRotation) * vertex3,Matrix4x4.Rotate(proxyRotation) * vertex4);
        
        //final bounds
        Debug.DrawLine(transform.localToWorldMatrix * vertex1,transform.localToWorldMatrix * vertex2,Color.blue);
        Debug.DrawLine(transform.localToWorldMatrix * vertex3,transform.localToWorldMatrix * vertex4,Color.blue);
        
        Debug.DrawLine(trasnsformation * vertex1,trasnsformation * vertex2,Color.green);
        Debug.DrawLine(trasnsformation * vertex3,trasnsformation * vertex4,Color.green);
        
        //proxy bounds
        Debug.DrawLine(t * vertex1,t * vertex2,Color.red);
        Debug.DrawLine(t * vertex3,t * vertex4,Color.red);
        
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

        boundsProxyTexelWidth = ((int)boundsMaxSs.x - (int)boundsMinSs.x) / texelPixelSize;
        boundsProxyTexelHeight = ((int)boundsMaxSs.y - (int)boundsMinSs.y) / texelPixelSize;

        Vector3 boundsMinVs = _currentCamera.ScreenToViewportPoint(boundsMinSs);
        Vector3 boundsMaxVs = _currentCamera.ScreenToViewportPoint(boundsMaxSs);
        Vector3 boundsMinCs = boundsMinVs * 2 - new Vector3(1, 1, 0);
        Vector3 boundsMaxCs = boundsMaxVs * 2 - new Vector3(1, 1, 0);

        return new Vector4(boundsMinCs.x, boundsMinCs.y, boundsMaxCs.x, boundsMaxCs.y);
    }
}
