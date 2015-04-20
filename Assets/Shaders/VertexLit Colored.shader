Shader "Custom/VertexLit Colored" {
        Properties {
            _Color ("Main Color", Color) = (1,1,1,1)
        }
 
        SubShader {
                Tags {
                        "Queue" = "Transparent"
                        "RenderType" = "Transparent"
                }
				LOD 150
                CGPROGRAM
                #pragma surface surf Lambert
                struct Input {
                        float4 color : color;
                };
                sampler2D _MainTex;
                fixed4 _Color;
               
                void surf(Input IN, inout SurfaceOutput o) {
                        o.Albedo = IN.color.rgb * _Color.rgb;
                        o.Alpha = IN.color.a * _Color.a;
                        o.Specular = 0.2;
                        o.Gloss = 1.0;
                }
                ENDCG
        }
        Fallback "VertexLit", 1
}