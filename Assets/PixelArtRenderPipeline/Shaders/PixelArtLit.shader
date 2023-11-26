// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PixelArtRp/PixelArtLit"
{
    Properties
    {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Albedo ("Albedo", 2D) = ""
    }

    SubShader
    {
        Tags
        {
            "LightMode"="PixelArtDeferred"
        }

        Pass
        {
            Name "FinalPass"
            Blend Off
            Cull Off
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include <UnityStandardUtils.cginc>

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            sampler2D _Albedo_proxy;
            sampler2D _Normal_proxy;
            sampler2D _Depth_proxy;
            float4 _Albedo_ST;
            float4 _Normal_ST;
            float4 _Depth_ST;

            float3 _DirectionalLightDir;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.xy * 2 - 1,1,1);
                o.uv = v.vertex;
                #if UNITY_UV_STARTS_AT_TOP
                o.uv.y = 1 - o.uv.y;
                #endif
                return o;
            }

            void frag(v2f i, out float4 color : COLOR0, out float4 normal : COLOR1)
            {
                color = tex2D(_Albedo_proxy, i.uv);
                normal = tex2D(_Normal_proxy, i.uv);
            }
            ENDCG
        }

        Pass
        {
            Name "DownscalePass"
            Blend Off
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include <UnityStandardUtils.cginc>

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 normalWs : TEXCOORD1;
            };

            float4 _Color;
            sampler2D _Albedo;
            float4 _Albedo_ST;

            float3 _DirectionalLightDir;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normalWs = mul(UNITY_MATRIX_M, float4(v.normal.xyz, 0));
                o.uv = TRANSFORM_TEX(v.uv, _Albedo);
                return o;
            }

            void frag(v2f i, out float4 color : COLOR0, out float4 normal : COLOR1)
            {
                i.normalWs = normalize(i.normalWs);

                color = tex2D(_Albedo, i.uv);
                normal = float4(i.normalWs.xyz * 0.5 + 0.5, 1);
            }
            ENDCG
        }
    }
}