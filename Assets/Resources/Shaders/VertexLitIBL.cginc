// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Copyright © 2014 Laurens Mathot
// Code Animo™ http://codeanimo.com
// License terms can be found at the bottom of this file.
#ifndef QT_VERTEX_LIT_IBL
	#define QT_VERTEX_LIT_IBL
	
	#include "QT_Cubemaps.cginc"
	
	struct vertexInput{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		fixed4 color : COLOR;
		#ifndef LIGHTMAP_OFF
		float4 texcoord1 : TEXCOORD1;
		#endif
	};
	struct vertex2Fragment{
		float4 position : SV_POSITION;
		float4 color : COLOR;
		fixed3 albedo : TEXCOORD0;
		float3 worldNormal : TEXCOORD1;
		#ifndef LIGHTMAP_OFF
		float2 lmap : TEXCOORD2;
		#endif
		#ifdef QT_REFLECTION
		float3 viewRefl : TEXCOORD3;
		#endif
	};
	
	#ifndef LIGHTMAP_OFF
		// float4 unity_LightmapST;
	#endif
	
	void vert(vertexInput v, out vertex2Fragment o){
		o.position = mul (UNITY_MATRIX_MVP, v.vertex);
		
		#ifdef QT_REFLECTION
		float3 viewDir = -ObjSpaceViewDir(v.vertex);
	  	o.viewRefl = reflect (viewDir, v.normal);
	  	#endif
	  	
		float3 worldNormal = mul ((float3x3)_Object2World, SCALED_NORMAL);
		o.worldNormal = worldNormal;
		
		o.color.a = 1;
		
		o.albedo = v.color.rgb;// Need albedo value in fragment shader for IBL and lightmap	
		float3 diffuseLight = ShadeVertexLights(v.vertex, v.normal) * 2;// Twice as bright to match regular fragment shader brightness
		
		#ifdef LIGHTMAP_OFF
		diffuseLight += ShadeSH9(half4(worldNormal, 1.0) );// Light Probes
		#endif
		
		#ifndef LIGHTMAP_OFF
		o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif
		
		o.color.rgb = o.albedo * diffuseLight;
		
		#ifdef QT_EMISSION_ENABLED
		o.color.rgb += o.albedo * _Emission * (1 - v.color.a);
		#endif
	}
	
	#ifndef LIGHTMAP_OFF
	// sampler2D unity_Lightmap;
	#endif
	
	float4 frag(vertex2Fragment IN) : COLOR {
		fixed4 outColor = IN.color;
		outColor.rgb += ImageBasedLighting(IN.worldNormal, IN.albedo.rgb);
		
		#ifdef QT_REFLECTION
		outColor.rgb += calculateReflection(IN.viewRefl) * _ReflectionAmount;
		#endif
		
		#ifndef LIGHTMAP_OFF
		fixed4 lightmapTexture = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap);
		fixed3 lightmapLight = DecodeLightmap(lightmapTexture);// Decode Lightmap automatically figures out whether it should use RGBM or DoubleLDR
		outColor.rgb += lightmapLight * IN.albedo;	
		#endif
		
		
		return outColor;
	}

#endif
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