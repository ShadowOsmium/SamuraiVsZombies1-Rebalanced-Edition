#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Mobile/Ethereal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
        _Alpha ("Texture Alpha", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Tags { "LIGHTMODE"="Vertex" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata_t
            {
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float3 vertex : POSITION;
            };
            struct v2f
            {
                float3 texcoord3 : TEXCOORD3;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            float4 _MainTex_ST;
            float4 _BumpMap_ST;
            float4 _glesTANGENT;
            float4 _glesMultiTexCoord0;
            float3 _glesNormal;
            float4 _glesVertex;
            float _RimPower;
            float4 _RimColor;
            sampler2D _MainTex;
            float4 _LightColor0;
            sampler2D _BumpMap;
            float _Alpha;
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.xyz = normalize (v.tangent.xyz);
                tmpvar_1.w = v.tangent.w;
                float3 tmpvar_2;
                tmpvar_2 = normalize (v.normal);
                float3 shlight;
                float4 tmpvar_3;
                float3 tmpvar_4;
                float3 tmpvar_5;
                tmpvar_3.xy = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                tmpvar_3.zw = ((v.texcoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
                float3x3 tmpvar_6;
                tmpvar_6[0] = unity_ObjectToWorld[0].xyz;
                tmpvar_6[1] = unity_ObjectToWorld[1].xyz;
                tmpvar_6[2] = unity_ObjectToWorld[2].xyz;
                float3x3 tmpvar_7;
                tmpvar_7[0] = tmpvar_1.xyz;
                tmpvar_7[1] = (cross (tmpvar_2, tmpvar_1.xyz) * v.tangent.w);
                tmpvar_7[2] = tmpvar_2;
                float3x3 tmpvar_8;
                tmpvar_8[0].x = tmpvar_7[0].x;
                tmpvar_8[0].y = tmpvar_7[1].x;
                tmpvar_8[0].z = tmpvar_7[2].x;
                tmpvar_8[1].x = tmpvar_7[0].y;
                tmpvar_8[1].y = tmpvar_7[1].y;
                tmpvar_8[1].z = tmpvar_7[2].y;
                tmpvar_8[2].x = tmpvar_7[0].z;
                tmpvar_8[2].y = tmpvar_7[1].z;
                tmpvar_8[2].z = tmpvar_7[2].z;
                float3 tmpvar_9;
                tmpvar_9 = mul(tmpvar_8, mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz);
                tmpvar_4 = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = _WorldSpaceCameraPos;
                float4 tmpvar_11;
                tmpvar_11.w = 1.0;
                tmpvar_11.xyz = mul(tmpvar_6, (tmpvar_2 * 1.0));
                float3 tmpvar_12;
                float4 normal;
                normal = tmpvar_11;
                float3 x3;
                float vC;
                float3 x2;
                float3 x1;
                float tmpvar_13;
                tmpvar_13 = dot (unity_SHAr, normal);
                x1.x = tmpvar_13;
                float tmpvar_14;
                tmpvar_14 = dot (unity_SHAg, normal);
                x1.y = tmpvar_14;
                float tmpvar_15;
                tmpvar_15 = dot (unity_SHAb, normal);
                x1.z = tmpvar_15;
                float4 tmpvar_16;
                tmpvar_16 = (normal.xyzz * normal.yzzx);
                float tmpvar_17;
                tmpvar_17 = dot (unity_SHBr, tmpvar_16);
                x2.x = tmpvar_17;
                float tmpvar_18;
                tmpvar_18 = dot (unity_SHBg, tmpvar_16);
                x2.y = tmpvar_18;
                float tmpvar_19;
                tmpvar_19 = dot (unity_SHBb, tmpvar_16);
                x2.z = tmpvar_19;
                float tmpvar_20;
                tmpvar_20 = ((normal.x * normal.x) - (normal.y * normal.y));
                vC = tmpvar_20;
                float3 tmpvar_21;
                tmpvar_21 = (unity_SHC.xyz * vC);
                x3 = tmpvar_21;
                tmpvar_12 = ((x1 + x2) + x3);
                shlight = tmpvar_12;
                tmpvar_5 = shlight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_3;
                o.texcoord1 = mul(tmpvar_8, ((mul(unity_WorldToObject, tmpvar_10).xyz * 1.0) - v.vertex.xyz));
                o.texcoord2 = tmpvar_4;
                o.texcoord3 = tmpvar_5;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c;
                float2 tmpvar_1;
                tmpvar_1 = i.texcoord0.xy;
                float2 tmpvar_2;
                tmpvar_2 = i.texcoord0.zw;
                float3 tmpvar_3;
                float tmpvar_4;
                float rim;
                float4 tmpvar_5;
                tmpvar_5 = tex2D (_MainTex, tmpvar_1);
                float3 tmpvar_6;
                tmpvar_6 = ((tex2D (_BumpMap, tmpvar_2).xyz * 2.0) - 1.0);
                float tmpvar_7;
                tmpvar_7 = (1.0 - clamp (dot (normalize (i.texcoord1), tmpvar_6), 0.0, 1.0));
                rim = tmpvar_7;
                float3 tmpvar_8;
                tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
                tmpvar_3 = tmpvar_8;
                float tmpvar_9;
                tmpvar_9 = (_Alpha + pow (rim, _RimPower));
                tmpvar_4 = tmpvar_9;
                float4 c_i0;
                c_i0.xyz = ((tmpvar_5.xyz * _LightColor0.xyz) * (max (0.0, dot (tmpvar_6, i.texcoord2)) * 2.0));
                c_i0.w = tmpvar_4;
                c = c_i0;
                c.xyz = (c_i0.xyz + (tmpvar_5.xyz * i.texcoord3));
                c.xyz = (c.xyz + tmpvar_3);
                return float4(tmpvar_5 + tmpvar_3, tmpvar_4);
            }
            ENDCG
        }
    }
}
