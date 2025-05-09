Shader "GripUnlit/Unlit (Supports Lightmap)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="Vertex" "RenderType"="Opaque" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord0
  }
  SetTexture [_MainTex] { combine texture * primary }
 }
 Pass {
  Tags { "LIGHTMODE"="VertexLM" "RenderType"="Opaque" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  SetTexture [_MainTex] { combine texture }
  SetTexture [unity_Lightmap] { Matrix [unity_LightmapMatrix] combine texture * previous }
  SetTexture [_MainTex] { combine previous +- primary }
 }
 Pass {
  Tags { "LIGHTMODE"="VertexLMRGBM" "RenderType"="Opaque" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  SetTexture [_MainTex] { combine texture }
  SetTexture [unity_Lightmap] { Matrix [unity_LightmapMatrix] combine texture * previous }
  SetTexture [_MainTex] { combine previous +- primary }
 }
}
}