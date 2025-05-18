



using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public interface ITemplateChild{
  Template parent {get; set;}
  Template.Propagation prop {get=>Template.Propagation.All;}
  void relposTo(Vector2 loc, Vector2 liftspeed);
  void addTo(Scene s){}
  void parentChangeStat(int vis, int col, int act);
  bool hasRiders<T>() where T : Actor{
    return false;
  }
  bool hasInside(Actor a){
    return false;
  }
  Template.Propagation propegatesTo(Template target, Template.Propagation p = Template.Propagation.All){
    ITemplateChild c=this;
    while(c != target && c!= null && p!=Template.Propagation.None){
      if(c==target) return p;
      c=c.parent;
      p&=c.parent.prop;
    }
    return Template.Propagation.None;
  }
  DashCollisionResults propegateDashHit(Player player, Vector2 direction){
    if((prop&Template.Propagation.DashHit) != Template.Propagation.None && (parent!=null)){
      if(parent.OnDashCollide != null) return parent.OnDashCollide(player, direction);
      return ((ITemplateChild)parent).propegateDashHit(player, direction);
    }
    return DashCollisionResults.NormalCollision;
  }
  void AddAllChildren(List<Entity> list);
  void setOffset(Vector2 ppos){}
  void shake(Vector2 amount){}
  void destroy(bool particles);
}

public class Template:Entity, ITemplateChild{
  public templateFiller t=null;
  public List<ITemplateChild> children = new List<ITemplateChild>();
  public int depthoffset;
  public Template parent{get;set;} = null;
  public string ownidpath;
  [Flags]
  public enum Propagation
  {
      None     = 0,      
      Riding   = 1 << 0, 
      DashHit  = 1 << 1,
      Weight   = 1 << 2,
      Shake = 1<<3,
      Inside = 1<<4,
      All = Riding|DashHit|Weight|Shake|Inside
  }
  public virtual Vector2 virtLoc=>Position;
  public Vector2 ownLiftspeed;
  public virtual Vector2 gatheredLiftspeed=>parent==null?ownLiftspeed:ownLiftspeed+parent.gatheredLiftspeed;
  public Vector2 parentLiftspeed=>parent==null?Vector2.Zero:parent.gatheredLiftspeed;
  public Propagation prop{get;set;} = Propagation.All; 
  public Vector2 toffset = Vector2.Zero;
  public Wrappers.BasicMultient basicents = null;
  public DashCollision OnDashCollide = null;
  public Template(EntityData data, Vector2 pos, int depthoffset):base(pos){
    string templateStr = data.Attr("template","");
    if(!MarkedRoomParser.templates.TryGetValue(templateStr, out t)){
      DebugConsole.Write("No template found with identifier "+templateStr);
    }
    this.depthoffset = depthoffset;
    this.Visible = false;
    Depth = 10000+depthoffset;
    this.ownidpath=getOwnID(data);
  }
  public virtual void relposTo(Vector2 loc, Vector2 liftspeed){
    Position = loc+toffset;
    childRelposTo(virtLoc, liftspeed);
  }
  public void childRelposTo(Vector2 loc, Vector2 liftspeed){
    foreach(ITemplateChild c in children){
      c.relposTo(loc, liftspeed);
    }
  }
  internal Wrappers.FgTiles fgt = null;
  public void addEnt(ITemplateChild c){
    children.Add(c);
    c.addTo(Scene);
    c.setOffset(virtLoc);
    c.parent = this;
    if(c is Template ct){
      ct.depthoffset+=depthoffset;
    }
  }
  public void setOffset(Vector2 ppos){
    this.toffset = Position-ppos;
  }
  public virtual void addTo(Scene scene){
    if(Scene != null){
      DebugConsole.Write("Something has gone terribly wrong in recursive adding process");
    }
    Scene = scene;
    if(basicents != null){
      DebugConsole.Write("Weird if this happens but nothing is actually wrong");
      basicents.sceneadd(scene);
    }
    scene.Add(this);

    if(t==null) return;
    if(t.bgt!=null) addEnt(new Wrappers.BgTiles(t,virtLoc,depthoffset));
    if(t.fgt!=null) addEnt(fgt=new Wrappers.FgTiles(t, virtLoc, depthoffset));
    Level l = SceneAs<Level>();
    Vector2 simoffset = this.virtLoc-t.origin;
    string fp = fullpath;
    foreach(EntityParser.EWrap w in t.childEntities){
      Entity e = EntityParser.create(w,l,t.roomdat,simoffset,this,fp);
      if(e is ITemplateChild c){
        addEnt(c);
      }
      else if(e!=null)scene.Add(e);
    }
    foreach(DecalData d in t.decals){
      Decal e = new Decal(d.Texture, simoffset+d.Position, d.Scale, d.GetDepth(0), d.Rotation, d.ColorHex){
        DepthSetByPlacement = true
      };
      AddBasicEnt(e, simoffset+d.Position-virtLoc);
    }
  }
  public override void Added(Scene scene){
    if(Scene == null){
      //DebugConsole.Write($"Got top-level template {this} of {t?.name}");
      addTo(scene);
    }
    base.Added(scene);
  }
  public void AddBasicEnt(Entity e, Vector2 offset){
    if(basicents == null){
      basicents = new Wrappers.BasicMultient(this);
      addEnt(basicents);
      if(Scene!=null)basicents.sceneadd(Scene);
    }
    basicents.add(e,offset);
  }
  public bool hasRiders<T>() where T:Actor{
    foreach(ITemplateChild c in children){
      if((c.prop & Propagation.Riding)!=0 && c.hasRiders<T>()) return true;
    }
    return false;
  }
  public bool hasInside(Actor a){
    foreach(ITemplateChild c in children) 
      if(((c.prop&Propagation.Inside)!=Propagation.None) && c.hasInside(a)) return true;
    return false;
  }
  public override void Removed(Scene scene){
    destroy(false);
  }
  public void AddAllChildren(List<Entity> l){
    foreach(ITemplateChild c in children){
      c.AddAllChildren(l);
    }
    if(!(this is TemplateDisappearer)) l.Add(this);
  }
  public List<T> GetChildren<T>() where T:Entity{
    List<Entity> list = new();
    AddAllChildren(list);
    List<T> nlist = new();
    foreach(var li in list) if(li is T le) nlist.Add(le);
    return nlist;
  }
  public virtual void parentChangeStat(int vis, int col, int act){
    foreach(ITemplateChild c in children){
      c.parentChangeStat(vis,col,act);
    }
  }
  public virtual void destroy(bool particles){
    foreach(ITemplateChild c in children){
      c.destroy(particles);
    }
    RemoveSelf();
  }
  public string fullpath=>parent==null?ownidpath.ToString():parent.fullpath+$"/{ownidpath}";
  public static string getOwnID(EntityData e){
    return e.ID.ToString();
  }
}