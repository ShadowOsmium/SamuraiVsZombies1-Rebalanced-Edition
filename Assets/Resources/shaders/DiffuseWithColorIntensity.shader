Shader "Griptonite/DiffuseWithColorIntensity" {
Properties {
 _MainColor ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _ColorIntensity ("Color Intensity", Range(0,1)) = 0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _MainColor;
float _ColorIntensity;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = (tex2D (_MainTex, IN.uv_MainTex) * (1.0 - _ColorIntensity)) + (_MainColor * _ColorIntensity);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}