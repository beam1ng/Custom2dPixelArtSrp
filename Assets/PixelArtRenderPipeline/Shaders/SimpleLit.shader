Shader "PixelArtRp/SimpleLit"
{
    Properties
    {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Albedo ("Albedo", 2D) = ""
//        _Metallic ("Metallic", 2D) = ""
//        _Smoothness ("Smoothness", 2D) = ""
//        _Normal ("Normal map", 2D) = ""
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

            #include <UnityStandardUtils.cginc>

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                // float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 normalWs : TEXCOORD1;
                // float4 tangentWs : TEXCOORD3;
            };

            int _PixelSize;
            
            float4 _Color;
            sampler2D _Albedo;
            // sampler2D _Normal;
            float4 _Albedo_ST;
            // float4 _Normal_ST;

            float3 _DirectionalLightDir;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normalWs = mul(UNITY_MATRIX_M, float4(v.normal.xyz,0));
                o.uv = TRANSFORM_TEX(v.uv, _Albedo);
                // o.tangentWs = mul(UNITY_MATRIX_M, v.tangent);
                return o;
            }

            void frag(v2f i, out float4 color : COLOR0, out float4 normal : COLOR1)
            {
                i.normalWs = normalize(i.normalWs);
                // i.tangentWs = normalize(i.tangentWs);
                
                color = tex2D(_Albedo, i.uv);
                normal = float4(i.normalWs.xyz * 0.5 + 0.5,1);
                
                // normal = tex2D(_Normal, i.uv);
                // normal = float4(UnpackNormalmapRGorAG(normal).xyz,0);
                //todo: use uv data to establish tangentWs
                
                // float3x3 tangentToWorld = CreateTangentToWorldPerVertex(i.normalWs,i.tangentWs,1);
                // normal.xyz = normalize(mul(tangentToWorld,normal.xyz)) * 0.5 + 0.5;

                // normal = float4(tangentWs.xyz,0);
            }

            ENDCG
        }
    }
}