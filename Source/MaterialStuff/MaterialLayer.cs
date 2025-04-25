


using Monocle;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Celeste.Mod;

namespace Celeste.Mods.auspicioushelper;


public interface IMaterialLayer{
  public bool enabled {get;set;}
  public bool removeNext {get;set;}
  public float depth {get;}
  public RenderTarget2D outtex {get;}
  public bool independent {get;}
  public bool diddraw {get;set;}
  public void render(Camera c, SpriteBatch sb){
    render(c,sb,null);
  }
  public void render(Camera c, SpriteBatch sb, RenderTarget2D back);
  public bool checkdo();
  public void onRemove(){}
}

public class BasicMaterialLayer:IMaterialLayer{
  public bool independent {get;set;}
  public float depth {get;set;}
  public RenderTarget2D mattex;
  public RenderTarget2D outtex {get; private set;}
  public List<IMaterialObject> willDraw = new List<IMaterialObject>();
  public Effect normalShader;
  public bool both;
  public bool always;
  public bool diddraw {get;set;}
  public bool enabled {get;set;}
  public bool removeNext {get;set;}
  public Effect quietShader = null;
  public BasicMaterialLayer(float _depth, Effect outshader = null, bool _independent = true, bool outonly = false, bool alwaysdraw=false){
    outtex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    if(!outonly){
      mattex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    }
    normalShader=outshader;
    both=!outonly;
    always = alwaysdraw;
    depth = _depth;
    diddraw=false;
    independent = _independent;
  }
  public virtual void planDraw(IMaterialObject obj){
    if(enabled)willDraw.Add(obj);
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
    Effect shader = auspicioushelperModule.Settings.UseQuietShader && quietShader!=null? quietShader:normalShader;
    if(back != null){
      MaterialPipe.gd.Textures[2]=back;
    }
    EffectParameter timeUniform = shader.Parameters["time"];
    if(timeUniform != null){
      //DebugConsole.Write((Engine.Scene as Level).TimeActive.ToString());
      timeUniform.SetValue((Engine.Scene as Level).TimeActive+2);
    } 
    EffectParameter camPosUniform = shader.Parameters["cpos"];
    if(camPosUniform != null){
      camPosUniform.SetValue(c.Position);
    } 
    EffectParameter photoSensitive = shader.Parameters["quiet"];
    if(photoSensitive != null){
      //DebugConsole.Write((Settings.Instance.DisableFlashes? 1f:0f).ToString());
      photoSensitive.SetValue(Settings.Instance.DisableFlashes? 1f:0f);
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
    return enabled && (always || willDraw.Count>0 || diddraw);
  }
  public RenderTarget2D swapOuttex(RenderTarget2D other){
    RenderTarget2D temp = outtex;
    if(other != null)outtex = other;
    return temp;
  }
}