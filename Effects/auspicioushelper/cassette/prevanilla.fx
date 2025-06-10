sampler2D layerSamp : register(s0);

uniform float4 edgecol;
uniform float4 lowcol;
uniform float4 highcol;
uniform float4 pattern;
uniform float stripecutoff;

uniform float2 cpos;
uniform float2 pscale;
uniform float time;

float4 valAt(float2 pos, float offsetx, float offsety){
    return tex2D(layerSamp,pos+float2(offsetx,offsety)*pscale);
}

float2 worldpos(float2 pos){
    return floor(pos/pscale+cpos);
}





float4 main(float4 color : COLOR0, float2 pos : TEXCOORD0) : SV_Target {
	float4 matval = valAt(pos,0,0);
	
  float alphacutoff = 0.2;
	if(matval.a < alphacutoff){
    return float4(0,0,0,0);
  }

  bool d1 = valAt(pos,1,0).a<alphacutoff || valAt(pos,-1,0).a<alphacutoff;
  if(d1){
    return float4(0.1,0,0,1);
  }
  bool d2 = valAt(pos,2,0).a<alphacutoff || valAt(pos,-2,0).a<alphacutoff;
  if(d2){
    return float4(0.2,0,0,1);
  }
  bool d3 = valAt(pos,3,0).a<alphacutoff || valAt(pos,-3,0).a<alphacutoff;
  if(d3){
    return float4(0.3,0,0,1);
  }
  return float4(1,1,1,1);
}

technique BasicTech {
    pass Pass0 {
        PixelShader = compile ps_3_0 main();
    }
}

