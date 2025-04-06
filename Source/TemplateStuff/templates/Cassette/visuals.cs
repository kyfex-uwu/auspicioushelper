



using System;
using System.Collections.Generic;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class CassetteMaterialLayer:IMaterialLayer{
  public bool independent{get;} = true;
  public bool diddraw {get;set;}
  public bool removeNext {get;set;}
  public bool enabled {get;set;}
  public RenderTarget2D outtex {get; private set;}
  RenderTarget2D mattex;
  public float depth {get;set;}
  public CassetteMaterialFormat format;
  string channel;
  public struct CassetteMaterialFormat{
    public Color border = Util.hexToColor("fff");
    public Color innerlow = Util.hexToColor("333");
    public Color innerhigh = Util.hexToColor("aaa");
    public Vector4 patternvec = new Vector4(0.5f,0.5f,0,0);
    public float alphacutoff = 0.1f;
    public float stripecutoff = 0f;
    public float depth = 9000;
    public CassetteMaterialFormat(){}

    public static CassetteMaterialFormat fromDict(Dictionary<string,string> dict){
      CassetteMaterialFormat c = new CassetteMaterialFormat();
      foreach(var pair in dict){
        switch(pair.Key){
          case "border": c.border=Util.hexToColor(pair.Value.Trim()); break;
          case "innerlow": c.innerlow=Util.hexToColor(pair.Value.Trim()); break;
          case "innerhigh": c.innerhigh=Util.hexToColor(pair.Value.Trim()); break;
          case "color":
            Color h =Util.hexToColor(pair.Value.Trim());
            c.border = h; 
            c.innerhigh = new Color(0.5f*h.ToVector4()); 
            c.innerlow = new Color(0.3f*h.ToVector4());
          break;
          case "x": c.patternvec.X=float.Parse(pair.Value); break;
          case "y": c.patternvec.Y=float.Parse(pair.Value); break;
          case "time": c.patternvec.Z=float.Parse(pair.Value); break;
          case "phase": c.patternvec.W=float.Parse(pair.Value); break;
          case "depth":c.depth = float.Parse(pair.Value); break;
          case "alphacutoff":c.alphacutoff = float.Parse(pair.Value); break;
          case "stripecutoff":c.stripecutoff = float.Parse(pair.Value); break;
        }
      }
      return c;
    }
    public int gethash(){
      return HashCode.Combine(border, innerlow, innerhigh, patternvec, alphacutoff, stripecutoff, depth);
    }
  }
  static Effect shader;
  public static Dictionary<string, CassetteMaterialLayer> layers = new Dictionary<string,CassetteMaterialLayer>();
  
  public CassetteMaterialLayer(CassetteMaterialFormat format, string channel){
    this.channel = channel;
    this.depth = format.depth;
    this.format = format;
    outtex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    mattex = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    resources.enable();
  }
  List<Entity> items = new List<Entity>();
  bool dirty;
  public void render(Camera c, SpriteBatch sb, RenderTarget2D back){
    if(dirty){
      items.Sort(EntityList.CompareDepth);
      dirty = false;
    }
    EffectParameterCollection prm = shader.Parameters;
    prm["edgecol"]?.SetValue(format.border.ToVector4());
    prm["lowcol"]?.SetValue(format.innerlow.ToVector4());
    prm["highcol"]?.SetValue(format.innerhigh.ToVector4());
    prm["pattern"]?.SetValue(format.patternvec);
    prm["cpos"]?.SetValue(c.Position);
    prm["time"]?.SetValue((Engine.Scene as Level)?.TimeActive??0);
    prm["stripecutoff"]?.SetValue(format.stripecutoff);
    MaterialPipe.gd.SetRenderTarget(mattex);
    MaterialPipe.gd.Clear(Color.Transparent);
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, c.Matrix);
    if(ChannelState.readChannel(channel) == 0)foreach(Entity e in items){
      if(e.Scene != null && e.Depth<=depth) e.Render();
    }
    foreach(IMaterialObject e in trying){
      e.renderMaterial(this, sb, c);
    }
    sb.End();
    MaterialPipe.gd.Textures[1] = mattex;
    MaterialPipe.gd.SetRenderTarget(outtex);
    MaterialPipe.gd.Clear(Color.Transparent);
    sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Matrix.Identity);
    sb.Draw(mattex,Vector2.Zero,Color.White);
    sb.End();
    diddraw = true;
  }
  public bool checkdo(){
    bool drawnormal = ChannelState.readChannel(channel) == 0;
    if(drawnormal) trying.Clear();
    return drawnormal || trying.Count>0;
  }
  public void onRemove(){
    if(layers.TryGetValue(channel, out var l) && l==this) layers.Remove(channel);
  }
  public void dump(List<Entity> l){
    foreach(Entity e in l){
      items.Add(e);
    }
    dirty=true;
  }
  public HashSet<IMaterialObject> trying = new HashSet<IMaterialObject>();
  public void addTrying(IMaterialObject o){
    trying.Add(o);
  }
  public void removeTrying(IMaterialObject o){
    trying.Remove(o);
  }
  static HookManager resources = new HookManager(()=>{
    //totally a hook (this is a good api)
    shader = auspicioushelperGFX.LoadEffect("cassetteshader");
  },bool ()=>{
    foreach(var pair in layers){
      List<Entity> keep = new List<Entity>();
      foreach(Entity e in pair.Value.items){
        if(e.Scene!=null)keep.Add(e);
      }
      pair.Value.items = keep;
      pair.Value.trying.Clear();
    }
    return false;
  },auspicioushelperModule.OnReset);
}