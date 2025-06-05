sampler2D TextureSampler : register(s0);

uniform float4 highcol;
uniform float4 lowcol;

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
    float4 orig = tex2D(TextureSampler, texCoord)*color;

    float3 remapped = orig.rgb*(highcol.rgb-lowcol.rgb)+lowcol.rgb;
    return float4(remapped, orig.a);
}
technique BasicTech {
    pass Pass0 {
        PixelShader = compile ps_3_0 main();
    }
}
