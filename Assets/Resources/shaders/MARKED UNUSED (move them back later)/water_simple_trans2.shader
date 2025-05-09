#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "FX/Water (trans)"
{
    Properties
    {
        _horizonColor ("Horizon color", Color) = (0.172,0.463,0.435,0)
        _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.07
        _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
        _ColorControlCube ("Reflective color cube (RGB) fresnel (A) ", CUBE) = "" { TexGen CubeReflect }
        _BumpMap ("Waves Normalmap ", 2D) = "" {}
        WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
        _MainTex ("Fallback texture", 2D) = "" {}
        _Opacity ("Opacity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float3 vertex : POSITION;
            };
            struct v2f
            {
                float3 texcoord2 : TEXCOORD2;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            float _WaveScale;
            float4 _WaveOffset;
            float4 _glesVertex;
            float4 _horizonColor;
            float _Opacity;
            sampler2D _ColorControl;
            sampler2D _BumpMap;
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 temp;
                float2 tmpvar_1[2];
                temp = (((v.vertex.xzxz * _WaveScale) / 1.0) + _WaveOffset);
                tmpvar_1[0] = (temp.xy * float2(0.4, 0.45));
                tmpvar_1[1] = temp.wz;
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = _WorldSpaceCameraPos;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_1[0];
                o.texcoord1 = tmpvar_1[1];
                o.texcoord2 = normalize (((mul(unity_WorldToObject, tmpvar_2).xyz * 1.0) - v.vertex.xyz)).xzy;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float2 tmpvar_1[2];
                tmpvar_1[0] = i.texcoord0;
                tmpvar_1[1] = i.texcoord1;
                float4 col;
                float4 water;
                float fresnel;
                float3 bump2;
                float3 bump1;
                float3 tmpvar_2;
                tmpvar_2 = ((tex2D (_BumpMap, tmpvar_1[0]).xyz * 2.0) - 1.0);
                bump1 = tmpvar_2;
                float3 tmpvar_3;
                tmpvar_3 = ((tex2D (_BumpMap, tmpvar_1[1]).xyz * 2.0) - 1.0);
                bump2 = tmpvar_3;
                float3 tmpvar_4;
                tmpvar_4 = ((bump1 + bump2) * 0.5);
                float tmpvar_5;
                tmpvar_5 = dot (i.texcoord2, tmpvar_4);
                fresnel = tmpvar_5;
                float2 tmpvar_6;
                tmpvar_6.x = fresnel;
                tmpvar_6.y = fresnel;
                float4 tmpvar_7;
                tmpvar_7 = tex2D (_ColorControl, tmpvar_6);
                water = tmpvar_7;
                float3 tmpvar_8;
                tmpvar_8 = water.www;
                float3 tmpvar_9;
                tmpvar_9 = lerp (water.xyz, _horizonColor.xyz, tmpvar_8);
                col.xyz = tmpvar_9;
                float tmpvar_10;
                tmpvar_10 = (_horizonColor.w * _Opacity);
                col.w = tmpvar_10;
                return col;
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            Color (0.5,0.5,0.5,0.5)
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { Matrix [_WaveMatrix] combine texture * primary }
            SetTexture [_MainTex] { Matrix [_WaveMatrix2] combine texture * primary + previous }
            SetTexture [_ColorControlCube] { Matrix [_Reflection] combine texture +- previous, primary alpha }
        }
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            Color (0.5,0.5,0.5,0.5)
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { Matrix [_WaveMatrix] combine texture }
            SetTexture [_ColorControlCube] { Matrix [_Reflection] combine texture +- previous, primary alpha }
        }
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            Color (0.5,0.5,0.5,0)
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { Matrix [_WaveMatrix] combine texture, primary alpha }
        }
    }
}
