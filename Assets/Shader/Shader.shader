ñœShader "Decal" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _DecalTex ("Decal (RGBA)", 2D) = "black" {}
}
SubShader { 
 LOD 250
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = tmpvar_5;
  mediump vec3 tmpvar_7;
  mediump vec4 normal;
  normal = tmpvar_6;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_8;
  tmpvar_8 = dot (unity_SHAr, normal);
  x1.x = tmpvar_8;
  highp float tmpvar_9;
  tmpvar_9 = dot (unity_SHAg, normal);
  x1.y = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = dot (unity_SHAb, normal);
  x1.z = tmpvar_10;
  mediump vec4 tmpvar_11;
  tmpvar_11 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHBr, tmpvar_11);
  x2.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHBg, tmpvar_11);
  x2.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHBb, tmpvar_11);
  x2.z = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_15;
  highp vec3 tmpvar_16;
  tmpvar_16 = (unity_SHC.xyz * vC);
  x3 = tmpvar_16;
  tmpvar_7 = ((x1 + x2) + x3);
  shlight = tmpvar_7;
  tmpvar_3 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_6.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_6.xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz));
  c.w = tmpvar_6.w;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = tmpvar_6;
  mediump vec3 tmpvar_8;
  mediump vec4 normal;
  normal = tmpvar_7;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_9;
  tmpvar_9 = dot (unity_SHAr, normal);
  x1.x = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = dot (unity_SHAg, normal);
  x1.y = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = dot (unity_SHAb, normal);
  x1.z = tmpvar_11;
  mediump vec4 tmpvar_12;
  tmpvar_12 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHBr, tmpvar_12);
  x2.x = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHBg, tmpvar_12);
  x2.y = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHBb, tmpvar_12);
  x2.z = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (unity_SHC.xyz * vC);
  x3 = tmpvar_17;
  tmpvar_8 = ((x1 + x2) + x3);
  shlight = tmpvar_8;
  tmpvar_3 = shlight;
  highp vec4 o_i0;
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_4 * 0.5);
  o_i0 = tmpvar_18;
  highp vec2 tmpvar_19;
  tmpvar_19.x = tmpvar_18.x;
  tmpvar_19.y = (tmpvar_18.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_19 + tmpvar_18.w);
  o_i0.zw = tmpvar_4.zw;
  gl_Position = tmpvar_4;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * texture2DProj (_ShadowMapTexture, xlv_TEXCOORD3).x) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_6.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform highp vec4 _ProjectionParams;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  highp vec4 o_i0;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * 0.5);
  o_i0 = tmpvar_3;
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_4 + tmpvar_3.w);
  o_i0.zw = tmpvar_2.zw;
  gl_Position = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD2 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_6.xyz * min ((2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz), vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD2).x * 2.0))));
  c.w = tmpvar_6.w;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = tmpvar_5;
  mediump vec3 tmpvar_7;
  mediump vec4 normal;
  normal = tmpvar_6;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_8;
  tmpvar_8 = dot (unity_SHAr, normal);
  x1.x = tmpvar_8;
  highp float tmpvar_9;
  tmpvar_9 = dot (unity_SHAg, normal);
  x1.y = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = dot (unity_SHAb, normal);
  x1.z = tmpvar_10;
  mediump vec4 tmpvar_11;
  tmpvar_11 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHBr, tmpvar_11);
  x2.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHBg, tmpvar_11);
  x2.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHBb, tmpvar_11);
  x2.z = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_15;
  highp vec3 tmpvar_16;
  tmpvar_16 = (unity_SHC.xyz * vC);
  x3 = tmpvar_16;
  tmpvar_7 = ((x1 + x2) + x3);
  shlight = tmpvar_7;
  tmpvar_3 = shlight;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_18;
  tmpvar_18 = (unity_4LightPosX0 - tmpvar_17.x);
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_4LightPosY0 - tmpvar_17.y);
  highp vec4 tmpvar_20;
  tmpvar_20 = (unity_4LightPosZ0 - tmpvar_17.z);
  highp vec4 tmpvar_21;
  tmpvar_21 = (((tmpvar_18 * tmpvar_18) + (tmpvar_19 * tmpvar_19)) + (tmpvar_20 * tmpvar_20));
  highp vec4 tmpvar_22;
  tmpvar_22 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_18 * tmpvar_5.x) + (tmpvar_19 * tmpvar_5.y)) + (tmpvar_20 * tmpvar_5.z)) * inversesqrt (tmpvar_21))) * 1.0/((1.0 + (tmpvar_21 * unity_4LightAtten0))));
  highp vec3 tmpvar_23;
  tmpvar_23 = (tmpvar_3 + ((((unity_LightColor[0].xyz * tmpvar_22.x) + (unity_LightColor[1].xyz * tmpvar_22.y)) + (unity_LightColor[2].xyz * tmpvar_22.z)) + (unity_LightColor[3].xyz * tmpvar_22.w)));
  tmpvar_3 = tmpvar_23;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_6.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = tmpvar_6;
  mediump vec3 tmpvar_8;
  mediump vec4 normal;
  normal = tmpvar_7;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_9;
  tmpvar_9 = dot (unity_SHAr, normal);
  x1.x = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = dot (unity_SHAg, normal);
  x1.y = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = dot (unity_SHAb, normal);
  x1.z = tmpvar_11;
  mediump vec4 tmpvar_12;
  tmpvar_12 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHBr, tmpvar_12);
  x2.x = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHBg, tmpvar_12);
  x2.y = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHBb, tmpvar_12);
  x2.z = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (unity_SHC.xyz * vC);
  x3 = tmpvar_17;
  tmpvar_8 = ((x1 + x2) + x3);
  shlight = tmpvar_8;
  tmpvar_3 = shlight;
  highp vec3 tmpvar_18;
  tmpvar_18 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_4LightPosX0 - tmpvar_18.x);
  highp vec4 tmpvar_20;
  tmpvar_20 = (unity_4LightPosY0 - tmpvar_18.y);
  highp vec4 tmpvar_21;
  tmpvar_21 = (unity_4LightPosZ0 - tmpvar_18.z);
  highp vec4 tmpvar_22;
  tmpvar_22 = (((tmpvar_19 * tmpvar_19) + (tmpvar_20 * tmpvar_20)) + (tmpvar_21 * tmpvar_21));
  highp vec4 tmpvar_23;
  tmpvar_23 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_19 * tmpvar_6.x) + (tmpvar_20 * tmpvar_6.y)) + (tmpvar_21 * tmpvar_6.z)) * inversesqrt (tmpvar_22))) * 1.0/((1.0 + (tmpvar_22 * unity_4LightAtten0))));
  highp vec3 tmpvar_24;
  tmpvar_24 = (tmpvar_3 + ((((unity_LightColor[0].xyz * tmpvar_23.x) + (unity_LightColor[1].xyz * tmpvar_23.y)) + (unity_LightColor[2].xyz * tmpvar_23.z)) + (unity_LightColor[3].xyz * tmpvar_23.w)));
  tmpvar_3 = tmpvar_24;
  highp vec4 o_i0;
  highp vec4 tmpvar_25;
  tmpvar_25 = (tmpvar_4 * 0.5);
  o_i0 = tmpvar_25;
  highp vec2 tmpvar_26;
  tmpvar_26.x = tmpvar_25.x;
  tmpvar_26.y = (tmpvar_25.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_26 + tmpvar_25.w);
  o_i0.zw = tmpvar_4.zw;
  gl_Position = tmpvar_4;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * texture2DProj (_ShadowMapTexture, xlv_TEXCOORD3).x) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_6.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
"!!GLES"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_3 = tmpvar_6;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = normalize (xlv_TEXCOORD2);
  lightDir = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = vec2(dot (xlv_TEXCOORD3, xlv_TEXCOORD3));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * texture2D (_LightTexture0, tmpvar_8).w) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_6 = _WorldSpaceLightPos0.xyz;
  tmpvar_3 = tmpvar_6;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  lightDir = xlv_TEXCOORD2;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD1, lightDir)) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_3 = tmpvar_6;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = normalize (xlv_TEXCOORD2);
  lightDir = tmpvar_7;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD3.xyz;
  highp vec2 tmpvar_8;
  tmpvar_8 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD3.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD3.xy / xlv_TEXCOORD3.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_8).w);
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * atten) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_3 = tmpvar_6;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = normalize (xlv_TEXCOORD2);
  lightDir = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = vec2(dot (xlv_TEXCOORD3, xlv_TEXCOORD3));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * (texture2D (_LightTextureB0, tmpvar_8).w * textureCube (_LightTexture0, xlv_TEXCOORD3).w)) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _DecalTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _DecalTex_ST.xy) + _DecalTex_ST.zw);
  mat3 tmpvar_4;
  tmpvar_4[0] = _Object2World[0].xyz;
  tmpvar_4[1] = _Object2World[1].xyz;
  tmpvar_4[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_2 = tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_6 = _WorldSpaceLightPos0.xyz;
  tmpvar_3 = tmpvar_6;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform sampler2D _DecalTex;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.zw;
  mediump vec4 decal;
  lowp vec4 c_i0;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, tmpvar_1);
  c_i0 = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_DecalTex, tmpvar_2);
  decal = tmpvar_4;
  mediump vec3 tmpvar_5;
  tmpvar_5 = mix (tmpvar_3.xyz, decal.xyz, decal.www);
  c_i0.xyz = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = (c_i0 * _Color);
  c_i0 = tmpvar_6;
  lightDir = xlv_TEXCOORD2;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_6.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD3).w) * 2.0));
  c_i0_i1.w = tmpvar_6.w;
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}
}
 }
}
Fallback "Diffuse"
}