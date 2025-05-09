Shader "Legacy Shaders/Transparent/Cutout/Diffuse_LM" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _LightMap ("Light Map", 2D) = "white" {}
    }
    
    SubShader {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 200
    
    CGPROGRAM
    #pragma surface surf Lambert alphatest:_Cutoff
    
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
    
    Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
    }