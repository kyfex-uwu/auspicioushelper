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
	float2 wpos = worldpos(pos);
	
	if(matval.a == 0) return float4(0,0,0,0);
  float alphacutoff = 0.2;
  
  
  float4 darkcol = matval*highcol+(1-matval)*lowcol;
  float4 v = float4(wpos+float2(0.5f,0.5f), time, 1);
  float sv = cos(dot(v,pattern));

  float d1 = min(valAt(pos,0,0).x,min(valAt(pos,0,1).x, valAt(pos,0,-1).x));
  if(d1 <0.15){
    return edgecol;
  }
  float d2 = min(d1, min(valAt(pos,0,2).x,valAt(pos,0,-2).x));
  if(d2<0.25){
    return darkcol;
  }
  float d3 = min(d2, min(valAt(pos,0,3).x,valAt(pos,0,-3).x));
  if(d3<0.35 || sv>stripecutoff){
    return edgecol;
  }
  return darkcol;
}

technique BasicTech {
    pass Pass0 {
        PixelShader = compile ps_3_0 main();
    }
}

