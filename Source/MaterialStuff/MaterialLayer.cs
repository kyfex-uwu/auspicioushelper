


using Monocle;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Celeleste.Mods.auspicioushelper;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mods.auspicioushelper;

public class MaterialLayer{
  public bool independent;
  public float depth;
  public RenderTarget2D mattex;
  public RenderTarget2D outtex;
  public List<IMaterialObject> willDraw = new List<IMaterialObject>();
  public Effect shader;
  public bool both;
  public bool always;
  public bool diddraw;
  public MaterialLayer(float _depth, Effect outshader = null, bool _independent = true, bool outonly = false, bool alwaysdraw=false){
    outtex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    if(!outonly){
      mattex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    }
    shader=outshader;
    both=!outonly;
    always = alwaysdraw;
    depth = _depth;
    diddraw=false;
    independent = _independent;
  }
  public void planDraw(IMaterialObject obj){
    willDraw.Add(obj);
  }
  public virtual void render(Camera c, SpriteBatch sb){
    render(c, sb, null);
  }
  public virtual void rasterMats(SpriteBatch sb, Camera c){
    foreach(IMaterialObject e in willDraw){
      e.renderMaterial(this, sb, c);
    }
  }
  public virtual void render(Camera c, SpriteBatch sb, RenderTarget2D back){
    //DebugConsole.Write("Rendering layer");
    if(back != null){
      MaterialPipe.gd.Textures[2]=back;
    }
    EffectParameter timeUniform = shader.Parameters["time"];
    if(timeUniform != null){
      timeUniform.SetValue((Engine.Scene as Level).TimeActive);
    } 
    EffectParameter camPosUniform = shader.Parameters["cpos"];
    if(camPosUniform != null){
      camPosUniform.SetValue(c.Position);
    } 

    MaterialPipe.gd.SetRenderTarget(both?mattex:outtex);
    MaterialPipe.gd.Clear(Color.Transparent);
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, c.Matrix);
    rasterMats(sb,c);
    sb.End();
    if(both){
      //DebugConsole.Write("doing deferred shader");
      MaterialPipe.gd.Textures[1] = mattex;
      MaterialPipe.gd.SetRenderTarget(outtex);
      MaterialPipe.gd.Clear(Color.Transparent);
      sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Matrix.Identity);
      sb.Draw(mattex,Vector2.Zero,Color.White);
      sb.End();
    }
    willDraw.Clear();
    diddraw = true;
  }
  public virtual bool checkdo(){
    return always || willDraw.Count>0 || diddraw;
  }
}