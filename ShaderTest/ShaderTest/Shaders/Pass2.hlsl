#define D2D_INPUT_COUNT 3
#define D2D_INPUT0_COMPLEX // output of pass1
#define D2D_INPUT1_COMPLEX // original texture
#define D2D_INPUT2_COMPLEX // LUT
#include "d2d1effecthelpers.hlsli"

float scale;
float2 textureSize;

D2D_PS_ENTRY(main)
{
    float2 texCoord = D2DGetInputCoordinate(1).xy / scale;

    float2 fp = frac(texCoord * textureSize);
    float2 quad = sign(-0.5 + fp);

    float2 ps = 1.0 / textureSize;
    float dx = ps.x;
    float dy = ps.y;

    float3 p1 = D2DSampleInput(1, texCoord).rgb;
    float3 p2 = D2DSampleInput(1, texCoord + float2(dx, dy) * quad).rgb;
    float3 p3 = D2DSampleInput(1, texCoord + float2(dx, 0) * quad).rgb;
    float3 p4 = D2DSampleInput(1, texCoord + float2(0, dy) * quad).rgb;
    float4x3 pixels = float4x3(p1, p2, p3, p4);

    float2 index = D2DSampleInput(0, texCoord).xy * float2(255.0, 15.0 * (scale * scale));
    index.y += dot(floor(fp * scale), float2(1.0, scale));

    float2 step = 1.0 / float2(256.0, 16.0 * (scale * scale));
    float2 offset = step / 2.0;
    float4 weights = D2DSampleInput(2, index * step + offset);
    float sum = dot(weights, float4(1.0, 1.0, 1.0, 1.0));
    float3 res = mul(transpose(pixels), weights / sum);

    return float4(res, 1.0);
}
