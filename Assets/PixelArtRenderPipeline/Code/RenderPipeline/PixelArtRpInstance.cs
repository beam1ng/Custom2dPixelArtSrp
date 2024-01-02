using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    public class PixelArtRpInstance : UnityEngine.Rendering.RenderPipeline
    {
        private PixelArtRpAsset _rpAsset;
        private MultipleRenderTarget mrt;

        public PixelArtRpInstance(PixelArtRpAsset rpAsset)
        {
            _rpAsset = rpAsset;
            mrt = new MultipleRenderTarget();
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            SetUpFrameData(context);
            RenderCameras(context, cameras);
            DisposeFrameData();
        }

        private void SetUpFrameData(ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            SetupLights();
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0,0));
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }

        private void RenderCameras(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                mrt.SetupRenderTargets(camera.pixelWidth,camera.pixelHeight);
                mrt.CreateRenderTargets();

                context.SetupCameraProperties(camera);
                DrawRenderers(context, camera);
                DeferredLighting(context);
                context.DrawSkybox(camera);
                
                #if UNITY_EDITOR
                if (Application.isEditor && SceneView.lastActiveSceneView.drawGizmos)
                {
                    context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                    context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
                }
                #endif
                
                context.Submit();
                
                mrt.DisposeRenderTargets();
            }
        }

        private void SetupLights()
        {
            Vector3 dlDir = GameObject.FindObjectOfType<Light>().transform.forward;
            Shader.SetGlobalVector("_DirectionalLightDir", dlDir);
        }

        private void DisposeFrameData()
        {
            
        }

        private void DrawRenderers(ScriptableRenderContext context, Camera camera)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.SetRenderTarget(mrt,mrt.DepthRt);
            cmd.ClearRenderTarget(true,true,new Color(0,0,0,0));
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            foreach (var renderer in PixelArtRenderer.PixelArtRenderers)
            {
                renderer.UpdateRenderingData(camera);
                int width = renderer.boundsProxyTexelWidth;
                int height = renderer.boundsProxyTexelHeight;

                if (width <= 0 || height <= 0)
                {
                    continue;
                }
                
                cmd.GetTemporaryRT(Shader.PropertyToID("_Albedo_proxy"), width,height,0,FilterMode.Point,mrt.AlbedoRt.descriptor.graphicsFormat);
                cmd.GetTemporaryRT(Shader.PropertyToID("_Normal_proxy"), width,height,0,FilterMode.Point,mrt.NormalRt.descriptor.graphicsFormat);
                cmd.GetTemporaryRT(Shader.PropertyToID("_Depth_proxy"), width,height,mrt.DepthRt.descriptor.depthBufferBits,FilterMode.Point,mrt.DepthRt.descriptor.graphicsFormat);
                
                cmd.SetRenderTarget(new RenderTargetIdentifier[]
                {
                    Shader.PropertyToID("_Albedo_proxy"),
                    Shader.PropertyToID("_Normal_proxy"),
                },Shader.PropertyToID("_Depth_proxy"));
                
                cmd.ClearRenderTarget(true,true, new Color(0,0,0,0));
                cmd.DrawMesh(renderer.mesh,Matrix4x4.Translate(Vector3.Scale(renderer.transform.parent.position,Vector3.forward)) *  renderer.localToProxyWs, renderer.material, 0, 1);
                
                cmd.SetGlobalTexture(Shader.PropertyToID("_Albedo_proxy"),Shader.PropertyToID("_Albedo_proxy"));
                cmd.SetGlobalTexture(Shader.PropertyToID("_Normal_proxy"),Shader.PropertyToID("_Normal_proxy"));
                cmd.SetGlobalTexture(Shader.PropertyToID("_Depth_proxy"),Shader.PropertyToID("_Depth_proxy"));
                
                Mesh mesh = GetQuad(width, height);
                
                cmd.SetRenderTarget(mrt,mrt.DepthRt);
                cmd.DrawMesh(mesh,Matrix4x4.identity, renderer.material,0,0);
                
                
                cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Albedo_proxy"));
                cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Normal_proxy"));
                cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Depth_proxy"));
                
                context.ExecuteCommandBuffer(cmd);
                context.Submit();
                cmd.Clear();
                Object.DestroyImmediate(mesh);
            }
            
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }

        private static Mesh GetQuad(int width, int height)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;
            return mesh;
        }

        private void DeferredLighting(ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            Material mat = new Material(Shader.Find("PixelArtRp/DeferredLighting"));
            mat.SetTexture("_Albedo",mrt.AlbedoRt);
            mat.SetTexture("_Normal",mrt.NormalRt);
            mat.SetTexture("_Depth",mrt.DepthRt);
            cmd.Blit(null,BuiltinRenderTextureType.CameraTarget,mat);
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            cmd.Release();
        }
    }
}