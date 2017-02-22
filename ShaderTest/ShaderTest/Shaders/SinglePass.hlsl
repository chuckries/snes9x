#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_COMPLEX
#define D2D_INPUT1_COMPLEX

#include "d2d1effecthelpers.hlsli"

#define trY 48.0
#define trU 7.0
#define trV 6.0

float scale;
float2 textureSize;

static float3 yuv_threshold = float3(trY / 255.0, trU / 255.0, trV / 255.0);

const static float3x3 yuv = float3x3(0.299, 0.587, 0.114, -0.169, -0.331, 0.5, 0.5, -0.419, -0.081);
const static float3 yuv_offset = half3(0, 0.5, 0.5);

bool diff(float3 yuv1, float3 yuv2) {
    bool3 res = abs((yuv1 + yuv_offset) - (yuv2 + yuv_offset)) > yuv_threshold;
    return res.x || res.y || res.z;
}

D2D_PS_ENTRY(main)
{
    // determine the scale and get our actual coordinate within the texture
    float2 texCoord = D2DGetInputCoordinate(0).xy / scale;

    // determine which quadrant we are in
    float2 fp = frac(texCoord * textureSize);
    float2 quad = sign(-0.5 + fp);

    // determine our deltas within the texture
    float dx = 1.0 / textureSize.x;
    float dy = 1.0 / textureSize.y;

    // determine the pixels we care about
    float3 p1 = D2DSampleInput(0, texCoord).rgb;
    float3 p2 = D2DSampleInput(0, texCoord + float2(dx, dy) * quad).rgb;
    float3 p3 = D2DSampleInput(0, texCoord + float2(dx, 0) * quad).rgb;
    float3 p4 = D2DSampleInput(0, texCoord + float2(0, dy) * quad).rgb;
    float4x3 pixels = float4x3(p1, p2, p3, p4);

    // calculate our weights
    float3 w1 = mul(yuv, D2DSampleInput(0, texCoord + float2(-dx, -dy)).rgb);
    float3 w2 = mul(yuv, D2DSampleInput(0, texCoord + float2(0, -dy)).rgb);
    float3 w3 = mul(yuv, D2DSampleInput(0, texCoord + float2(dx, -dy)).rgb);

    float3 w4 = mul(yuv, D2DSampleInput(0, texCoord + float2(-dx, 0)).rgb);
    float3 w5 = mul(yuv, p1);
    float3 w6 = mul(yuv, D2DSampleInput(0, texCoord + float2(dx, 0)).rgb);

    float3 w7 = mul(yuv, D2DSampleInput(0, texCoord + float2(-dx, dy)).rgb);
    float3 w8 = mul(yuv, D2DSampleInput(0, texCoord + float2(0, dy)).rgb);
    float3 w9 = mul(yuv, D2DSampleInput(0, texCoord + float2(dx, dy)).rgb);

    // determine the pattern based on weights
    bool3x3 pattern = bool3x3(diff(w5, w1), diff(w5, w2), diff(w5, w3),
        diff(w5, w4), false, diff(w5, w6),
        diff(w5, w7), diff(w5, w8), diff(w5, w9));
    bool4 cross = bool4(diff(w4, w2), diff(w2, w6), diff(w8, w4), diff(w6, w8));

    // math I don't understand
    float2 index;
    index.x = dot(pattern[0], float3(1, 2, 4)) +
        dot(pattern[1], float3(8, 0, 16)) +
        dot(pattern[2], float3(32, 64, 128));

    index.y = dot(cross, float4(1, 2, 4, 8)) * (scale * scale) +
        dot(floor(fp * scale), float2(1, scale));

    // Lookup table and result
    float2 step = 1.0 / float2(256.0, 16.0 * (scale * scale));
    float2 offset = step / 2.0;
    float4 weights = D2DSampleInput(1, index * step + offset);
    //float4 weights = tex2D(LUT, index * step + offset);
    float sum = dot(weights, float4(1, 1, 1, 1));
    float3 res = mul(transpose(pixels), weights / sum);

    return float4(res, 1.0);
}