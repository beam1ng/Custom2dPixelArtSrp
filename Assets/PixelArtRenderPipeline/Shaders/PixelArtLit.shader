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
            Name "BlitToGBufferPass"
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
                float2 debug : TEXCOORD1;
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

                o.debug = v.vertex.xy;
                return o;
            }

            void frag(v2f i, out float4 color : SV_Target0, out float4 normal : SV_Target1, out float depth : SV_Depth)
            {
                color = tex2D(_Albedo_proxy, i.uv);
                normal = tex2D(_Normal_proxy, i.uv);
                depth = tex2D(_Normal_proxy, i.uv);

                // color = float4(1, 0, 0, 1);
                // color = float4(i.debug.xy%1,0,1);
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

            float4 RemapPos(float4 position,float2 inMin, float2 inMax, float2 outMin, float2 outMax)
            {
                float2 progress = (position - inMin)/(inMax - inMin);
                float2 newPosition = outMin + progress * (outMax - outMin);
                return float4(newPosition.xy,position.zw);
            }

            v2f vert(appdata v)
            {
                v2f o;

                //y is flipped (needed for rendering, else it will be backfaced)
                o.vertex = UnityObjectToClipPos(v.vertex);

                float4 vertexCs = o.vertex;

                //y is not flipped
                #if UNITY_UV_STARTS_AT_TOP
                vertexCs.y*=-1;
                #endif

                //xy range from 0.0 (left down) to 1.0 (right up) 
                float2 rendererUv = (vertexCs - _RendererBoundsCs2d.xy)/(_RendererBoundsCs2d.zw - _RendererBoundsCs2d.xy);

                float2 screenUv = (_RendererBoundsCs2d.xy + (_RendererBoundsCs2d.zw - _RendererBoundsCs2d.xy) * rendererUv) * 0.5 + 0.5;

                o.vertex = RemapPos(vertexCs,_RendererBoundsCs2d.xy,_RendererBoundsCs2d.zw,float2(-1,-1),float2(1,1));

                //y needs to be flipped for rendering
                #if UNITY_UV_STARTS_AT_TOP
                o.vertex.y *= -1;
                #endif
                
                o.screenUv = screenUv;
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