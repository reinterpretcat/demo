// Copyright © 2014 Laurens Mathot
// Code Animo™ http://codeanimo.com
// License terms can be found at the bottom of this file.

Shader "QuantumTheory/VertexColors/IBL/Reflective" {
	Properties {
		_DiffuseIBL("Diffuse IBL", CUBE) = "" {}
		_IBLBrightness("IBL Brightness", Float) = 1
		
		_CubeMipLevel ("Reflection Cube Mip Level", Float) = 0
		
		_ReflectionCube ("Reflection", CUBE) = "" {}
		_ReflectionAmount ("Reflection Amount", Range(0,1)) = 1
		_ReflectionTint ("Reflection Tint", Color) = (1,1,1,1)
	}
	CGINCLUDE
	
	float _IBLBrightness;
	float _ReflectionAmount;
	
	ENDCG
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 350
		
		// Surface Shader Passes:
		CGPROGRAM
		#pragma surface surf Lambert addshadow
		#pragma target 3.0
		#pragma glsl
		#define QT_REFLECTION
		
		#include "QT_Cubemaps.cginc"
		
		
		struct Input {
			float4 color : COLOR;
			float3 worldNormal;
			float3 worldRefl;
		};
	
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.color.rgb;
			float3 IBL = ImageBasedLighting(IN.worldNormal, IN.color.rgb);
			float3 reflection = calculateReflection(IN.worldRefl) * _ReflectionAmount;
			o.Emission = IBL + reflection;
		}
		ENDCG
		
		// Vertex Lit:
		Pass {
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			
			#define QT_REFLECTION
			
			#include "UnityCG.cginc"
			#include "VertexLitIBL.cginc"
			
			
			
			ENDCG
		}
		// Vertex Lit Lightmapped:
		Pass {
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			
			#define QT_REFLECTION
			
			#include "UnityCG.cginc"
			#include "VertexLitIBL.cginc"
			
			
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			
			#define QT_REFLECTION
			
			#include "UnityCG.cginc"
			#include "VertexLitIBL.cginc"
			
			
			ENDCG
		}
	}
	Fallback "QuantumTheory/VertexColors/VertexLit"
	
	
}

// Copyright © 2014 Laurens Mathot
// Code Animo™ http://codeanimo.com
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.