Shader "ActionStreetMap/Vertex Color Water" {
    Properties {
        _Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Freq ("Frequency", Float) = 1
        _Speed ("Speed", Float) = 1
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
                uniform float _Freq;
			    uniform float _Speed;

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
                    
                    float4 srcPos = mul(UNITY_MATRIX_MVP, v.vertex);
                    
                    //srcPos = mul(_Object2World, v.vertex).xyzw;
                    srcPos /= _Freq;
                    srcPos.y += _CosTime * _Speed;
                    
                    o.pos = srcPos;
             
                    // diffuse lighting
                    float3 normal_dir = normalize(float4(mul(float4(v.normal, 0.0), _World2Object)));
                    float3 light_dir = normalize(float4(_WorldSpaceLightPos0));
                    float3 diffuse_reflection = float4(_LightColor0) * float4(_Color) * max(0.0, dot(normal_dir, light_dir));
                    
                    // ambient
                    float3 ambient_lighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;
                    
                    float3 lightning = diffuse_reflection + ambient_lighting;

                    o.color = lerp(v.color, float4(lightning, 1.0), 0.5);
                    
                    TRANSFER_VERTEX_TO_FRAGMENT(o);

                    return o;
                };

                float4 frag(vertex_output v) : COLOR {
                    return v.color;
                };
            ENDCG    
        }
    }
}