


using System;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mods.auspicioushelper;

internal class UserLayer:BasicMaterialLayer{
  internal static UserLayer make(EntityData d){
    var eff = auspicioushelperGFX.LoadExternEffect(d.Attr("path"));
    if(eff != null){
      return new UserLayer(d,eff);
    } else {
      return null;
    }
  }
  bool uselast = false;
  RenderTarget2D prev = null;
  public UserLayer(EntityData d, Tuple<Effect,Effect> effects):base(
    d.Int("depth",0),effects.Item1,d.Bool("independent"),d.Bool("simple"),d.Bool("always") 
  ){
    quietShader = effects.Item2;
    if(uselast = d.Bool("useprev",false)){
      prev = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    }
  }
  public override void render(Camera c, SpriteBatch sb, RenderTarget2D back){
    SamplerState origss = null;
    if(uselast){
      origss = MaterialPipe.gd.SamplerStates[4];
      prev = swapOuttex(prev);
      MaterialPipe.gd.Textures[4]=prev;
      MaterialPipe.gd.SamplerStates[4]=SamplerState.PointClamp;
    }
    base.render(c, sb, back);
    if(uselast){
      MaterialPipe.gd.SamplerStates[4]=origss;
    }
  }
}