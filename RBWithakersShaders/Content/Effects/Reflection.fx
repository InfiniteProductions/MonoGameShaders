﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
 
float4 TintColor = float4(1, 1, 1, 1);
float3 CameraPosition;
 
Texture SkyboxTexture; 
samplerCUBE SkyboxSampler = sampler_state 
{ 
   texture = <SkyboxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};
 
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
};
 
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;  // POSITION0 or this: no change
    float3 Reflection : TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    float3 ViewDirection = CameraPosition - VertexPosition;
 
    float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
    output.Reflection = reflect(-normalize(ViewDirection), normalize(Normal));
 
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    return TintColor * texCUBE(SkyboxSampler, normalize(input.Reflection));
}
 
technique Reflection
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}