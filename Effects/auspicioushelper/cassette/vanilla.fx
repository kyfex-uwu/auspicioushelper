sampler2D TextureSamp : register(s0);
texture2D materialTex : register(t1);
sampler2D materialSamp : register(s1);
texture2D sdfTex: register(t2);
sampler2D sdfSamp: register(s2);

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
float4 sdfAt(float2 pos, float offsetx, float offsety){
    return tex2D(sdfSamp,float2(pos.x+offsetx/320.,pos.y+offsety/180.));
}
float2 worldpos(float2 pos){
    return floor(pos*float2(320.,180.)+cpos);
}





float4 main(float4 color : COLOR0, float2 pos : TEXCOORD0) : SV_Target {
	float4 matval = valAt(pos,0,0);
	float2 wpos = worldpos(pos);
	
	if(matval.a == 0) return float4(0,0,0,0);
  float alphacutoff = 0.2;
  
  
  float4 darkcol = matval*highcol+(1-matval)*lowcol;
  float4 v = float4(wpos+float2(0.5f,0.5f), time, 1);
  float sv = cos(dot(v,pattern));

  float d1 = min(sdfAt(pos,0,0).x,min(sdfAt(pos,0,1).x, sdfAt(pos,0,-1).x));
  if(d1 <0.15){
    return edgecol;
  }
  float d2 = min(d1, min(sdfAt(pos,0,2).x,sdfAt(pos,0,-2).x));
  if(d2<0.25){
    return darkcol;
  }
  float d3 = min(d2, min(sdfAt(pos,0,3).x,sdfAt(pos,0,-3).x));
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

