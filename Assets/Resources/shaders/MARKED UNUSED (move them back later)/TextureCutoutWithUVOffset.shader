Shader "Griptonite/TextureCutoffWithUVOffset"
{
    Properties
    {
        _Intensity ("Intensity", Float) = 0.1
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OffsetTex ("Offset (IA)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,0.9)) = 0.5
    }
    SubShader
    {
        Tags { "QUEUE"="AlphaTest" "RenderType"="TransparentCutout" }
        Pass
        {
            Tags { "QUEUE"="AlphaTest" "RenderType"="TransparentCutout" }
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float3 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            float4 _OffsetTex_ST;
            float4 _MainTex_ST;
            float4 _glesMultiTexCoord1;
            float4 _glesMultiTexCoord0;
            float4 _glesVertex;
            sampler2D _OffsetTex;
            sampler2D _MainTex;
            float _Intensity;
            float _Cutoff;
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord1 = ((v.texcoord1.xy * _OffsetTex_ST.xy) + _OffsetTex_ST.zw);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c;
                float2 texOffset;
                float4 tmpvar_1;
                tmpvar_1 = tex2D (_OffsetTex, i.texcoord1);
                float2 tmpvar_2;
                tmpvar_2 = (tmpvar_1.wz * _Intensity);
                texOffset = tmpvar_2;
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, (i.texcoord0 + texOffset));
                c = tmpvar_3;
                float x;
                x = (c.w - _Cutoff);
                discard;
                return c;
            }
            ENDCG
        }
    }
}
