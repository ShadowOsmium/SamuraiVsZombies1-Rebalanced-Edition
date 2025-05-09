Shader "GripUnlit/UnlitWithColorIntensity"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ColorIntensity ("Color Intensity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "RenderType"="Opaque" }
            Fog { Mode Off }
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
            float4 _MainTex_ST;
            float4 _glesMultiTexCoord0;
            float4 _glesVertex;
            sampler2D _MainTex;
            float4 _MainColor;
            float _ColorIntensity;
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 t2;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                t2 = tmpvar_2;
                tmpvar_1 = ((t2 * (1.0 - _ColorIntensity)) + (_MainColor * _ColorIntensity));
                return tmpvar_1;
            }
            ENDCG
        }
    }
}
