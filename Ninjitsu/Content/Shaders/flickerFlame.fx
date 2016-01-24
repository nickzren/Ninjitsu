float blendAmount = .33f;
float2 blendTexturePosition;
float2 blendTexturePosition2;

texture ScreenTexture;    
sampler ScreenS = sampler_state 
{ 
	texture = <ScreenTexture>; 
	AddressU	= CLAMP;	
	AddressV	= CLAMP;	
	AddressW	= CLAMP; 
	MIPFILTER	= POINT;	
	MINFILTER	= POINT;	
	MAGFILTER	= POINT;
};

texture bloodTexture;
sampler ScreenS2 = sampler_state 
{ 
	texture = <bloodTexture>; 
	AddressU	= WRAP;	
	AddressV	= WRAP;	
	AddressW	= WRAP; 
	MIPFILTER	= POINT;	
	MINFILTER	= POINT;	
	MAGFILTER	= POINT;
};

texture flameTexture;
sampler ScreenS3 = sampler_state 
{ 
	texture = <flameTexture>; 
	AddressU	= WRAP;	
	AddressV	= WRAP;	
	AddressW	= WRAP; 
	MIPFILTER	= POINT;	
	MINFILTER	= POINT;	
	MAGFILTER	= POINT;
};

struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
};



// -----------------------------------------------------------------------------------
float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 color =  tex2D(ScreenS, texCoord); 
    float4 color2 = tex2D(ScreenS2, texCoord + blendTexturePosition); 
    float4 color3 = tex2D(ScreenS3, texCoord + blendTexturePosition2); 
    
    // Blend
    if (color.a > 0)
    {
		color.r = 1.0f;
		color.g = 0.7f;
		color.b = 0.0f;
	    color = (color*.50f + color2*.25f + color3*.25f);
    }
    
    return color;
}

// Region 5:
//   Technique Setup
// --------------------------------------------------------------------------
technique
{
    pass P0
    {
//        VertexShader = compile vs_2_0 VertexShader();
        PixelShader = compile ps_2_0 PixelShader();
        
        AlphaBlendEnable = true; SrcBlend = SrcAlpha; DestBlend = InvSrcAlpha;
		FillMode = Solid;

    }
}

// end - Thanks for playing! ;)
