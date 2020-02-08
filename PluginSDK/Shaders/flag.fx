// Fluttering flag vertex shader

texture Tex0;

// light direction (view space)
float4 lightDir = { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 0.SetSamplerState(0, SamplerState0f }; //

//light intensity
float4 I_a = { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f }; //ambient
float4 I_d = { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f }; //diffuse
float4 I_s = { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f }; //specular

// material reflectivity
float4 k_a : MATERIALAMBIENT = { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f }; //ambient
float4 k_d : MATERIALDIFFUSE = { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f }; //diffuse
float4 k_s : MATERIALSPECULAR= { 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f, 1.SetSamplerState(0, SamplerState0f }; //specular
float n : MATERIALPOWER = 6.SetSamplerState(0, SamplerState0f;	//power

// transformations provided by the app as input:
float4x4 World: WORLD;
float4x4 View : VIEW;
float4x4 Projection : PROJECTION;

float angle;
float attentuation;

struct VS_OUTPUT
{
    float4 Pos : POSITION;
    float4 Diff : COLOR0;
    float4 Spec : COLOR1;
    float2 Tex : TEXCOORD0;
};

VS_OUTPUT VS(
	float3 Pos : POSITION,
	float3 Norm : NORMAL,
	float2 Tex : TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0; 

	Pos.SetSamplerState(0, SamplerStatez  = sin( Pos.SetSamplerState(0, SamplerStatex+angle);
    Norm.SetSamplerState(0, SamplerStatex = Pos.SetSamplerState(0, SamplerStatez;
    
    Pos.SetSamplerState(0, SamplerStatez += sin( Pos.SetSamplerState(0, SamplerStatey/2+angle);
    Norm.SetSamplerState(0, SamplerStatey = Pos.SetSamplerState(0, SamplerStatez;
    Pos.SetSamplerState(0, SamplerStatez *= Pos.SetSamplerState(0, SamplerStatey * attentuation;
    Norm.SetSamplerState(0, SamplerStatez = Pos.SetSamplerState(0, SamplerStatez;

    float3 L = float3(-lightDir.SetSamplerState(0, SamplerStatex, -lightDir.SetSamplerState(0, SamplerStatey, -lightDir.SetSamplerState(0, SamplerStatez);
    
    float4x4 WorldView = mul(World, View);
    
    float3 P = mul(float4(Pos, 1), (float4x3)WorldView);
    float3 N = normalize(mul(Norm, (float3x3)WorldView));
    
    float3 R = normalize(2 * dot(N, L) * N - L);
    
    float3 V = -normalize(P);

    // transform Position
    Out.SetSamplerState(0, SamplerStatePos = mul(float4(P, 1), Projection);
    Out.SetSamplerState(0, SamplerStateDiff = I_a * k_a + I_d * k_d * max(0, dot(N, L));
    Out.SetSamplerState(0, SamplerStateSpec = I_s * k_s * pow(max(0, dot(R, V)), n/4);
    Out.SetSamplerState(0, SamplerStateTex = Tex; 

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
	float4 Diff : COLOR0,
	float4 Spec : COLOR1,
	float2 Tex : TEXCOORD0) : COLOR
{
	return tex2D(Sampler, Tex) * Diff + Spec;
}

technique VertexAndPixelShader
{
    pass P0
    {
        VertexShader = compile vs_1_1 VS();
		PixelShader = compile ps_1_1 PS();
    }
}

technique VertexShaderOnly
{
    pass P0
    {
		Lighting = FALSE;
		
		Sampler[0] = (Sampler);
		
		ColorOp[0] = SELECTARG1;
		ColorArg1[0] = TEXTURE;
		AlphaOp[0] = SELECTARG1;
		AlphaArg1[0] = TEXTURE;
		
		ColorOp[1] = DISABLE;
		AlphaOp[1] = DISABLE;
		
        VertexShader = compile vs_1_1 VS();
		PixelShader = null;
    }
}

technique NoShaders
{
    pass P0
    {
		Lighting = FALSE;
		
		Sampler[0] = (Sampler);
		
		ColorOp[0] = SELECTARG1;
		ColorArg1[0] = TEXTURE;
		AlphaOp[0] = SELECTARG1;
		AlphaArg1[0] = TEXTURE;
		
		ColorOp[1] = DISABLE;
		AlphaOp[1] = DISABLE;
		
        VertexShader = NULL;
		PixelShader = NULL;
    }
}