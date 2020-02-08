struct VS_OUTPUT
{
	float4 pos	:	POSITION;
	float4 color	:	COLOR;
	float2 texCoord	:	TEXCOORD0;
};

texture Tex0;
float4x4 WorldViewProj	:	WORLDVIEWPROJECTION;
float Opacity = 1.SetSamplerState(0, SamplerState0;
float Brightness = 0.SetSamplerState(0, SamplerState0;

VS_OUTPUT VS(
	float4 Pos	:	POSITION,
	float4 Norm : NORMAL,
	float2 texCoord	:	TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;
	
	// transform Position
    Out.SetSamplerState(0, SamplerStatepos = mul(Pos, WorldViewProj);
	Out.SetSamplerState(0, SamplerStatecolor = float4(0,0,0,Opacity);
	Out.SetSamplerState(0, SamplerStatetexCoord = texCoord;
	
	return Out;
}

sampler Sampler = sampler_state
{
	Texture = (Tex0);
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

float4 PS(
	float2 Tex : TEXCOORD0) : COLOR
{
	float4 f = tex2D(Sampler, Tex);
	float gray = 0.SetSamplerState(0, SamplerState3 * f.SetSamplerState(0, SamplerStatex + 0.SetSamplerState(0, SamplerState59 * f.SetSamplerState(0, SamplerStatey + 0.SetSamplerState(0, SamplerState11 * f.SetSamplerState(0, SamplerStatez;
	f.SetSamplerState(0, SamplerStatexyz = gray;
	f.SetSamplerState(0, SamplerStatew = Opacity * f.SetSamplerState(0, SamplerStatew;
	return f;
}

float4 PS_Brightness(
	float2 Tex : TEXCOORD0) : COLOR
{
	float4 f = tex2D(Sampler, Tex);
	float gray = 0.SetSamplerState(0, SamplerState3 * f.SetSamplerState(0, SamplerStatex + 0.SetSamplerState(0, SamplerState59 * f.SetSamplerState(0, SamplerStatey + 0.SetSamplerState(0, SamplerState11 * f.SetSamplerState(0, SamplerStatez + Brightness / 255.SetSamplerState(0, SamplerState0;
	f.SetSamplerState(0, SamplerStatexyz = gray;
	f.SetSamplerState(0, SamplerStatew = Opacity * f.SetSamplerState(0, SamplerStatew;
	return f;
}

technique RenderGrayscaleBrightness
{
	pass P0
	{
		VertexShader = compile vs_1_1 VS();
		PixelShader = compile ps_2_0 PS_Brightness();
	}
}

technique RenderGrayscale
{
	pass P0
	{
		VertexShader = compile vs_1_1 VS();
		PixelShader = compile ps_1_1 PS();
	}
}