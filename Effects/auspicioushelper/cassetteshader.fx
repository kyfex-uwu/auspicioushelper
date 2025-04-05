sampler2D TextureSamp : register(s0);
texture2D materialTex : register(t1);
sampler2D materialSamp : register(s1);

uniform float4 edgecol;
uniform float4 lowcol;
uniform float4 highcol;
uniform float4 pattern;

uniform float2 cpos;
uniform float time;
uniform float stripecutoff;

float4 valAt(float2 pos, float offsetx, float offsety){
    return tex2D(materialSamp,float2(pos.x+offsetx/320.,pos.y+offsety/180.));
}

float2 worldpos(float2 pos){
    return floor(pos*float2(320.,180.)+cpos);
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
  bool farcenters = valAt(pos,2,0).a<alphacutoff || valAt(pos,-2,0).a<alphacutoff ||
                    valAt(pos, 0,2).a<alphacutoff || valAt(pos, 0,-2).a<alphacutoff;
  
  float4 v = float4(wpos, time, 1);
  float sv = cos(dot(v,pattern));
  if(sv>stripecutoff || centers || diags || farcenters){
    return edgecol;
  }
  return matval*highcol+(1-matval)*lowcol;
}

technique BasicTech {
    pass Pass0 {
        PixelShader = compile ps_3_0 main();
    }
}

