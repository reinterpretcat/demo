// Copyright © 2014 Laurens Mathot
// Code Animo™ http://codeanimo.com
// License terms can be found at the bottom of this file.
#ifndef QT_CUBEMAPS_INCLUDED
#define QT_CUBEMAPS_INCLUDED

	#ifdef SHADER_API_D3D11
		TextureCube _DiffuseIBL;
		SamplerState sampler_DiffuseIBL;
		
		TextureCube _ReflectionCube;
		SamplerState sampler_ReflectionCube;
		
		#define SAMPLE_CUBE_LEVEL(texture, worldNormal, level) texture.SampleLevel(sampler##texture, worldNormal, level)
	#else
		samplerCUBE _DiffuseIBL;
		samplerCUBE _ReflectionCube;
		#ifdef SHADER_API_GLES
			#define SAMPLE_CUBE_LEVEL(texture, worldNormal, level) texCUBE(texture, worldNormal)
		#else
			#define SAMPLE_CUBE_LEVEL(texture, worldNormal, level) texCUBElod(texture, float4(worldNormal, level))
		#endif
	#endif
	
	
	float3 ImageBasedLighting(float3 worldNormal,float3 albedo){
		float3 cubeSample = SAMPLE_CUBE_LEVEL(_DiffuseIBL, worldNormal, 0).rgb;
		return cubeSample * _IBLBrightness * albedo;
	}
	
	#ifdef QT_REFLECTION
	float _CubeMipLevel;
	float4 _ReflectionTint;
	
	float3 calculateReflection(float3 reflectionDirection){
		float3 cubeSample = SAMPLE_CUBE_LEVEL(_ReflectionCube, reflectionDirection, _CubeMipLevel).rgb;
		return cubeSample *= _ReflectionTint.rgb;
	}
	#endif
	
	
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