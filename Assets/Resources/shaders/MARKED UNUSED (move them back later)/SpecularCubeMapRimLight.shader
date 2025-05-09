؁Shader "Griptonite/SpecularCubeMapRimLight" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
 _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
 _RimPower ("Rim Power", Range(0.5,8)) = 3
 _CubeMapPower ("Cube Power", Range(0.5,3)) = 0.5
 _Cube ("Cubemap", CUBE) = "" {}
 _SpecStrength ("Specular Strength", Range(0.01,200)) = 100
}
SubShader { 
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

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect (-((((_World2Object * tmpvar_5).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump float rim;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD2), xlv_TEXCOORD3), 0.0, 1.0));
  rim = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_4.xyz * (tmpvar_6.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_3 = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD2);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_9;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (xlv_TEXCOORD3, lightDir));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir + viewDir))));
  nh = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_10) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * 2.0);
  c_i0.xyz = tmpvar_12;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_2 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + tmpvar_3);
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

varying highp vec2 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect (-((((_World2Object * tmpvar_4).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump float rim;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD2), xlv_TEXCOORD3), 0.0, 1.0));
  rim = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_4.xyz * (tmpvar_6.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_3 = tmpvar_8;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_2 * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD4).xyz));
  c.w = 0.0;
  c.xyz = (c.xyz + tmpvar_3);
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

varying highp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_10;
  mediump vec3 tmpvar_12;
  mediump vec4 normal;
  normal = tmpvar_11;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAr, normal);
  x1.x = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAg, normal);
  x1.y = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAb, normal);
  x1.z = tmpvar_15;
  mediump vec4 tmpvar_16;
  tmpvar_16 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBr, tmpvar_16);
  x2.x = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBg, tmpvar_16);
  x2.y = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBb, tmpvar_16);
  x2.z = tmpvar_19;
  mediump float tmpvar_20;
  tmpvar_20 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (unity_SHC.xyz * vC);
  x3 = tmpvar_21;
  tmpvar_12 = ((x1 + x2) + x3);
  shlight = tmpvar_12;
  tmpvar_4 = shlight;
  highp vec4 o_i0;
  highp vec4 tmpvar_22;
  tmpvar_22 = (tmpvar_5 * 0.5);
  o_i0 = tmpvar_22;
  highp vec2 tmpvar_23;
  tmpvar_23.x = tmpvar_22.x;
  tmpvar_23.y = (tmpvar_22.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_23 + tmpvar_22.w);
  o_i0.zw = tmpvar_5.zw;
  gl_Position = tmpvar_5;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform sampler2D _ShadowMapTexture;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump float rim;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD2), xlv_TEXCOORD3), 0.0, 1.0));
  rim = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_4.xyz * (tmpvar_6.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_3 = tmpvar_8;
  lowp float tmpvar_9;
  tmpvar_9 = texture2DProj (_ShadowMapTexture, xlv_TEXCOORD5).x;
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (xlv_TEXCOORD2);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_10;
  mediump float atten_i0;
  atten_i0 = tmpvar_9;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD3, lightDir));
  mediump float tmpvar_12;
  tmpvar_12 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir + viewDir))));
  nh = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_11) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten_i0 * 2.0));
  c_i0.xyz = tmpvar_13;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_2 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + tmpvar_3);
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

varying highp vec4 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect (-((((_World2Object * tmpvar_5).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 o_i0;
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_4 * 0.5);
  o_i0 = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11.x = tmpvar_10.x;
  tmpvar_11.y = (tmpvar_10.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_11 + tmpvar_10.w);
  o_i0.zw = tmpvar_4.zw;
  gl_Position = tmpvar_4;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump float rim;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD2), xlv_TEXCOORD3), 0.0, 1.0));
  rim = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_4.xyz * (tmpvar_6.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_3 = tmpvar_8;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_2 * min ((2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD4).xyz), vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD5).x * 2.0))));
  c.w = 0.0;
  c.xyz = (c.xyz + tmpvar_3);
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

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
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

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect (-((((_World2Object * tmpvar_5).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosX0 - tmpvar_21.x);
  highp vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosY0 - tmpvar_21.y);
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosZ0 - tmpvar_21.z);
  highp vec4 tmpvar_25;
  tmpvar_25 = (((tmpvar_22 * tmpvar_22) + (tmpvar_23 * tmpvar_23)) + (tmpvar_24 * tmpvar_24));
  highp vec4 tmpvar_26;
  tmpvar_26 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_22 * tmpvar_9.x) + (tmpvar_23 * tmpvar_9.y)) + (tmpvar_24 * tmpvar_9.z)) * inversesqrt (tmpvar_25))) * 1.0/((1.0 + (tmpvar_25 * unity_4LightAtten0))));
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_26.x) + (unity_LightColor[1].xyz * tmpvar_26.y)) + (unity_LightColor[2].xyz * tmpvar_26.z)) + (unity_LightColor[3].xyz * tmpvar_26.w)));
  tmpvar_4 = tmpvar_27;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump float rim;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD2), xlv_TEXCOORD3), 0.0, 1.0));
  rim = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_4.xyz * (tmpvar_6.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_3 = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD2);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_9;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (xlv_TEXCOORD3, lightDir));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir + viewDir))));
  nh = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_10) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * 2.0);
  c_i0.xyz = tmpvar_12;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_2 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + tmpvar_3);
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

varying highp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
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

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_10;
  mediump vec3 tmpvar_12;
  mediump vec4 normal;
  normal = tmpvar_11;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAr, normal);
  x1.x = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAg, normal);
  x1.y = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAb, normal);
  x1.z = tmpvar_15;
  mediump vec4 tmpvar_16;
  tmpvar_16 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBr, tmpvar_16);
  x2.x = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBg, tmpvar_16);
  x2.y = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBb, tmpvar_16);
  x2.z = tmpvar_19;
  mediump float tmpvar_20;
  tmpvar_20 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (unity_SHC.xyz * vC);
  x3 = tmpvar_21;
  tmpvar_12 = ((x1 + x2) + x3);
  shlight = tmpvar_12;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_22;
  tmpvar_22 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosX0 - tmpvar_22.x);
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosY0 - tmpvar_22.y);
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_4LightPosZ0 - tmpvar_22.z);
  highp vec4 tmpvar_26;
  tmpvar_26 = (((tmpvar_23 * tmpvar_23) + (tmpvar_24 * tmpvar_24)) + (tmpvar_25 * tmpvar_25));
  highp vec4 tmpvar_27;
  tmpvar_27 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_23 * tmpvar_10.x) + (tmpvar_24 * tmpvar_10.y)) + (tmpvar_25 * tmpvar_10.z)) * inversesqrt (tmpvar_26))) * 1.0/((1.0 + (tmpvar_26 * unity_4LightAtten0))));
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_27.x) + (unity_LightColor[1].xyz * tmpvar_27.y)) + (unity_LightColor[2].xyz * tmpvar_27.z)) + (unity_LightColor[3].xyz * tmpvar_27.w)));
  tmpvar_4 = tmpvar_28;
  highp vec4 o_i0;
  highp vec4 tmpvar_29;
  tmpvar_29 = (tmpvar_5 * 0.5);
  o_i0 = tmpvar_29;
  highp vec2 tmpvar_30;
  tmpvar_30.x = tmpvar_29.x;
  tmpvar_30.y = (tmpvar_29.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_30 + tmpvar_29.w);
  o_i0.zw = tmpvar_5.zw;
  gl_Position = tmpvar_5;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform sampler2D _ShadowMapTexture;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump float rim;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD2), xlv_TEXCOORD3), 0.0, 1.0));
  rim = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_4.xyz * (tmpvar_6.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_3 = tmpvar_8;
  lowp float tmpvar_9;
  tmpvar_9 = texture2DProj (_ShadowMapTexture, xlv_TEXCOORD5).x;
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (xlv_TEXCOORD2);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_10;
  mediump float atten_i0;
  atten_i0 = tmpvar_9;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD3, lightDir));
  mediump float tmpvar_12;
  tmpvar_12 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir + viewDir))));
  nh = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_11) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten_i0 * 2.0));
  c_i0.xyz = tmpvar_13;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_2 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + tmpvar_3);
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

varying highp vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_3.xyz * (tmpvar_4.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_6 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = vec2(dot (xlv_TEXCOORD5, xlv_TEXCOORD5));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_LightTexture0, tmpvar_7);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = tmpvar_8.w;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_9;
  tmpvar_9 = max (0.0, dot (xlv_TEXCOORD2, lightDir_i0));
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (xlv_TEXCOORD2, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD4)))));
  nh = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_9) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_11;
  c_i0.w = 0.0;
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

varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_3.xyz * (tmpvar_4.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_5;
  lightDir = xlv_TEXCOORD3;
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_6;
  tmpvar_6 = max (0.0, dot (xlv_TEXCOORD2, lightDir_i0));
  mediump float tmpvar_7;
  tmpvar_7 = max (0.0, dot (xlv_TEXCOORD2, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD4)))));
  nh = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_6) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * 2.0);
  c_i0.xyz = tmpvar_8;
  c_i0.w = 0.0;
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

varying highp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_3.xyz * (tmpvar_4.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_6 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_6;
  lowp vec4 tmpvar_7;
  tmpvar_7 = texture2D (_LightTexture0, ((xlv_TEXCOORD5.xy / xlv_TEXCOORD5.w) + 0.5));
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD5.xyz;
  highp vec2 tmpvar_8;
  tmpvar_8 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_LightTextureB0, tmpvar_8);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = ((float((xlv_TEXCOORD5.z > 0.0)) * tmpvar_7.w) * tmpvar_9.w);
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (xlv_TEXCOORD2, lightDir_i0));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD2, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD4)))));
  nh = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_10) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_12;
  c_i0.w = 0.0;
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

varying highp vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_3.xyz * (tmpvar_4.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_6 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = vec2(dot (xlv_TEXCOORD5, xlv_TEXCOORD5));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_LightTextureB0, tmpvar_7);
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureCube (_LightTexture0, xlv_TEXCOORD5);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = (tmpvar_8.w * tmpvar_9.w);
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (xlv_TEXCOORD2, lightDir_i0));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD2, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD4)))));
  nh = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_10) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_12;
  c_i0.w = 0.0;
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

varying highp vec2 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_3.xyz * (tmpvar_4.xyz * _CubeMapPower));
  tmpvar_2 = tmpvar_5;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_LightTexture0, xlv_TEXCOORD5);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = tmpvar_6.w;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_7;
  tmpvar_7 = max (0.0, dot (xlv_TEXCOORD2, lightDir_i0));
  mediump float tmpvar_8;
  tmpvar_8 = max (0.0, dot (xlv_TEXCOORD2, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD4)))));
  nh = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((((tmpvar_2 * _LightColor0.xyz) * tmpvar_7) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_9;
  c_i0.w = 0.0;
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