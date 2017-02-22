#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#include "d2d1effecthelpers.hlsli"

#define trY 48.0
#define trU 7.0
#define trV 6.0

static float3 yuv_threshold = float3(trY / 255.0, trU / 255.0, trV / 255.0);

const static float3x3 yuv = float3x3(0.299, 0.587, 0.114, -0.169, -0.331, 0.5, 0.5, -0.419, -0.081);
const static float3 yuv_offset = float3(0, 0.5, 0.5);

bool diff(float3 yuv1, float3 yuv2) {
    bool3 res = abs((yuv1 + yuv_offset) - (yuv2 + yuv_offset)) > yuv_threshold;
    return res.x || res.y || res.z;
}

D2D_PS_ENTRY(main)
{
    float3 w1 = mul(yuv, D2DSampleInputAtOffset(0, float2(-1.0f, -1.0f)).rgb);
    float3 w2 = mul(yuv, D2DSampleInputAtOffset(0, float2( 0.0f, -1.0f)).rgb);
    float3 w3 = mul(yuv, D2DSampleInputAtOffset(0, float2( 1.0f, -1.0f)).rgb);
    float3 w4 = mul(yuv, D2DSampleInputAtOffset(0, float2(-1.0f,  0.0f)).rgb);
    float3 w5 = mul(yuv, D2DGetInput(0).rgb);
    float3 w6 = mul(yuv, D2DSampleInputAtOffset(0, float2( 1.0f,  0.0f)).rgb);
    float3 w7 = mul(yuv, D2DSampleInputAtOffset(0, float2(-1.0f,  1.0f)).rgb);
    float3 w8 = mul(yuv, D2DSampleInputAtOffset(0, float2( 0.0f,  1.0f)).rgb);
    float3 w9 = mul(yuv, D2DSampleInputAtOffset(0, float2( 1.0f,  1.0f)).rgb);

    bool3x3 pattern = bool3x3(diff(w5, w1), diff(w5, w2), diff(w5, w3),
                              diff(w5, w4), false,        diff(w5, w6),
                              diff(w5, w7), diff(w5, w8), diff(w5, w9));
    bool4 cross = bool4(diff(w4, w2), diff(w2, w6), diff(w8, w4), diff(w6, w8));

    float2 index;
    index.x = dot(pattern[0], float3(1, 2, 4)) +
        dot(pattern[1], float3(8, 0, 16)) +
        dot(pattern[2], float3(32, 64, 128));
    index.y = dot(cross, float4(1, 2, 4, 8));

    return float4(index / float2(255.0, 15.0), 0.0, 1.0);
}
