



using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public interface ITemplateChild{
  Template parent {get; set;}
  Template.Propagation prop {get;}
  void relposTo(Vector2 loc){

  }
  void parentChangesVis(bool visibility, Color tint){

  }
  bool hasRiders<T>() where T : Actor{
    return false;
  }
  Template.Propagation propegatesTo(Template target){
    ITemplateChild c=this;
    Template.Propagation p = Template.Propagation.All;
    while(c != target && c!= null){
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
  int depthoffset;
  public Template parent{get;set;} = null;
  [Flags]
  public enum Propagation
  {
      None     = 0,      
      Riding   = 1 << 0, 
      DashHit  = 1 << 1,
      Weight   = 1 << 2,
      Inside = 1<<3,
      All = Riding|DashHit|Weight|Inside
  }
  public Propagation prop{get;} = Propagation.All; 
  public Vector2 toffset = Vector2.Zero;
  public Template(string templateStr, Vector2 pos, int depthoffset):base(pos){
    if(!MarkedRoomParser.templates.TryGetValue(templateStr, out t)){
      DebugConsole.Write("No template found with identifier "+templateStr);
    }
    this.depthoffset = depthoffset;
    this.Visible = false;
  }
  public virtual void relposTo(Vector2 loc){
    Position = loc+toffset;
    childRelposTo(Position);
  }
  public void childRelposTo(Vector2 loc){
    foreach(ITemplateChild c in children){
      c.relposTo(loc);
    }
  }
  public void addEnt(ITemplateChild c){
    Entity ce = c as Entity;
    ce.Add(new ChildMarkerComponent(this));
    Scene.Add(ce);
    children.Add(c);
    c.parent = this;
  }
  public override void Added(Scene scene){
    base.Added(scene);
    if(t== null) return;
    if(t.bgt!=null) addEnt(new Wrappers.BgTiles(t,Position,depthoffset));
    if(t.fgt!=null) addEnt(new Wrappers.FgTiles(t, Position, depthoffset));
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