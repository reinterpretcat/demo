Shader "Custom/VertexLit Colored" {
    Properties {
        _Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Saturation ("Saturation", Float) = 1
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
                uniform float _Saturation; 

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
                
                inline float3 rgb2hsv(float3 c)
                {
                    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                    float d = q.x - min(q.w, q.y);
                    float e = 1.0e-10;
                    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
                }

                inline float3 hsv2rgb(float3 c)
                {
                    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
                }

                vertex_output vert(vertex_input v) {
                    vertex_output o;                   
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
             
                    // calculate the diffuse lighting
                    float3 normal_dir = normalize(float4(mul(float4(v.normal, 0.0), _World2Object)));
                    float3 light_dir = normalize(float4(_WorldSpaceLightPos0));
                    float3 diffuse_reflection = float4(_LightColor0) * float4(_Color) * max(0.0, dot(normal_dir, light_dir));
                    
                    // specular
                    //float3 view_dir = normalize(_WorldSpaceCameraPos - o.pos.xyz);
                    //float3 specular_reflection = _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-light_dir, normal_dir), view_dir)), _Shininess);
                    
                    // ambient
                    //float3 ambient_lighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;
                    
                    float3 lightning = diffuse_reflection;// + ambient_lighting;

                    //o.color = v.color + float4(lightning, 1.0) * 0.5;
                    //o.color = lerp(v.color, float4(lightning, 1.0), 0.5;
                    
                    float3 hsv_color = rgb2hsv(lerp(v.color, float4(lightning, 1.0), 0.5).rgb) * float3(1.0, _Saturation, 1.0);
                    o.color = float4(hsv2rgb(hsv_color), 1.0);
                    
                    TRANSFER_VERTEX_TO_FRAGMENT(o);

                    return o;
                };

                float4 frag(vertex_output v) : COLOR {
                    return v.color;
                };
            ENDCG    
        }
    }
    //Fallback "VertexLit", 1
}