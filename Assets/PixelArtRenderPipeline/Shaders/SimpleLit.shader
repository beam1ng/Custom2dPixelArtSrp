Shader "PixelArtRp/SimpleLit"
{
    Properties
    {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags
        {
            "LightMode"="PixelArtDeferred"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 vertexWs : TEXCOORD1;
                float4 normalWs : TEXCOORD2;
            };

            float3 _DirectionalLightDir;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexWs = mul(UNITY_MATRIX_M, v.vertex);
                o.normalWs = mul(UNITY_MATRIX_M, v.normal);
                return o;
            }

            void frag(v2f i, out float4 color : COLOR0, out float4 normal : COLOR1)
            {
                color = _Color;
                normal = float4(saturate(i.normalWs.xyz * 0.5 + float3(0.5, 0.5, 0.5)), 1);
            }
            ENDCG
        }
    }
}