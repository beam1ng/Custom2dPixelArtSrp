using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PixelArtRenderPipeline.Code.RenderPipeline.RenderPasses
{
    public class LightPass
    {
        private static readonly int PointLightBuffer = Shader.PropertyToID("_PointLightDataBuffer");
        private static readonly int DirectionalLightBuffer = Shader.PropertyToID("_DirectionalLightDataBuffer");
        private static readonly int PointLightCount = Shader.PropertyToID("_PointLightCount");
        private static readonly int DirectionalLightCount = Shader.PropertyToID("_DirectionalLightCount");

        private ComputeBuffer _directionalLightBuffer;
        private ComputeBuffer _pointLightBuffer;
        
        public void Initialize()
        {
            _directionalLightBuffer = new ComputeBuffer(SrpConstants.MaxDirectionalLightsCount, 9 * sizeof(float), ComputeBufferType.Structured);
            _pointLightBuffer = new ComputeBuffer(SrpConstants.MaxPointLightsCount, 10 * sizeof(float), ComputeBufferType.Structured);
        }

        public void SetupLights()
        {
            CustomLight[] lights = GameObject.FindObjectsOfType<CustomLight>();

            CustomLight[] pointLights = lights.Where(l => l.Type == CustomLight.LightType.Point).ToArray();
            CustomLight[] directionalLights = lights.Where(l => l.Type == CustomLight.LightType.Directional).ToArray();

            SetupPointLights(pointLights);
            SetupDirectionalLights(directionalLights);
        }

        private void SetupDirectionalLights(CustomLight[] directionalLights)
        {
            int directionalLightCount = Math.Min(directionalLights.Length, SrpConstants.MaxDirectionalLightsCount);
            Shader.SetGlobalInt(DirectionalLightCount,directionalLightCount);
            if (directionalLightCount == 0)
            {
                return;
            }

            DirectionalLightData[] directionalLightDataArray = new DirectionalLightData[directionalLightCount];
            
            for (int lightIndex = 0; lightIndex < directionalLightCount; lightIndex++)
            {
                CustomLight currentCustomLight = directionalLights[lightIndex];

                Vector4 lightDir = -currentCustomLight.transform.forward;
                lightDir.w = 0;

                directionalLightDataArray[lightIndex] = new DirectionalLightData()
                {
                    DirectionToLight = lightDir,
                    Color = currentCustomLight.Color,
                    Intensity = currentCustomLight.Intensity
                };
            }
            
            _directionalLightBuffer.SetData(directionalLightDataArray);
            Shader.SetGlobalBuffer(DirectionalLightBuffer, _directionalLightBuffer);
        }

        private void SetupPointLights(CustomLight[] pointLights)
        {
            int pointLightCount = Math.Min(pointLights.Length, SrpConstants.MaxPointLightsCount);
            Shader.SetGlobalInt(PointLightCount,pointLightCount);
            if (pointLightCount == 0)
            {
                return;
            }
            
            
            PointLightData[] pointLightsDataArray = new PointLightData[pointLightCount];

            for (int lightIndex = 0; lightIndex < pointLightCount; lightIndex++)
            {
                CustomLight currentCustomLight = pointLights[lightIndex];

                Vector4 lightPos = currentCustomLight.transform.position;
                lightPos.w = 1;

                pointLightsDataArray[lightIndex] = new PointLightData()
                {
                    Position = lightPos,
                    Color = currentCustomLight.Color,
                    Range = currentCustomLight.Range,
                    Intensity = currentCustomLight.Intensity
                };
            }
            
            _pointLightBuffer.SetData(pointLightsDataArray);
            Shader.SetGlobalBuffer(PointLightBuffer, _pointLightBuffer);
        }

        public void Dispose()
        {
            _directionalLightBuffer.Release();
            _pointLightBuffer.Release();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DirectionalLightData
        {
            public Vector4 DirectionToLight;
            public Vector4 Color;
            public float Intensity;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PointLightData
        {
            public Vector4 Position;
            public Vector4 Color;
            public float Range;
            public float Intensity;
        }
    }
}