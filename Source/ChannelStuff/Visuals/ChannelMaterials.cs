using System.Collections.Generic;
using Celeleste.Mods.auspicioushelper;
using Celeste.Mod.auspicioushelper;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

//I have become the c# abstraction-enraptured person lmao like actually what is this haha
//OK to be sort of fair I intended to do a custom rasterMats for this but decided not.
public class ChannelMaterialsA:MaterialLayer{
  public List<IMaterialObject> bgItemsDraw = new List<IMaterialObject>();
  public List<IMaterialObject> connectedBlocksDraw = new List<IMaterialObject>();
  public RenderTarget2D bgtex;
  public ChannelMaterialsA():base(-10000, auspicioushelperGFX.LoadEffect("testshader")){
    DebugConsole.Write(shader == null? "Static shader is null":"Static shader found and registered");
    if(shader == null)return;
    bgtex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    foreach (var param in shader.Parameters) {
      DebugConsole.Write($"Parameter: {param.Name}, Type: {param.ParameterType}");
    }
  }
  public override bool checkdo()
  {
    return connectedBlocksDraw.Count+willDraw.Count>0 || diddraw;
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