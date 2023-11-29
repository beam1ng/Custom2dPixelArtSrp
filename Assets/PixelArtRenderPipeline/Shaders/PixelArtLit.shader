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

            float4 _RendererBoundsCs2d; //min x, min y, max x, max y

            v2f vert(appdata v)
            {
                v2f o;

                float2 minBoundsCs = _RendererBoundsCs2d.xy;
                float2 maxBoundsCs = _RendererBoundsCs2d.zw;
                float2 boundsVertex = (1 - v.vertex) * minBoundsCs + v.vertex * maxBoundsCs;
                o.vertex = float4(boundsVertex.xy, 1, 1);
                o.uv = v.vertex;

                #if UNITY_UV_STARTS_AT_TOP
                o.vertex.y = -o.vertex.y;
                #endif
                return o;
            }

            void frag(v2f i, out float4 color : SV_Target0, out float4 normal : SV_Target1, out float depth : SV_Depth)
            {
                color = tex2D(_Albedo_proxy, i.uv);
                normal = tex2D(_Normal_proxy, i.uv);
                depth = tex2D(_Normal_proxy, i.uv);

                // color = float4(1, 0, 0, 1);
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
                float2 screenUv : TexCoord2;
            };

            float4 _Color;
            sampler2D _Albedo;
            float4 _Albedo_ST;

            float3 _DirectionalLightDir;

            float4 _RendererBoundsCs2d; //min x, min y, max x, max y

            float4 RemapRendererToScreen(float4 vertexCs, float2 screenUv, float4 rendererBoundsCs)
            {
                float4 rendererBoundsNdc = rendererBoundsCs * 0.5 + 0.5;
                
                float2 newMin = rendererBoundsNdc.zw/(rendererBoundsNdc.zw-rendererBoundsNdc.xy);
                float2 newMax = (rendererBoundsNdc.zw-1)/(rendererBoundsNdc.zw-rendererBoundsNdc.xy);

                float2 newCs = screenUv * (newMax - newMin) + newMin;
                return float4(newCs.xy,vertexCs.zw);
            }

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                float4 rbcs2d = _RendererBoundsCs2d;

                // #if UNITY_UV_STARTS_AT_TOP
                // _RendererBoundsCs2d.yw = - _RendererBoundsCs2d.yw;
                // #endif
                
                
                float2 rendererUv = (o.vertex.xy - _RendererBoundsCs2d.xy)/(_RendererBoundsCs2d.zw - _RendererBoundsCs2d.xy);
                
                o.vertex = RemapRendererToScreen(o.vertex, rendererUv, _RendererBoundsCs2d);
                o.normalWs = mul(UNITY_MATRIX_M, float4(v.normal.xyz, 0));
                o.uv = TRANSFORM_TEX(v.uv, _Albedo);
                o.screenUv = rendererUv;
                return o;
            }

            void frag(v2f i, out float4 color : COLOR0, out float4 normal : COLOR1)
            {
                i.normalWs = normalize(i.normalWs);

                color = tex2D(_Albedo, i.uv);
                normal = float4(i.normalWs.xyz * 0.5 + 0.5, 1);

                color = float4(i.screenUv.xy,0,1);
            }
            ENDCG
        }
    }
}