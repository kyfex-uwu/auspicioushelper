


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.auspicioushelper;

public class TemplateMoveCollidable:TemplateDisappearer{ 
  public override Vector2 gatheredLiftspeed => ownLiftspeed;
  Vector2 movementCounter;
  Vector2 exactPosition=>Position+movementCounter;
  public override Vector2 virtLoc => dislocated?Position.Round():Position;
  public TemplateMoveCollidable(EntityData data, Vector2 pos, int depthoffset, string idp):base(data,pos,depthoffset){
    Position = Position.Round();
    movementCounter = Vector2.Zero;
    prop &= ~Propagation.Riding; 
  }
  List<Solid> solids;
  bool dislocated = false;
  
  public override void addTo(Scene scene){
    base.addTo(scene);
    solids = GetChildren<Solid>();
  }
  public override void relposTo(Vector2 loc, Vector2 liftspeed) {
    if(!dislocated) base.relposTo(loc,liftspeed);
  }
  public virtual void disconnect(){
    dislocated = true;
    prop &= ~Propagation.Shake;
    Position = Position.Round();
    childRelposTo(virtLoc,gatheredLiftspeed);
  }
  public virtual void reconnect(){
    dislocated = false;
    prop |= Propagation.Shake;
    bool old = getSelfCol();
    setCollidability(false);
    relposTo(parent.virtLoc, Vector2.Zero);
    setCollidability(old);
  }
  class QueryBounds {
    public List<FloatRect> rects=new();
    public List<MipGrid> grids=new();
  }
  QueryBounds getQinfo(FloatRect f,HashSet<Solid> exclude){
    QueryBounds res  =new();
    foreach(Solid s in solids){
      if(!s.Collidable || exclude.Contains(s)) continue;
      FloatRect coarseBounds = new FloatRect(s);
      if(s.Collider is Hitbox h && f.CollideFr(coarseBounds)) res.rects.Add(coarseBounds);
      if(s.Collider is Grid g && f.CollideFr(coarseBounds)){
        DynamicData d = new DynamicData(g);
        if(d.TryGet("__mipgrid", out var obj) && obj is MipGrid m){
          res.grids.Add(m);
        } else {
          res.grids.Add(m = new MipGrid(g));
          d.Set("__mipgrid",m);
        }
      }
    }
    return res;
  }
}