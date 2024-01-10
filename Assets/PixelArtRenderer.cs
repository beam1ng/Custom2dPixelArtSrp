using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[ExecuteAlways]
public class PixelArtRenderer : MonoBehaviour
{
    public static readonly List<PixelArtRenderer> PixelArtRenderers = new List<PixelArtRenderer>();
    public Material material;
    public Mesh mesh;
    public bool is3D = false;
    
    [Range(1,20)]
    public int texelPixelSize = 10;

    public int boundsProxyTexelWidth;
    public int boundsProxyTexelHeight;
    
    [HideInInspector]
    public Matrix4x4 localToProxyWs = Matrix4x4.identity;
    
    private Vector3 _postPixelizationUpVectorWs = Vector3.up;
    private Quaternion _proxyRotation = Quaternion.identity;
    private Vector4 _proxyBoundsWs2d = Vector4.zero;
    private Vector4 _proxyBoundsCs2d = Vector4.zero;
    private Vector4 _finalBoundsCs2d = Vector4.zero;
    private Vector4 _finalToProxyBoundsRatio = Vector4.zero;
    private Vector4 _finalBoundsCenterWs = Vector4.zero;

    private static readonly int ProxyBoundsWs2d = Shader.PropertyToID("_ProxyBoundsWs2d");
    private static readonly int ProxyBoundsCs2d = Shader.PropertyToID("_ProxyBoundsCs2d");
    private static readonly int FinalBoundsCs2d = Shader.PropertyToID("_FinalBoundsCs2d");
    private static readonly int PostPixelizationUpVectorWs = Shader.PropertyToID("_PostPixelizationUpVectorWs");
    private static readonly int FinalToProxyBoundsRatio = Shader.PropertyToID("_FinalToProxyBoundsRatio");
    private static readonly int FinalBoundsCenterWs = Shader.PropertyToID("_FinalBoundsCenterWs");

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
        _proxyRotation = Quaternion.FromToRotation(_postPixelizationUpVectorWs, Vector3.up);
        CalculateProxyBounds();
        CalculateFinalBounds();
        _finalToProxyBoundsRatio = new Vector4((_finalBoundsCs2d.z - _finalBoundsCs2d.x) / (_proxyBoundsCs2d.z - _proxyBoundsCs2d.x), (_finalBoundsCs2d.w - _finalBoundsCs2d.y) / (_proxyBoundsCs2d.w - _proxyBoundsCs2d.y), 0, 0);
        _postPixelizationUpVectorWs = Vector3.Cross(Vector3.back,Vector3.Cross(transform.rotation * Vector3.up,Vector3.back)).normalized;
        Debug.DrawRay(transform.position,_postPixelizationUpVectorWs,Color.red);
        
        material.SetKeyword(new LocalKeyword(material.shader, "PIXELIZATION3D"),is3D);
        material.SetVector(PostPixelizationUpVectorWs,_postPixelizationUpVectorWs);
        material.SetVector(ProxyBoundsCs2d,_proxyBoundsCs2d);
        material.SetVector(ProxyBoundsWs2d,_proxyBoundsWs2d);
        material.SetVector(FinalBoundsCs2d,_finalBoundsCs2d);
        material.SetVector(FinalToProxyBoundsRatio,_finalToProxyBoundsRatio);
        material.SetVector(FinalBoundsCenterWs,_finalBoundsCenterWs);
    }

    private void CalculateFinalBounds()
    {
        Bounds finalBoundsWs = GeometryUtility.CalculateBounds(mesh.vertices, transform.localToWorldMatrix);
        Vector3 finalBoundsMinSs = _currentCamera.WorldToScreenPoint(finalBoundsWs.min);
        Vector3 finalBoundsMaxSs = _currentCamera.WorldToScreenPoint(finalBoundsWs.max);
        Vector3 finalBoundsMinVs = _currentCamera.ScreenToViewportPoint(finalBoundsMinSs);
        Vector3 finalBoundsMaxVs = _currentCamera.ScreenToViewportPoint(finalBoundsMaxSs);
        Vector3 finalBoundsMinCs = finalBoundsMinVs * 2 - new Vector3(1, 1, 0);
        Vector3 finalBoundsMaxCs = finalBoundsMaxVs * 2 - new Vector3(1, 1, 0);
        _finalBoundsCs2d = new Vector4(finalBoundsMinCs.x, finalBoundsMinCs.y, finalBoundsMaxCs.x, finalBoundsMaxCs.y);
    }

    private void CalculateProxyBounds()
    {
        Bounds finalBoundsWs = GeometryUtility.CalculateBounds(mesh.vertices,  transform.localToWorldMatrix);
        localToProxyWs = Matrix4x4.Rotate(_proxyRotation) * Matrix4x4.Translate(-finalBoundsWs.center) * transform.localToWorldMatrix;
        
        Bounds proxyBoundsWs = GeometryUtility.CalculateBounds(mesh.vertices, localToProxyWs);

        Vector4 vertex1 = mesh.vertices[0];
        Vector4 vertex2 = mesh.vertices[1];
        Vector4 vertex3 = mesh.vertices[2];
        Vector4 vertex4 = mesh.vertices[3];

        vertex1.w = 1;
        vertex2.w = 1;
        vertex3.w = 1;
        vertex4.w = 1;
        
        Vector3 proxyBoundsMinWs = proxyBoundsWs.min;
        Vector3 proxyBoundsMaxWs = proxyBoundsWs.max;
        Vector3 proxyBoundsMinSs = _currentCamera.WorldToScreenPoint(proxyBoundsMinWs);
        Vector3 proxyBoundsMaxSs = _currentCamera.WorldToScreenPoint(proxyBoundsMaxWs);

        Vector2Int sizeExtension = new Vector2Int(
            texelPixelSize - (((int)proxyBoundsMaxSs.x - (int)proxyBoundsMinSs.x - 1) % texelPixelSize + 1),
            texelPixelSize - (((int)proxyBoundsMaxSs.y - (int)proxyBoundsMinSs.y - 1) % texelPixelSize + 1)
        );

        int sizeExtensionLeft = sizeExtension.y % 2 + sizeExtension.y / 2;
        int sizeExtensionRight = sizeExtension.y / 2;
        int sizeExtensionUp = sizeExtension.x % 2 + sizeExtension.x / 2;
        int sizeExtensionDown = sizeExtension.x / 2;

        proxyBoundsMinSs.x -= sizeExtensionLeft;
        proxyBoundsMinSs.y -= sizeExtensionDown;
        proxyBoundsMaxSs.x += sizeExtensionRight;
        proxyBoundsMaxSs.y += sizeExtensionUp;

        proxyBoundsMinWs =  _currentCamera.ScreenToWorldPoint(proxyBoundsMinSs);
        proxyBoundsMaxWs =  _currentCamera.ScreenToWorldPoint(proxyBoundsMaxSs);

        _proxyBoundsWs2d = new Vector4(proxyBoundsMinWs.x, proxyBoundsMinWs.y, proxyBoundsMaxWs.x, proxyBoundsMaxWs.y);

        boundsProxyTexelWidth = ((int)proxyBoundsMaxSs.x - (int)proxyBoundsMinSs.x) / texelPixelSize;
        boundsProxyTexelHeight = ((int)proxyBoundsMaxSs.y - (int)proxyBoundsMinSs.y) / texelPixelSize;

        Vector3 proxyBoundsMinVs = _currentCamera.ScreenToViewportPoint(proxyBoundsMinSs);
        Vector3 proxyBoundsMaxVs = _currentCamera.ScreenToViewportPoint(proxyBoundsMaxSs);
        Vector3 proxyBoundsMinCs = proxyBoundsMinVs * 2 - new Vector3(1, 1, 0);
        Vector3 proxyBoundsMaxCs = proxyBoundsMaxVs * 2 - new Vector3(1, 1, 0);

        _finalBoundsCenterWs = finalBoundsWs.center;
        _finalBoundsCenterWs.w = 1;

        _proxyBoundsCs2d = new Vector4(proxyBoundsMinCs.x, proxyBoundsMinCs.y, proxyBoundsMaxCs.x, proxyBoundsMaxCs.y);
    }
}
