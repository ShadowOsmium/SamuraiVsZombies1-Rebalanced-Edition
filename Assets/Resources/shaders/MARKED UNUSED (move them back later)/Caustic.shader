º”Shader "Griptonite/Caustic" {
Properties {
 _CausticIntensity ("Caustic Intensity", Float) = 0.1
 _MainColor ("Main Color", Color) = (1,1,1,1)
 _CausticColor ("Caustic Color", Color) = (1,1,1,1)
 _MainTex ("Main (RGB)", 2D) = "white" {}
 _BlendTex ("Blend (IA)", 2D) = "white" {}
 _C1Tex ("Caustic1 (RGB)", 2D) = "white" {}
 _C2Tex ("Caustic2 (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 200
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

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
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
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
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
  tmpvar_4 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_4 * xlv_TEXCOORD3));
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

varying highp vec2 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform highp vec4 _MainTex_ST;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_4 * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD2).xyz));
  c.w = tmpvar_5;
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

varying highp vec4 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
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
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = tmpvar_7;
  mediump vec3 tmpvar_9;
  mediump vec4 normal;
  normal = tmpvar_8;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_10;
  tmpvar_10 = dot (unity_SHAr, normal);
  x1.x = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = dot (unity_SHAg, normal);
  x1.y = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAb, normal);
  x1.z = tmpvar_12;
  mediump vec4 tmpvar_13;
  tmpvar_13 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHBr, tmpvar_13);
  x2.x = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHBg, tmpvar_13);
  x2.y = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBb, tmpvar_13);
  x2.z = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = (unity_SHC.xyz * vC);
  x3 = tmpvar_18;
  tmpvar_9 = ((x1 + x2) + x3);
  shlight = tmpvar_9;
  tmpvar_4 = shlight;
  highp vec4 o_i0;
  highp vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_5 * 0.5);
  o_i0 = tmpvar_19;
  highp vec2 tmpvar_20;
  tmpvar_20.x = tmpvar_19.x;
  tmpvar_20.y = (tmpvar_19.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_20 + tmpvar_19.w);
  o_i0.zw = tmpvar_5.zw;
  gl_Position = tmpvar_5;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * texture2DProj (_ShadowMapTexture, xlv_TEXCOORD4).x) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_4 * xlv_TEXCOORD3));
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

varying highp vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform highp vec4 _ProjectionParams;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  highp vec4 o_i0;
  highp vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * 0.5);
  o_i0 = tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = (tmpvar_4.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_5 + tmpvar_4.w);
  o_i0.zw = tmpvar_3.zw;
  gl_Position = tmpvar_3;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD3 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_4 * min ((2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD2).xyz), vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD3).x * 2.0))));
  c.w = tmpvar_5;
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

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
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
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
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
  tmpvar_4 = shlight;
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
  tmpvar_24 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_23.x) + (unity_LightColor[1].xyz * tmpvar_23.y)) + (unity_LightColor[2].xyz * tmpvar_23.z)) + (unity_LightColor[3].xyz * tmpvar_23.w)));
  tmpvar_4 = tmpvar_24;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_4 * xlv_TEXCOORD3));
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

varying highp vec4 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
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
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = tmpvar_7;
  mediump vec3 tmpvar_9;
  mediump vec4 normal;
  normal = tmpvar_8;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_10;
  tmpvar_10 = dot (unity_SHAr, normal);
  x1.x = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = dot (unity_SHAg, normal);
  x1.y = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAb, normal);
  x1.z = tmpvar_12;
  mediump vec4 tmpvar_13;
  tmpvar_13 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHBr, tmpvar_13);
  x2.x = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHBg, tmpvar_13);
  x2.y = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBb, tmpvar_13);
  x2.z = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = (unity_SHC.xyz * vC);
  x3 = tmpvar_18;
  tmpvar_9 = ((x1 + x2) + x3);
  shlight = tmpvar_9;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_20;
  tmpvar_20 = (unity_4LightPosX0 - tmpvar_19.x);
  highp vec4 tmpvar_21;
  tmpvar_21 = (unity_4LightPosY0 - tmpvar_19.y);
  highp vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosZ0 - tmpvar_19.z);
  highp vec4 tmpvar_23;
  tmpvar_23 = (((tmpvar_20 * tmpvar_20) + (tmpvar_21 * tmpvar_21)) + (tmpvar_22 * tmpvar_22));
  highp vec4 tmpvar_24;
  tmpvar_24 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_20 * tmpvar_7.x) + (tmpvar_21 * tmpvar_7.y)) + (tmpvar_22 * tmpvar_7.z)) * inversesqrt (tmpvar_23))) * 1.0/((1.0 + (tmpvar_23 * unity_4LightAtten0))));
  highp vec3 tmpvar_25;
  tmpvar_25 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_24.x) + (unity_LightColor[1].xyz * tmpvar_24.y)) + (unity_LightColor[2].xyz * tmpvar_24.z)) + (unity_LightColor[3].xyz * tmpvar_24.w)));
  tmpvar_4 = tmpvar_25;
  highp vec4 o_i0;
  highp vec4 tmpvar_26;
  tmpvar_26 = (tmpvar_5 * 0.5);
  o_i0 = tmpvar_26;
  highp vec2 tmpvar_27;
  tmpvar_27.x = tmpvar_26.x;
  tmpvar_27.y = (tmpvar_26.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_27 + tmpvar_26.w);
  o_i0.zw = tmpvar_5.zw;
  gl_Position = tmpvar_5;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * texture2DProj (_ShadowMapTexture, xlv_TEXCOORD4).x) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_4 * xlv_TEXCOORD3));
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

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_7 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  mediump vec3 tmpvar_14;
  tmpvar_14 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_14;
  highp vec2 tmpvar_15;
  tmpvar_15 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, tmpvar_15).w) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
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

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_7 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, lightDir)) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
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

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_7 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  mediump vec3 tmpvar_14;
  tmpvar_14 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_14;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD4.xyz;
  highp vec2 tmpvar_15;
  tmpvar_15 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD4.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD4.xy / xlv_TEXCOORD4.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_15).w);
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * atten) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
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

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_7 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  mediump vec3 tmpvar_14;
  tmpvar_14 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_14;
  highp vec2 tmpvar_15;
  tmpvar_15 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * (texture2D (_LightTextureB0, tmpvar_15).w * textureCube (_LightTexture0, xlv_TEXCOORD4).w)) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
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

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _C2Tex_ST;
uniform highp vec4 _C1Tex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.zw = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _C1Tex_ST.xy) + _C1Tex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _C2Tex_ST.xy) + _C2Tex_ST.zw);
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * (normalize (_glesNormal) * unity_Scale.w));
  tmpvar_3 = tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_7 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_COLOR0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp vec4 _MainColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CausticIntensity;
uniform highp vec4 _CausticColor;
uniform sampler2D _C2Tex;
uniform sampler2D _C1Tex;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_COLOR0;
  lowp vec3 tmpvar_4;
  lowp float tmpvar_5;
  mediump vec4 result;
  mediump vec4 mask2;
  mediump vec4 mask1;
  highp vec2 offset;
  mediump vec4 diffuse;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _MainColor);
  diffuse = tmpvar_7;
  lowp vec2 tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, tmpvar_3).wz;
  offset = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((offset - 0.5) * _CausticIntensity);
  offset = tmpvar_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_C1Tex, (xlv_TEXCOORD1.xy + tmpvar_9));
  mask1 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_C2Tex, (xlv_TEXCOORD1.zw + tmpvar_9));
  mask2 = tmpvar_11;
  result.w = clamp ((mask1.x * tmpvar_1.w), 0.0, 1.0);
  result.w = clamp ((result.w + (mask2.x * tmpvar_1.w)), 0.0, 1.0);
  result.xyz = clamp (((tmpvar_1.xyz * diffuse.xyz) * 2.0), 0.0, 1.0);
  result.xyz = clamp (mix (result.xyz, _CausticColor.xyz, result.www), 0.0, 1.0);
  result.w = 1.0;
  mediump vec3 tmpvar_12;
  tmpvar_12 = result.xyz;
  tmpvar_4 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = result.w;
  tmpvar_5 = tmpvar_13;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0;
  c_i0.xyz = ((tmpvar_4 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD4).w) * 2.0));
  c_i0.w = tmpvar_5;
  c = c_i0;
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