Shader "GripUnlit/Transparent Cutout With Vert Colors" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="True" "RenderType"="TransparentCutout" }
 Pass {
  Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="True" "RenderType"="TransparentCutout" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord0
  }
  AlphaTest Greater [_Cutoff]
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}