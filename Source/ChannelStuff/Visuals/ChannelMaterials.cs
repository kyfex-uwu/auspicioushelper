using System.Collections.Generic;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;
//I have become the c# abstraction-enraptured person lmao like actually what is this haha
//OK to be sort of fair I intended to do a custom rasterMats for this but decided not.
public class ChannelMaterialsA:BasicMaterialLayer{
  private List<IMaterialObject> bgItemsDraw = new List<IMaterialObject>();
  public RenderTarget2D bgtex;
  public ChannelMaterialsA():base(-10000, auspicioushelperGFX.LoadEffect("testshader2")){
    quietShader = auspicioushelperGFX.LoadEffect("quietshader");
    bgtex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
  }
  public override bool checkdo()
  {
    return willDraw.Count>0 || diddraw;
  }
  public void planDrawBG(IMaterialObject t){
    if(enabled) bgItemsDraw.Add(t);
  }
  public override void render(Camera c, SpriteBatch sb, RenderTarget2D back){
    MaterialPipe.gd.SetRenderTarget(bgtex);
    MaterialPipe.gd.Clear(Color.Transparent);
    MaterialPipe.gd.Textures[2]=bgtex;
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, c.Matrix);
    foreach(IMaterialObject b in bgItemsDraw){
      b.renderMaterial(this, sb, c);
    }
    sb.End();
    bgItemsDraw.Clear();

    base.render(c,sb,back);
  }
}