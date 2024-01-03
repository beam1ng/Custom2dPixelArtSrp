using System;
using System.Linq;
using UnityEngine;

namespace PixelArtRenderPipeline.Code.RenderPipeline.RenderPasses
{
    public static class LightPass
    {
        public static void SetupLights()
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();

            Light[] pointLights = lights.Where(l => l.type == LightType.Point).ToArray();
            Light[] directionalLights = lights.Where(l => l.type == LightType.Directional).ToArray();

            SetupPointLights(pointLights);
            SetupDirectionalLights(directionalLights);
        }

        private static void SetupPointLights(Light[] pointLights)
        {
            int pointLightsCount = Math.Min(pointLights.Length, SrpConstants.MaxPointLightsCount);
            Shader.SetGlobalInt($"_CustomPointLightCount", pointLightsCount);

            for (int lightIndex = 0; lightIndex < pointLightsCount; lightIndex++)
            {
                Light currentLight = pointLights[lightIndex];

                Vector4 lightPos = currentLight.transform.position;
                lightPos.w = 1;
                Color lightColor = currentLight.color;

                Shader.SetGlobalVector($"_CustomPointLightPos{lightIndex}", lightPos);
                Shader.SetGlobalColor($"_CustomPointLightColor{lightIndex}", lightColor);
                Shader.SetGlobalFloat($"_CustomPointLightRange{lightIndex}", currentLight.range);
                Shader.SetGlobalFloat($"_CustomPointLightIntensity{lightIndex}", currentLight.intensity);
            }
        }

        private static void SetupDirectionalLights(Light[] directionalLights)
        {
            int directionalLightsCount = Math.Min(directionalLights.Length, SrpConstants.MaxDirectionalLightsCount);
            Shader.SetGlobalInt($"_CustomDirectionalLightCount", directionalLightsCount);

            for (int lightIndex = 0; lightIndex < directionalLightsCount; lightIndex++)
            {
                Light currentLight = directionalLights[lightIndex];

                Vector4 lightDir = -currentLight.transform.forward;
                lightDir.w = 0;
                Color lightColor = currentLight.color;

                Shader.SetGlobalVector($"_CustomDirectionalLightPos{lightIndex}", lightDir);
                Shader.SetGlobalColor($"_CustomDirectionalLightColor{lightIndex}", lightColor);
                Shader.SetGlobalFloat($"_CustomDirectionalLightRange{lightIndex}", currentLight.range);
                Shader.SetGlobalFloat($"_CustomDirectionalLightIntensity{lightIndex}", currentLight.intensity);
            }
        }
    }
}