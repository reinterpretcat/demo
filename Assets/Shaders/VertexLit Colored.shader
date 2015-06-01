Shader "Custom/VertexLit Colored" {
    Properties {
        _Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                #include "AutoLight.cginc"

                uniform float4 _Color;

                struct vertex_input {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float3 normal : NORMAL;
                };

                struct vertex_output {
                    float4 pos : POSITION;
                    float4 color : COLOR;
                    LIGHTING_COORDS(3, 4)
                };

                vertex_output vert(vertex_input v) {
                    vertex_output o;                   
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
             
                    // calculate the diffuse lighting
                    float3 normal_dir = normalize(float4(mul(float4(v.normal, 0.0), _World2Object)));
                    float3 light_dir = normalize(float4(_WorldSpaceLightPos0));
                    float3 diffuse_reflection = float4(_LightColor0) * float4(_Color) * max(0.0, dot(normal_dir, light_dir));

                    //o.color = v.color + float4(diffuse_reflection, 1.0) * 0.5;
                    o.color = lerp(v.color, float4(diffuse_reflection, 1.0), 0.5);
                    TRANSFER_VERTEX_TO_FRAGMENT(o);

                    return o;
                };

                float4 frag(vertex_output v) : COLOR {
                    return v.color;
                };
            ENDCG    
        }
    }
    Fallback "VertexLit", 1
}