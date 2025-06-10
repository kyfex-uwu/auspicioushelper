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

  bool centers = valAt(pos,1,0).a<alphacutoff || valAt(pos,-1,0).a<alphacutoff ||
                 valAt(pos, 0,1).a<alphacutoff || valAt(pos, 0,-1).a<alphacutoff;
  bool diags = valAt(pos, 1,1).a<alphacutoff || valAt(pos, 1,-1).a<alphacutoff ||
               valAt(pos,-1,-1).a<alphacutoff || valAt(pos, -1,1).a<alphacutoff;
  
  
  float4 darkcol = matval*highcol+(1-matval)*lowcol;
  if(centers || diags){
    return edgecol;
  }
  float4 v = float4(wpos+float2(0.5f,0.5f), time, 1);
  float sv = cos(dot(v,pattern));
  bool farcenters = valAt(pos,2,0).a<alphacutoff || valAt(pos,-2,0).a<alphacutoff ||
                    valAt(pos, 0,2).a<alphacutoff || valAt(pos, 0,-2).a<alphacutoff;
  if(farcenters || sv>stripecutoff){
    return edgecol;
  }
  return darkcol;
}

technique BasicTech {
    pass Pass0 {
        PixelShader = compile ps_3_0 main();
    }
}

