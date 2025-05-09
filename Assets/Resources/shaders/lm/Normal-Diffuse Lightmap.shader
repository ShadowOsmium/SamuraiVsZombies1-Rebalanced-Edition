Shader "Legacy Shaders/Diffuse_LM" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _LightMap ("Light Map", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
    
    CGPROGRAM
    #pragma surface surf Lambert
    
    sampler2D _MainTex, _LightMap;
    fixed4 _Color;
    
    struct Input {
        float2 uv_MainTex;
        float2 uv2_LightMap;
    };
    
    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgb * (tex2D(_LightMap, IN.uv2_LightMap) * 2.0).rgb;
        o.Alpha = c.a;
    }
    ENDCG
    }
    
    Fallback "Legacy Shaders/VertexLit"
    }