Shader "PixelArtRp/DeferredLighting"
{
    Properties
    {
        _Color("Color",Color) = (0,0,0,0)
        _MainTex("MainTex",2D) = "white"{}
        _Albedo ("Albedo (RGB)", 2D) = ""
        _Normal ("Albedo (RGB)", 2D) = ""
        _Depth ("Albedo (RGB)", 2D) = ""
    }

    SubShader
    {
        Pass
        {
            Blend Off
            Cull Off
            ZWrite On
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"
            #include "UnityShaderUtilities.cginc"
            #include "UnityPBSLighting.cginc"
            #include "HLSLSupport.cginc"

            #include "CustomLights.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCs : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 _DirectionalLightDir;

            sampler2D _MainTex;
            sampler2D _Albedo;
            sampler2D _Normal;
            sampler2D _Depth;
            float4 _MainTex_ST;
            float4 _Albedo_ST;
            float4 _Normal_ST;
            float4 _Depth_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _Albedo);
                o.positionCs = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i, out float depth: SV_Depth):SV_Target
            {
                float4 albedo = tex2D(_Albedo, i.uv);
                float4 normal = tex2D(_Normal, i.uv) * 2 - 1;
                depth = tex2D(_Depth, i.uv);

                float3 color;
                
                for (int lightIndex = 0; lightIndex < _DirectionalLightCount; lightIndex++)
                {
                    DirectionalLightData currentLight = _DirectionalLightDataBuffer[lightIndex];
                    float intensity = saturate(dot(normal, currentLight.directionToLight) * currentLight.intensity);
                    color += intensity * currentLight.color * albedo.rgb;
                }

                //DEBUG START todo: extract ws from depth and positionCs to process point lights

                // for (int lightIndex = 0; lightIndex < _PointLightCount; lightIndex++)
                // {
                //     PointLightData currentLight = _PointLightDataBuffer[lightIndex];
                //     float intensity = saturate(dot(normal, currentLight.position - i.positionWs) * currentLight.intensity);
                //     color += intensity * currentLight.color * albedo.rgb;
                // }

                //DEBUG END

                return float4(color, 1);
            }
            ENDCG
        }
    }
}