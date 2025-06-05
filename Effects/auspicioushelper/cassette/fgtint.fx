sampler2D TextureSampler : register(s0);

uniform float4 highcol;
uniform float4 lowcol;
uniform float fgsat;

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
    float4 orig = tex2D(TextureSampler, texCoord)*color;
    if(orig.a<0.5) return float4(0,0,0,0);
    float3 remapped = orig.rgb*(highcol.rgb-lowcol.rgb)+lowcol.rgb;
    float lum = dot(orig.rgb, float3(0.3,0.55,0.15));
    float3 bnw = highcol*lum+lowcol*(1-lum);
    return float4(fgsat*remapped+(1-fgsat)*bnw, 1);
}
technique BasicTech {
    pass Pass0 {
        PixelShader = compile ps_3_0 main();
    }
}
