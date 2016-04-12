#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler textureSampler : register(s0);
sampler lightSampler : register(s1);
Texture2D sTexture;
Texture2D lightMask;
float lightIntensity = 1;
float lightRadius;
//float lightFalloff = 2;
//float lightAttenuation = 5000;

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = sTexture.Sample(textureSampler, texCoord.xy);
	float4 lightColor = lightMask.Sample(lightSampler, texCoord.xy);

	//float d = distance(LightPosition, input.WorldPosition);
	//float att = 1 - pow(clamp(d / lightAttenuation, 0, 1),lightFalloff);
	return color * lightColor * lightIntensity;
}


technique Lighting
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};