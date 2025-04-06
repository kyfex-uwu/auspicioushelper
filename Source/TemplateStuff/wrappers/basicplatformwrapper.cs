




using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class BasicPlatform:Entity, ITemplateChild{
  public Template parent{get;set;}
  public Template.Propagation prop {get;} = Template.Propagation.All;
  Platform p;
  Vector2 toffset;
  public BasicPlatform(Platform p, Template t, Vector2 offset):base(t.Position){
    p.Depth += t.depthoffset;
    this.p=p;
    parent = t;
    toffset = offset;
    Visible = false;
    p.OnDashCollide = (Player p, Vector2 dir)=>((ITemplateChild) this).propegateDashHit(p,dir);
  }
  public void relposTo(Vector2 loc, Vector2 liftspeed){
    if(p == null)return;
    p.MoveTo(loc+toffset, liftspeed);
  }
  public void addTo(Scene scene){
    scene.Add(this);
    scene.Add(p);
  }
  public bool hasRiders<T>() where T:Actor{
    if(p == null || p.Scene==null) return false;
    if(p is Solid s){
      if(typeof(T) == typeof(Player)) return s.HasPlayerRider();
      if(typeof(T) == typeof(Actor)) return s.HasRider();
      return false;
    } else if(p is JumpThru j){
      if(typeof(T) == typeof(Player)) return j.HasPlayerRider();
      if(typeof(T) == typeof(Actor)) return j.HasRider();
    }
    return false;
  }
  public bool hasInside(Actor a){
    return (p is Solid) && p.Collider.Collide(a.Collider);
  }
  public void AddAllChildren(List<Entity> l){
    l.Add(p);
  }
  public void parentChangeStat(int vis, int col){
    if(vis!=0)p.Visible = vis>0;
    if(col!=0)p.Collidable = col>0;
    if(col>0) p.EnableStaticMovers();
    else if(col<0) p.DisableStaticMovers();
  }
}