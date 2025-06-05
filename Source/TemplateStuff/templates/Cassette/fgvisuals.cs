


using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

public class FgCassetteVisuals:IMaterialLayerSimple{
  public MaterialLayerInfo info{get;}
  public RenderTarget2D outtex{get;}
  CassetteMaterialLayer.CassetteMaterialFormat f;
  public FgCassetteVisuals(CassetteMaterialLayer.CassetteMaterialFormat format){
    f=format;
    info = new(true, f.depth);
    outtex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    resources.enable();
  }
  public HashSet<Template> templates=new();
  public List<Entity> todraw=new();
  public bool dirty=true;
  public void render(Camera c, SpriteBatch sb, RenderTarget2D back){
    if(dirty){
      todraw.Clear();
      foreach(Template t in templates) t.AddAllChildren(todraw);
      todraw.Sort(EntityList.CompareDepth);
      dirty = false;
    }
    EffectParameterCollection prm = shader.Parameters;
    prm["highcol"]?.SetValue(f.fghigh.ToVector4());
    prm["lowcol"]?.SetValue(f.fglow.ToVector4());
    prm["fgsat"]?.SetValue(f.fgsat);
    MaterialPipe.gd.SetRenderTarget(outtex);
    MaterialPipe.gd.Clear(Color.Transparent);
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, shader, c.Matrix);
    foreach(Entity e in todraw) if(e.Scene!=null)e.Render();
    sb.End();
    info.diddraw=true;
  }
  public void Add(Template t){
    dirty = true;
    templates.Add(t);
  }
  public void Remove(Template t){
    dirty = true;
    templates.Remove(t);
  }
  public bool checkdo(){
    return templates.Count>0;
  }
  public void onRemove(){
    templates.Clear();
    todraw.Clear();
  }
  public void Clean(){
    HashSet<Template> keep = new();
    foreach(Template e in templates){
      if(e.Scene!=null)keep.Add(e);
    }
    templates = keep;
    dirty = true;
  }
  static Effect shader;
  static HookManager resources = new (()=>{
    shader = auspicioushelperGFX.LoadEffect("cassette/fgtint");
  },()=>{});
}