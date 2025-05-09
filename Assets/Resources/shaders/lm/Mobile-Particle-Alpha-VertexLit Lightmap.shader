Shader "Mobile/Particles/Alpha Blended_LM"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _LightMap ("Light Map", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float4 color : COLOR;
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct v2f
            {
                float4 color : COLOR;
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            float4 _MainTex_ST, _LightMap_ST;
            sampler2D _MainTex, _LightMap;
            v2f vert(appdata_t v)
            {
                v2f o;
                o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.texcoord0 * _MainTex_ST.xy + _MainTex_ST.zw;
                o.texcoord1 = v.texcoord1 * _LightMap_ST.xy + _LightMap_ST.zw;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                return tex2D(_MainTex, i.texcoord0) * i.color * (tex2D(_LightMap, i.texcoord1) * 2.0);
            }
            ENDCG
        }
    }
}