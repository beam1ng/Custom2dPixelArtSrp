Shader "PixelArtRp/DeferredLighting"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    
    SubShader
    {
//        Tags
//        {
//            "LightMode"="PixelArtRp"
//        }
//        
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
            
            struct appdata
            {
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex; //Albedo
            // sampler2D _Metallic;
            // sampler2D _Smoothness;
            // sampler2D _Normal;
            float4 _MainTex_ST;
            // float4 _Metallic_ST;
            // float4 _Smoothness_ST;
            // float4 _Normal_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i):SV_Target
            {
                float4 color = tex2D(_MainTex,i.uv);
                return float4(color.rgb,1);
            }
            ENDCG
        }
    }
}