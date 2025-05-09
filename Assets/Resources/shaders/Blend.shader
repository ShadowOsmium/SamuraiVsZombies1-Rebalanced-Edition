Shader "AlphaBlend"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _TintColor ("TintColor", Color) = (1,1,1,0.2)
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="RenderReflectionTransparentBlend" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="RenderReflectionTransparentBlend" }
            ZWrite Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float3 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            float4 _glesMultiTexCoord0;
            float4 _glesVertex;
            float4 _TintColor;
            sampler2D _MainTex;
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                float2 tmpvar_2;
                float4 tmpvar_3;
                tmpvar_3 = UnityObjectToClipPos(v.vertex);
                tmpvar_1 = tmpvar_3;
                float2 tmpvar_4;
                tmpvar_4 = v.texcoord0.xy;
                tmpvar_2 = tmpvar_4;
                o.vertex = tmpvar_1;
                o.texcoord0 = tmpvar_2;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                return (tex2D (_MainTex, i.texcoord0) * _TintColor);
            }
            ENDCG
        }
    }
}
