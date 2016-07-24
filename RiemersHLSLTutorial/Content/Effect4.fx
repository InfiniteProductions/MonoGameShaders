struct VertexToPixel
{
    float4 Position     : POSITION;    
    float2 TexCoords    : TEXCOORD0;
    float3 Normal        : TEXCOORD1;
    float3 Position3D    : TEXCOORD2;
};

struct PixelToFrame
{
    float4 Color        : COLOR0;
};

struct SMapVertexToPixel
{
    float4 Position     : POSITION;
    float4 Position2D    : TEXCOORD0;
};

struct SMapPixelToFrame
{
    float4 Color : COLOR0;
};


struct SSceneVertexToPixel
{
    float4 Position             : POSITION;
    float4 Pos2DAsSeenByLight    : TEXCOORD0;
	float2 TexCoords            : TEXCOORD1;
	float3 Normal                : TEXCOORD2;
	float4 Position3D            : TEXCOORD3;
};

struct SScenePixelToFrame
{
    float4 Color : COLOR0;
};

float3 xLightPos;
float xLightPower;
float xAmbient;

float4x4 xWorld;
float4x4 xWorldViewProjection;
Texture xTexture;

float4x4 xLightsWorldViewProjection;

sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xShadowMap;

sampler ShadowMapSampler = sampler_state { texture = <xShadowMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};


VertexToPixel SimplestVertexShader( float4 inPos : POSITION0, float3 inNormal: NORMAL0, float2 inTexCoords : TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;
    
    Output.Position = mul(inPos, xWorldViewProjection);
    Output.TexCoords = inTexCoords;
	Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));    
	Output.Position3D = mul(inPos, xWorld);

    return Output;
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}

PixelToFrame OurFirstPixelShader(VertexToPixel PSIn)
{
	PSIn.TexCoords.y--;
    PixelToFrame Output = (PixelToFrame)0;    

	float diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
	diffuseLightingFactor = saturate(diffuseLightingFactor);
	diffuseLightingFactor *= xLightPower;

    //Output.Color = diffuseLightingFactor; //tex2D(TextureSampler, PSIn.TexCoords);

	float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
	Output.Color = baseColor*(diffuseLightingFactor + xAmbient);


    return Output;
}



technique SimplestTex
{
    pass Pass0
    {
        VertexShader = compile vs_4_0_level_9_3 SimplestVertexShader();
        PixelShader = compile ps_4_0_level_9_3 OurFirstPixelShader();
    }
}


SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
{
    SMapVertexToPixel Output = (SMapVertexToPixel)0;

    Output.Position = mul(inPos, xLightsWorldViewProjection);
    Output.Position2D = Output.Position;

    return Output;
}


SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
{
    SMapPixelToFrame Output = (SMapPixelToFrame)0;            

    Output.Color = PSIn.Position2D.z/PSIn.Position2D.w;

    return Output;
}

technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_4_0_level_9_3 ShadowMapVertexShader();
        PixelShader = compile ps_4_0_level_9_3 ShadowMapPixelShader();
    }
}


SSceneVertexToPixel ShadowedSceneVertexShader( float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL)
{
    SSceneVertexToPixel Output = (SSceneVertexToPixel)0;

    Output.Position = mul(inPos, xWorldViewProjection);    
    Output.Pos2DAsSeenByLight = mul(inPos, xLightsWorldViewProjection);    
    Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));    
    Output.Position3D = mul(inPos, xWorld);
    Output.TexCoords = inTexCoords;    

    return Output;
}


SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame)0;    
 
     float2 ProjectedTexCoords;
     ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
     ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
     
     float diffuseLightingFactor = 0;
     if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
     {
         float depthStoredInShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;
         float realDistance = PSIn.Pos2DAsSeenByLight.z/PSIn.Pos2DAsSeenByLight.w;
         if ((realDistance - 1.0f/100.0f) <= depthStoredInShadowMap)
         {
             diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
             diffuseLightingFactor = saturate(diffuseLightingFactor);
             diffuseLightingFactor *= xLightPower;            
         }
     }
         
     float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);                
     Output.Color = baseColor*(diffuseLightingFactor + xAmbient);
 
     return Output;
}

technique ShadowedScene
{
    pass Pass0
    {
        VertexShader = compile vs_4_0_level_9_3 ShadowedSceneVertexShader();
        PixelShader = compile ps_4_0_level_9_3 ShadowedScenePixelShader();
    }
}