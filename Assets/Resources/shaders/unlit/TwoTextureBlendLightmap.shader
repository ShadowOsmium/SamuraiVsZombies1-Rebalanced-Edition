Shader "GripUnlit/TwoTextureBlend (Supports Lightmap)"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SecondTex ("Second (RGB)", 2D) = "white" {}
    }
    SubShader
    { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="Vertex" "RenderType"="Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MainTex;
            sampler2D _SecondTex;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.texcoord;
                return o;
            }
            float4 frag(v2f i) : SV_Target
            {
                return lerp(tex2D(_MainTex, i.uv), tex2D(_SecondTex, i.uv), 1 - i.color.a) * i.color;
            }
            ENDCG
        }
    }
}