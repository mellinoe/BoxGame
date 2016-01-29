cbuffer MatrixBuffer : register(b0)
{
    float4x4 world;
    float4x4 view;
    float4x4 projection;
}

cbuffer LightBuffer : register(b0)
{
    float4 diffuseColor;
    float3 lightDirection;
}

cbuffer AmbientBuffer : register(b1)
{
    float4 ambientColor;
}

struct VertexInput
{
    float4 position : POSITION;
    float3 normal : NORMAL;
    float4 color : COLOR;
    float2 texCoord : TEXCOORD0;
};

struct PixelInput
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
    float4 color : COLOR;
    float2 texCoord : TEXCOORD0;
};

PixelInput VS(VertexInput input)
{
    PixelInput output = (PixelInput)0;

    input.position.w = 1;

    float4 worldPosition = mul(input.position, world);
    float4 viewPosition = mul(worldPosition, view);
    output.position = mul(viewPosition, projection);
    output.color = input.color;

    output.normal = mul(input.normal, (float3x3)world);
    output.normal = normalize(output.normal);

    output.texCoord = input.texCoord;

    return output;
}

/////////////
// GLOBALS //
/////////////
Texture2D shaderTexture;
SamplerState MeshTextureSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    MaxLOD = 0.0f;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 PS(PixelInput input) : SV_Target
{
    float4 color = shaderTexture.Sample(MeshTextureSampler, input.texCoord);
    float3 lightDir = -normalize(lightDirection).xyz;
    float effectiveness = dot(input.normal, lightDir);
    float lightEffectiveness = saturate(effectiveness);
    float4 lightColor = saturate(diffuseColor * lightEffectiveness);
    return saturate((lightColor * color) + (ambientColor * color));
}