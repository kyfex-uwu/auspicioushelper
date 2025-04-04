



using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public interface ITemplateChild{
  Template parent {get; set;}
  Template.Propagation prop {get;}
  void relposTo(Vector2 loc, Vector2 liftspeed);
  void addTo(Scene s){

  }
  void parentChangeStat(bool visibility, bool collidability, int clayer){

  }
  bool hasRiders<T>() where T : Actor{
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
}

public class Template:Entity, ITemplateChild{
  templateFiller t=null;
  public List<ITemplateChild> children = new List<ITemplateChild>();
  public int depthoffset;
  public Template parent{get;set;} = null;
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
  public Propagation prop{get;} = Propagation.All; 
  public Vector2 toffset = Vector2.Zero;
  public Wrappers.BasicMultient basicents = null;
  public Template(string templateStr, Vector2 pos, int depthoffset):base(pos){
    if(!MarkedRoomParser.templates.TryGetValue(templateStr, out t)){
      DebugConsole.Write("No template found with identifier "+templateStr);
    }
    this.depthoffset = depthoffset;
    this.Visible = false;
  }
  public virtual void relposTo(Vector2 loc, Vector2 liftspeed){
    Position = loc+toffset;
    childRelposTo(Position, liftspeed);
  }
  public void childRelposTo(Vector2 loc, Vector2 liftspeed){
    foreach(ITemplateChild c in children){
      c.relposTo(loc, liftspeed);
    }
  }
  public void addEnt(ITemplateChild c, bool sceneadd=false){
    Entity ce = c as Entity;
    if(sceneadd)Scene.Add(ce);
    children.Add(c);
    c.parent = this;
  }
  public override void Added(Scene scene){
    base.Added(scene);
    if(basicents!=null && basicents.Scene==null) basicents.sceneadd(scene);
    if(t== null) return;
    if(t.bgt!=null) addEnt(new Wrappers.BgTiles(t,Position,depthoffset),true);
    if(t.fgt!=null) addEnt(new Wrappers.FgTiles(t, Position, depthoffset),true);
    Level l = SceneAs<Level>();
    Vector2 simoffset = this.Position-t.origin;
    foreach(EntityParser.EWrap w in t.childEntities){
      Entity e = EntityParser.create(w,l,t.roomdat,simoffset,this);
      if(e is ITemplateChild c){
        c.addTo(scene);
        addEnt(c);
      }
      else if(e!=null)scene.Add(e);
    }
    foreach(DecalData d in t.decals){
      Decal e = new Decal(d.Texture, simoffset+d.Position, d.Scale, d.GetDepth(0), d.Rotation, d.ColorHex){
        DepthSetByPlacement = true
      };
      AddBasicEnt(e, simoffset+d.Position-Position);
    }
  }
  public void AddBasicEnt(Entity e, Vector2 offset){
    if(basicents == null){
      basicents = new Wrappers.BasicMultient(this);
      addEnt(basicents, Scene!=null);
      if(Scene!=null)basicents.Scene = Scene;
    }
    basicents.add(e,offset);
  }
  public bool hasRiders<T>() where T:Actor{
    foreach(ITemplateChild c in children){
      if((c.prop & Propagation.Riding)!=0 && c.hasRiders<T>()) return true;
    }
    return false;
  }
  public override void Removed(Scene scene){
    foreach(ITemplateChild c in children) (c as Entity).RemoveSelf();
  }
}