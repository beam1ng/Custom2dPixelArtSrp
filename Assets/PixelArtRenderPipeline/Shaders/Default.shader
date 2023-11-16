Shader "PixelArtRp/Default"
{
    Properties
    {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
    }
    
    SubShader
    {
        Tags
        {
            "LightMode"="PixelArtRp"
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = _Color;
                return col;
            }
            ENDCG
        }
    }
}