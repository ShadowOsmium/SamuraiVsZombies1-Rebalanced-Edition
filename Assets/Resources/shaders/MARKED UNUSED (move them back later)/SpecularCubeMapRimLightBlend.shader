œÒShader "Griptonite/SpecularCubeMapRimLightBlend" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
 _SecondTex ("Texture2", 2D) = "white" {}
 _BlendTex ("Blend Texture", 2D) = "white" {}
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

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
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
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_10;
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
  tmpvar_5 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump float rim;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD3), xlv_TEXCOORD4), 0.0, 1.0));
  rim = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_4 * (tmpvar_11.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_5 = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize (xlv_TEXCOORD3);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_14;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (xlv_TEXCOORD4, lightDir));
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (xlv_TEXCOORD4, normalize ((lightDir + viewDir))));
  nh = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_15) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * 2.0);
  c_i0.xyz = tmpvar_17;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_4 * xlv_TEXCOORD5));
  c.xyz = (c.xyz + tmpvar_5);
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

varying highp vec2 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect (-((((_World2Object * tmpvar_5).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_9;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _SecondTex;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump float rim;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD3), xlv_TEXCOORD4), 0.0, 1.0));
  rim = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_4 * (tmpvar_11.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_5 = tmpvar_13;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_4 * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD5).xyz));
  c.w = 0.0;
  c.xyz = (c.xyz + tmpvar_5);
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

varying highp vec4 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
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
uniform highp vec4 _SecondTex_ST;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = tmpvar_11;
  mediump vec3 tmpvar_13;
  mediump vec4 normal;
  normal = tmpvar_12;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAr, normal);
  x1.x = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAg, normal);
  x1.y = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHAb, normal);
  x1.z = tmpvar_16;
  mediump vec4 tmpvar_17;
  tmpvar_17 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBr, tmpvar_17);
  x2.x = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBg, tmpvar_17);
  x2.y = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = dot (unity_SHBb, tmpvar_17);
  x2.z = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_21;
  highp vec3 tmpvar_22;
  tmpvar_22 = (unity_SHC.xyz * vC);
  x3 = tmpvar_22;
  tmpvar_13 = ((x1 + x2) + x3);
  shlight = tmpvar_13;
  tmpvar_5 = shlight;
  highp vec4 o_i0;
  highp vec4 tmpvar_23;
  tmpvar_23 = (tmpvar_6 * 0.5);
  o_i0 = tmpvar_23;
  highp vec2 tmpvar_24;
  tmpvar_24.x = tmpvar_23.x;
  tmpvar_24.y = (tmpvar_23.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_24 + tmpvar_23.w);
  o_i0.zw = tmpvar_6.zw;
  gl_Position = tmpvar_6;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _SecondTex;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump float rim;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD3), xlv_TEXCOORD4), 0.0, 1.0));
  rim = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_4 * (tmpvar_11.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_5 = tmpvar_13;
  lowp float tmpvar_14;
  tmpvar_14 = texture2DProj (_ShadowMapTexture, xlv_TEXCOORD6).x;
  highp vec3 tmpvar_15;
  tmpvar_15 = normalize (xlv_TEXCOORD3);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_15;
  mediump float atten_i0;
  atten_i0 = tmpvar_14;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (xlv_TEXCOORD4, lightDir));
  mediump float tmpvar_17;
  tmpvar_17 = max (0.0, dot (xlv_TEXCOORD4, normalize ((lightDir + viewDir))));
  nh = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_16) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten_i0 * 2.0));
  c_i0.xyz = tmpvar_18;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_4 * xlv_TEXCOORD5));
  c.xyz = (c.xyz + tmpvar_5);
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

varying highp vec4 xlv_TEXCOORD6;
varying highp vec2 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_10;
  highp vec4 o_i0;
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_5 * 0.5);
  o_i0 = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12.x = tmpvar_11.x;
  tmpvar_12.y = (tmpvar_11.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_12 + tmpvar_11.w);
  o_i0.zw = tmpvar_5.zw;
  gl_Position = tmpvar_5;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD6 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD6;
varying highp vec2 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _SecondTex;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump float rim;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD3), xlv_TEXCOORD4), 0.0, 1.0));
  rim = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_4 * (tmpvar_11.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_5 = tmpvar_13;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_4 * min ((2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD5).xyz), vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD6).x * 2.0))));
  c.w = 0.0;
  c.xyz = (c.xyz + tmpvar_5);
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

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
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

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * reflect (-((((_World2Object * tmpvar_6).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_10;
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
  tmpvar_5 = shlight;
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
  tmpvar_28 = (tmpvar_5 + ((((unity_LightColor[0].xyz * tmpvar_27.x) + (unity_LightColor[1].xyz * tmpvar_27.y)) + (unity_LightColor[2].xyz * tmpvar_27.z)) + (unity_LightColor[3].xyz * tmpvar_27.w)));
  tmpvar_5 = tmpvar_28;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump float rim;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD3), xlv_TEXCOORD4), 0.0, 1.0));
  rim = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_4 * (tmpvar_11.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_5 = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize (xlv_TEXCOORD3);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_14;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (xlv_TEXCOORD4, lightDir));
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (xlv_TEXCOORD4, normalize ((lightDir + viewDir))));
  nh = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_15) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * 2.0);
  c_i0.xyz = tmpvar_17;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_4 * xlv_TEXCOORD5));
  c.xyz = (c.xyz + tmpvar_5);
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

varying highp vec4 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
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

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = tmpvar_11;
  mediump vec3 tmpvar_13;
  mediump vec4 normal;
  normal = tmpvar_12;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAr, normal);
  x1.x = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAg, normal);
  x1.y = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHAb, normal);
  x1.z = tmpvar_16;
  mediump vec4 tmpvar_17;
  tmpvar_17 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBr, tmpvar_17);
  x2.x = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBg, tmpvar_17);
  x2.y = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = dot (unity_SHBb, tmpvar_17);
  x2.z = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_21;
  highp vec3 tmpvar_22;
  tmpvar_22 = (unity_SHC.xyz * vC);
  x3 = tmpvar_22;
  tmpvar_13 = ((x1 + x2) + x3);
  shlight = tmpvar_13;
  tmpvar_5 = shlight;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosX0 - tmpvar_23.x);
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_4LightPosY0 - tmpvar_23.y);
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_4LightPosZ0 - tmpvar_23.z);
  highp vec4 tmpvar_27;
  tmpvar_27 = (((tmpvar_24 * tmpvar_24) + (tmpvar_25 * tmpvar_25)) + (tmpvar_26 * tmpvar_26));
  highp vec4 tmpvar_28;
  tmpvar_28 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_24 * tmpvar_11.x) + (tmpvar_25 * tmpvar_11.y)) + (tmpvar_26 * tmpvar_11.z)) * inversesqrt (tmpvar_27))) * 1.0/((1.0 + (tmpvar_27 * unity_4LightAtten0))));
  highp vec3 tmpvar_29;
  tmpvar_29 = (tmpvar_5 + ((((unity_LightColor[0].xyz * tmpvar_28.x) + (unity_LightColor[1].xyz * tmpvar_28.y)) + (unity_LightColor[2].xyz * tmpvar_28.z)) + (unity_LightColor[3].xyz * tmpvar_28.w)));
  tmpvar_5 = tmpvar_29;
  highp vec4 o_i0;
  highp vec4 tmpvar_30;
  tmpvar_30 = (tmpvar_6 * 0.5);
  o_i0 = tmpvar_30;
  highp vec2 tmpvar_31;
  tmpvar_31.x = tmpvar_30.x;
  tmpvar_31.y = (tmpvar_30.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_31 + tmpvar_30.w);
  o_i0.zw = tmpvar_6.zw;
  gl_Position = tmpvar_6;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp float _SpecStrength;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _SecondTex;
uniform highp float _RimPower;
uniform highp vec4 _RimColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump float rim;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (1.0 - clamp (dot (normalize (xlv_TEXCOORD3), xlv_TEXCOORD4), 0.0, 1.0));
  rim = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_4 * (tmpvar_11.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_RimColor.xyz * pow (rim, _RimPower));
  tmpvar_5 = tmpvar_13;
  lowp float tmpvar_14;
  tmpvar_14 = texture2DProj (_ShadowMapTexture, xlv_TEXCOORD6).x;
  highp vec3 tmpvar_15;
  tmpvar_15 = normalize (xlv_TEXCOORD3);
  mediump vec3 lightDir;
  lightDir = _WorldSpaceLightPos0.xyz;
  mediump vec3 viewDir;
  viewDir = tmpvar_15;
  mediump float atten_i0;
  atten_i0 = tmpvar_14;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (xlv_TEXCOORD4, lightDir));
  mediump float tmpvar_17;
  tmpvar_17 = max (0.0, dot (xlv_TEXCOORD4, normalize ((lightDir + viewDir))));
  nh = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_16) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten_i0 * 2.0));
  c_i0.xyz = tmpvar_18;
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c.xyz + (tmpvar_4 * xlv_TEXCOORD5));
  c.xyz = (c.xyz + tmpvar_5);
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

varying highp vec3 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_6 = tmpvar_13;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
  xlv_TEXCOORD6 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_6;
  lowp float tmpvar_7;
  tmpvar_7 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_4 * (tmpvar_9.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_10;
  mediump vec3 tmpvar_11;
  tmpvar_11 = normalize (xlv_TEXCOORD4);
  lightDir = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = vec2(dot (xlv_TEXCOORD6, xlv_TEXCOORD6));
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture2D (_LightTexture0, tmpvar_12);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = tmpvar_13.w;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_14;
  tmpvar_14 = max (0.0, dot (xlv_TEXCOORD3, lightDir_i0));
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD5)))));
  nh = tmpvar_15;
  highp vec3 tmpvar_16;
  tmpvar_16 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_14) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_16;
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

varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = _WorldSpaceLightPos0.xyz;
  tmpvar_5 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_6 = tmpvar_13;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_6;
  lowp float tmpvar_7;
  tmpvar_7 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_4 * (tmpvar_9.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_10;
  lightDir = xlv_TEXCOORD4;
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (xlv_TEXCOORD3, lightDir_i0));
  mediump float tmpvar_12;
  tmpvar_12 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD5)))));
  nh = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_11) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * 2.0);
  c_i0.xyz = tmpvar_13;
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

varying highp vec4 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_6 = tmpvar_13;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
  xlv_TEXCOORD6 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_6;
  lowp float tmpvar_7;
  tmpvar_7 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_4 * (tmpvar_9.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_10;
  mediump vec3 tmpvar_11;
  tmpvar_11 = normalize (xlv_TEXCOORD4);
  lightDir = tmpvar_11;
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_LightTexture0, ((xlv_TEXCOORD6.xy / xlv_TEXCOORD6.w) + 0.5));
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD6.xyz;
  highp vec2 tmpvar_13;
  tmpvar_13 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp vec4 tmpvar_14;
  tmpvar_14 = texture2D (_LightTextureB0, tmpvar_13);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = ((float((xlv_TEXCOORD6.z > 0.0)) * tmpvar_12.w) * tmpvar_14.w);
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (xlv_TEXCOORD3, lightDir_i0));
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD5)))));
  nh = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_15) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_17;
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

varying highp vec3 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_5 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_6 = tmpvar_13;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
  xlv_TEXCOORD6 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_6;
  lowp float tmpvar_7;
  tmpvar_7 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_4 * (tmpvar_9.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_10;
  mediump vec3 tmpvar_11;
  tmpvar_11 = normalize (xlv_TEXCOORD4);
  lightDir = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = vec2(dot (xlv_TEXCOORD6, xlv_TEXCOORD6));
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture2D (_LightTextureB0, tmpvar_12);
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_LightTexture0, xlv_TEXCOORD6);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = (tmpvar_13.w * tmpvar_14.w);
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (xlv_TEXCOORD3, lightDir_i0));
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD5)))));
  nh = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_15) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_17;
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

varying highp vec2 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _SecondTex_ST;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _BlendTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  mediump vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mediump vec3 tmpvar_5;
  mediump vec3 tmpvar_6;
  tmpvar_2.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.zw = ((_glesMultiTexCoord0.xy * _SecondTex_ST.xy) + _SecondTex_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * reflect (-((((_World2Object * tmpvar_7).xyz * unity_Scale.w) - _glesVertex.xyz)), tmpvar_1));
  tmpvar_3 = tmpvar_9;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (tmpvar_1 * unity_Scale.w));
  tmpvar_4 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = _WorldSpaceLightPos0.xyz;
  tmpvar_5 = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  tmpvar_6 = tmpvar_13;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _BlendTex_ST.xy) + _BlendTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
  xlv_TEXCOORD6 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD6;
varying mediump vec3 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp float _SpecStrength;
uniform sampler2D _SecondTex;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform highp float _CubeMapPower;
uniform samplerCube _Cube;
uniform sampler2D _BlendTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD2;
  lowp vec3 tmpvar_4;
  highp float colorBlend;
  highp vec3 color2;
  highp vec3 color1;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, tmpvar_2).xyz;
  color1 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_SecondTex, tmpvar_3).xyz;
  color2 = tmpvar_6;
  lowp float tmpvar_7;
  tmpvar_7 = texture2D (_BlendTex, xlv_TEXCOORD1).w;
  colorBlend = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((color1 * colorBlend) + (color2 * (1.0 - colorBlend)));
  tmpvar_4 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureCube (_Cube, tmpvar_1);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_4 * (tmpvar_9.xyz * _CubeMapPower));
  tmpvar_4 = tmpvar_10;
  lightDir = xlv_TEXCOORD4;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_LightTexture0, xlv_TEXCOORD6);
  mediump vec3 lightDir_i0;
  lightDir_i0 = lightDir;
  mediump float atten;
  atten = tmpvar_11.w;
  mediump vec4 c_i0;
  highp float nh;
  mediump float tmpvar_12;
  tmpvar_12 = max (0.0, dot (xlv_TEXCOORD3, lightDir_i0));
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (xlv_TEXCOORD3, normalize ((lightDir_i0 + normalize (xlv_TEXCOORD5)))));
  nh = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = ((((tmpvar_4 * _LightColor0.xyz) * tmpvar_12) + (_LightColor0.xyz * pow (nh, _SpecStrength))) * (atten * 2.0));
  c_i0.xyz = tmpvar_14;
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