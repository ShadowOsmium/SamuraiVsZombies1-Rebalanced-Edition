Shader "Griptonite/TwoDiffuseBlendUnlitTwoUVs"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _SecondColor ("Second Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SecondTex ("Second (RGB)", 2D) = "white" {}
        _ColorIntensity ("Color Intensity", Range(0,4)) = 1
    }
    SubShader
    {
        Pass
        {
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float3 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            float4 _SecondTex_ST;
            float4 _MainTex_ST;
            float4 _glesMultiTexCoord1;
            float4 _glesMultiTexCoord0;
            float4 _glesColor;
            float4 _glesVertex;
            sampler2D _SecondTex;
            float4 _SecondColor;
            sampler2D _MainTex;
            float4 _MainColor;
            float _ColorIntensity;
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord1 = ((v.texcoord1.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c;
                float4 c2;
                float4 c1;
                float4 tmpvar_1;
                tmpvar_1 = tex2D (_MainTex, i.texcoord0);
                float4 tmpvar_2;
                tmpvar_2 = (tmpvar_1 * _MainColor);
                c1 = tmpvar_2;
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_SecondTex, i.texcoord1);
                float4 tmpvar_4;
                tmpvar_4 = (tmpvar_3 * _SecondColor);
                c2 = tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5 = ((c1 * i.color.w) + (c2 * (1.0 - i.color.w)));
                c = tmpvar_5;
                float3 tmpvar_6;
                tmpvar_6 = (tmpvar_5.xyz * (i.color.xyz * _ColorIntensity));
                c.xyz = tmpvar_6;
                return c;
            }
            ENDCG
        }
    }
}
