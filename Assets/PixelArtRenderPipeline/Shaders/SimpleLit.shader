Shader "PixelArtRp/SimpleLit"
{
    Properties
    {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Albedo ("Albedo", 2D) = ""
        _Metallic ("Metallic", 2D) = ""
        _Smoothness ("Smoothness", 2D) = ""
        _Normal ("Normal map", 2D) = ""
    }

    SubShader
    {
        Tags
        {
            "LightMode"="PixelArtDeferred"
        }

        Pass
        {
            Blend Off
            Cull Back
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float4 vertexWs : TEXCOORD1;
                float4 normalWs : TEXCOORD2;
            };

            float4 _Color;
            sampler2D _Albedo;
            sampler2D _Metallic;
            sampler2D _Smoothness;
            sampler2D _Normal;
            float4 _Albedo_ST;
            float4 _Metallic_ST;
            float4 _Smoothness_ST;
            float4 _Normal_ST;

            float3 _DirectionalLightDir;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Albedo);
                o.vertexWs = mul(UNITY_MATRIX_M, v.vertex);
                o.normalWs = mul(UNITY_MATRIX_M, v.normal);
                return o;
            }

            void frag(v2f i, out float4 color : COLOR0, out float4 normal : COLOR1)
            {
                color = tex2D(_Albedo, i.uv);
                color = float4(i.uv,0,1);
                normal = float4(saturate(i.normalWs.xyz * 0.5 + float3(0.5, 0.5, 0.5)), 1);
            }
            ENDCG
        }
    }
}