


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
  public FgCassetteVisuals(CassetteMaterialLayer.CassetteMaterialFormat format, string ch){
    f=format;
    channel=ch;
    info = new(true, f.depth);
    resources.enable();
    layers.Add(channel,this);
  }
  public List<Template> templates=new();
  public List<Entity> todraw;
  public bool dirty=true;
  string channel;
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
    MaterialPipe.gd.SetRenderTarget(outtex);
    MaterialPipe.gd.Clear(Color.Transparent);
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, shader, c.Matrix);
    foreach(Entity e in todraw) if(e.Scene!=null)e.Render();
    sb.End();
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
    return todraw.Count>0;
  }
  public void onRemove(){
    if(layers.TryGetValue(channel, out var l) && l==this) layers.Remove(channel);
    templates.Clear();
    todraw.Clear();
  }
  static Effect shader;
  static Dictionary<string, FgCassetteVisuals> layers = new();
  static HookManager resources = new (()=>{
    shader = auspicioushelperGFX.LoadEffect("cassette/foreground");
  },()=>{
    foreach(var pair in layers){
      List<Template> keep = new List<Template>();
      foreach(Template e in pair.Value.templates){
        if(e.Scene!=null)keep.Add(e);
      }
      pair.Value.templates = keep;
      pair.Value.dirty = true;
    }
    return false;
  },auspicioushelperModule.OnReset);
}