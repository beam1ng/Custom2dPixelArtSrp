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
            ZWrite Off
            ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityShaderUtilities.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityImageBasedLighting.cginc"
            #include "HLSLSupport.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityGBuffer.cginc"
            #include "UnityGlobalIllumination.cginc"
            
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

            float4 frag(v2f i):SV_Target
            {
                float4 albedo = tex2D(_Albedo,i.uv);
                float4 normal = tex2D(_Normal,i.uv) * 2 - 1;
                
                float intensity = saturate(saturate(dot(normal,-_DirectionalLightDir)) + 0.1);
                float3 color = intensity * albedo.rgb;
                
                return float4(color,1);
            }
            
            ENDCG
        }
    }
}