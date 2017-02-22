#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#include "d2d1effecthelpers.hlsli"

float2 textureSize;

const static float3 dtt = float3(65536, 255, 1);

float reduce(float3 color)
{
    return dot(color, dtt);
}

D2D_PS_ENTRY(main)
{
    float2 texCoord = D2DGetInputCoordinate(0).xy / 5.0;

    float2 fp = frac(texCoord * textureSize);

    float2 ps = 1.0 / textureSize;
    float dx = ps.x;
    float dy = ps.y;

    float2 t1xy = float2(0, -dy);
    float2 t1zw = float2(-dx, 0);

    float2 g1 = t1xy * (step(0.5, fp.x) + step(0.5, fp.y) - 1) + t1zw * (step(0.5, fp.x) - step(0.5, fp.y));
    float2 g2 = t1xy * (step(0.5, fp.y) - step(0.5, fp.x)) + t1zw * (step(0.5, fp.x) + step(0.5, fp.y) - 1);


    float3 B = D2DSampleInput(0, texCoord + g1).xyz;
    float3 C = D2DSampleInput(0, texCoord + g1 - g2).xyz;
    float3 D = D2DSampleInput(0, texCoord + g2).xyz;
    float3 E = D2DSampleInput(0, texCoord).xyz;
    float3 F = D2DSampleInput(0, texCoord - g2).xyz;
    float3 G = D2DSampleInput(0, texCoord - g1 + g2).xyz;
    float3 H = D2DSampleInput(0, texCoord - g1).xyz;
    float3 I = D2DSampleInput(0, texCoord - g1 - g2).xyz;

    float3 E14 = E;
    float3 E19 = E;
    float3 E24 = E;

    float b = reduce(B);
    float c = reduce(C);
    float d = reduce(D);
    float e = reduce(E);
    float f = reduce(F);
    float g = reduce(G);
    float h = reduce(H);
    float i = reduce(I);


    if (h == f && h != e && (e == g && (h == i || e == d) || e == c && (h == i || e == b)))
    {
        E24 = F;
        E19 = lerp(E19, F, 0.875);
        E14 = lerp(E14, F, 0.125);
    }

    float3 res;

    res = (fp.x < 0.4) ? ((fp.x < 0.2) ? ((fp.y < 0.2) ? E24 : (fp.y < 0.4) ? E19 : (fp.y < 0.6) ? E14 : (fp.y < 0.8) ? E19 : E24) : ((fp.y < 0.2) ? E19 : (fp.y < 0.4) ? E14 : (fp.y < 0.6) ? E : (fp.y < 0.8) ? E14 : E19)) : ((fp.x < 0.8) ? ((fp.x < 0.6) ? ((fp.y < 0.2) ? E14 : (fp.y < 0.4) ? E : (fp.y < 0.6) ? E : (fp.y < 0.8) ? E : E14) : ((fp.y < 0.2) ? E19 : (fp.y < 0.4) ? E14 : (fp.y < 0.6) ? E : (fp.y < 0.8) ? E14 : E19)) : ((fp.y < 0.2) ? E24 : (fp.y < 0.4) ? E19 : (fp.y < 0.6) ? E14 : (fp.y < 0.8) ? E19 : E24));

    return float4(res, 1.0);
}
