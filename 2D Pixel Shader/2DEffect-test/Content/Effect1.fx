sampler TextureSampler : register(s0);
sampler RainbowSampler : register(s1);
Texture2D  myTex2D;
Texture2D rainbow;
float param1;

//float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    //float4 tex;
	//tex = myTex2D.Sample(TextureSampler, texCoord.xy) * .6f;
	//tex += myTex2D.Sample(TextureSampler, texCoord.xy + (0.005)) * .2f;
	//return tex;

	//base
	//return myTex2D.Sample(TextureSampler, texCoord.xy);

	//grayscale
	//float4 color = myTex2D.Sample(TextureSampler, texCoord.xy);
	//color.gb = color.r;
	//return color;

	//blackout
	//float4 color = myTex2D.Sample(TextureSampler, texCoord.xy);
	//color.rgb =  0;
	//color.rgb = color.gbr;


	//high contrast
	//float high = .6;
	//float low  = .4;

	//if      (color.r > high) color.r = 1;
	//else if (color.r < low) color.r = 0;

	//if      (color.g > high) color.g = 1;
	//else if (color.g < low) color.g = 0;

	//if      (color.b > high) color.b = 1;
	//else if (color.b < low) color.b = 0;

	//if (color.a)
	//color.rgb = 1 - color.rgb;

	//rainbow
	// if (!any(color)) return color;

	// float step = 1.0/7;

	// if      (texCoord.x < (step * 1)) color = float4(1, 0, 0, 1);
	// else if (texCoord.x < (step * 2)) color = float4(1, .5, 0, 1);
	// else if (texCoord.x < (step * 3)) color = float4(1, 1, 0, 1);
	// else if (texCoord.x < (step * 4)) color = float4(0, 1, 0, 1);
	// else if (texCoord.x < (step * 5)) color = float4(0, 0, 1, 1);
	// else if (texCoord.x < (step * 6)) color = float4(.3, 0, .8, 1);
	// else                            color = float4(1, .8, 1, 1);

	//180 rotate
	//float4 color = myTex2D.Sample(TextureSampler, 1 - texCoord);
	
	//horiz mirror
	//float4 color = myTex2D.Sample(TextureSampler, float2(1 - texCoord.x, texCoord.y));
	
	// gradient
	// float4 color = myTex2D.Sample(TextureSampler, texCoord.xy);
	// if (color.a)
		// color.rgb = texCoord.y;
	
	//param 1
	// float4 color = myTex2D.Sample(TextureSampler, texCoord.xy);
	// if (texCoord.y > param1)
		// color = float4(0,0,0,0);
	
	float4 color = myTex2D.Sample(TextureSampler, texCoord.xy);
	float4 rainbow_color = rainbow.Sample(RainbowSampler, texCoord.xy);
	
	if (color.a)
		return rainbow_color;

	return color;
}


technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_3 PixelShaderFunction();  
    }
}