#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Mobile/OpaqueEthereal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
    }
    SubShader
    {
        Tags { "LIGHTMODE"="Vertex" }
        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float3 vertex : POSITION;
            };
            struct v2f
            {
                float3 texcoord3 : TEXCOORD3;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            float4 _MainTex_ST;
            float4 _glesMultiTexCoord0;
            float3 _glesNormal;
            float4 _glesVertex;
            float _RimPower;
            float4 _RimColor;
            sampler2D _MainTex;
            float4 _LightColor0;
            v2f vert(appdata_t v)
            {
                v2f o;
                float3 shlight;
                float3 tmpvar_1;
                float3 tmpvar_2;
                float3x3 tmpvar_3;
                tmpvar_3[0] = unity_ObjectToWorld[0].xyz;
                tmpvar_3[1] = unity_ObjectToWorld[1].xyz;
                tmpvar_3[2] = unity_ObjectToWorld[2].xyz;
                float3 tmpvar_4;
                tmpvar_4 = mul(tmpvar_3, (normalize (v.normal) * 1.0));
                tmpvar_1 = tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5.w = 1.0;
                tmpvar_5.xyz = tmpvar_4;
                float3 tmpvar_6;
                float4 normal;
                normal = tmpvar_5;
                float3 x3;
                float vC;
                float3 x2;
                float3 x1;
                float tmpvar_7;
                tmpvar_7 = dot (unity_SHAr, normal);
                x1.x = tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = dot (unity_SHAg, normal);
                x1.y = tmpvar_8;
                float tmpvar_9;
                tmpvar_9 = dot (unity_SHAb, normal);
                x1.z = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10 = (normal.xyzz * normal.yzzx);
                float tmpvar_11;
                tmpvar_11 = dot (unity_SHBr, tmpvar_10);
                x2.x = tmpvar_11;
                float tmpvar_12;
                tmpvar_12 = dot (unity_SHBg, tmpvar_10);
                x2.y = tmpvar_12;
                float tmpvar_13;
                tmpvar_13 = dot (unity_SHBb, tmpvar_10);
                x2.z = tmpvar_13;
                float tmpvar_14;
                tmpvar_14 = ((normal.x * normal.x) - (normal.y * normal.y));
                vC = tmpvar_14;
                float3 tmpvar_15;
                tmpvar_15 = (unity_SHC.xyz * vC);
                x3 = tmpvar_15;
                tmpvar_6 = ((x1 + x2) + x3);
                shlight = tmpvar_6;
                tmpvar_2 = shlight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord1 = (_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.texcoord2 = tmpvar_1;
                o.texcoord3 = tmpvar_2;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c;
                float3 tmpvar_1;
                float rim;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                float tmpvar_3;
                tmpvar_3 = (1.0 - clamp (dot (normalize (i.texcoord1), i.texcoord2), 0.0, 1.0));
                rim = tmpvar_3;
                float3 tmpvar_4;
                tmpvar_4 = (_RimColor.xyz * pow (rim, _RimPower));
                tmpvar_1 = tmpvar_4;
                float4 c_i0;
                c_i0.xyz = ((tmpvar_2.xyz * _LightColor0.xyz) * (max (0.0, dot (i.texcoord2, _WorldSpaceLightPos0.xyz)) * 2.0));
                c_i0.w = 0.0;
                c = c_i0;
                c.xyz = (c_i0.xyz + (tmpvar_2.xyz * i.texcoord3));
                c.xyz = (c.xyz + tmpvar_1);
                return float4(tmpvar_2 + tmpvar_1, 1);
            }
            ENDCG
        }
    }
}
