Shader "Billboard/Displacement Map Shader"
{
    Properties
    {
        _Background ("Background", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _BumpAmt ("Distortion", Range(0,0.1)) = 0.02
        _BlendAmt ("Blend Amount", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "IGNOREPROJECTOR"="True" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" }
            ZWrite Off
            Cull Off
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
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            float4 _glesMultiTexCoord0;
            float4 _glesVertex;
            sampler2D _BumpMap;
            float _BumpAmt;
            float _BlendAmt;
            sampler2D _Background;
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float4 pos;
                float4 tmpvar_2;
                tmpvar_2 = UnityObjectToClipPos(v.vertex);
                pos = tmpvar_2;
                float2 tmpvar_3;
                float4 tmpvar_4;
                tmpvar_4.zw = float2(0.0, 0.0);
                tmpvar_4.x = v.texcoord0.x;
                tmpvar_4.y = v.texcoord0.y;
                tmpvar_3 = mul(UNITY_MATRIX_TEXTURE0, tmpvar_4).xy;
                tmpvar_1 = tmpvar_3;
                o.vertex = pos;
                o.texcoord0 = (((pos.xy / pos.w) * 0.5) + 0.5);
                o.texcoord1 = tmpvar_1;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1.xyz = tex2D (_Background, (i.texcoord0 + ((tex2D (_BumpMap, i.texcoord1).xy - 0.501961) * _BumpAmt))).xyz;
                tmpvar_1.w = _BlendAmt;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}
