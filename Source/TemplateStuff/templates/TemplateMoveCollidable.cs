


using System;
using System.Collections;
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
  bool useOwnUncollidable;
  public TemplateMoveCollidable(EntityData data, Vector2 pos, int depthoffset, string idp):base(data,pos,depthoffset){
    Position = Position.Round();
    movementCounter = Vector2.Zero;
    prop &= ~Propagation.Riding; 
  }
  List<Solid> solids;
  bool dislocated = false;
  
  MipGridCollisionCacher mgcc = new();
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
  public class QueryBounds {
    public List<FloatRect> rects=new();
    public List<MipGrid> grids=new();
    public bool Collide(FloatRect o, Vector2 offset){
      float nx = MathF.Round(o.x)+offset.X;
      float ny = MathF.Round(o.y)+offset.Y;
      foreach(var rect in rects){
        if(rect.CollideExRect(nx,ny,o.w,o.h)) return true;
      }
      FloatRect n = new FloatRect(nx,ny,o.w,o.h);
      foreach(var grid in grids){
        if(grid.collideFrOffset(n,offset)) return true;
      }
      return false;
    }
    public bool Collide(MipGrid g, Vector2 offset){
      foreach(var grid in grids){
        if(grid.collideMipGridOffset(g,offset)) return true;
      }
      foreach(var rect in rects){
        if(g.collideFrOffset(rect, -offset)) return true;
      }
      return false;
    }
    public bool Collide(QueryIn q, Vector2 offset){
      foreach(var g in q.grids) if(Collide(g,offset)) return true;
      foreach(var r in q.rects) if(Collide(r,offset)) return true;
      return false;
    }
  }
  public class QueryIn{
    public List<FloatRect> rects=new();
    public List<MipGrid> grids=new();
    public FloatRect bounds = FloatRect.empty;
    public HashSet<Solid> gotten;
  }
  QueryBounds getQinfo(FloatRect f, HashSet<Solid> exclude){
    QueryBounds res  =new();
    foreach(Solid s in Scene.Tracker.GetEntities<Solid>()){
      if(!s.Collidable || exclude.Contains(s)) continue;
      FloatRect coarseBounds = new FloatRect(s);
      if(s.Collider is Hitbox h && f.CollideFr(coarseBounds)) res.rects.Add(coarseBounds);
      if(s.Collider is Grid g && f.CollideFr(coarseBounds)) res.grids.Add(MipGrid.fromGrid(g));
    }
    return res;
  }
  QueryIn getQself(){
    QueryIn res = new();
    FloatRect bounds = FloatRect.empty;
    var all = GetChildren<Solid>(Propagation.Shake);
    res.gotten = new(all);
    foreach(Solid s in all){
      if(useOwnUncollidable || s.Collidable){
        FloatRect coarseBounds = new FloatRect(s);
        if(s.Collider is Grid g) res.grids.Add(MipGrid.fromGrid(g));
        else if(s.Collider is Hitbox f) res.rects.Add(coarseBounds);
        else continue; 
        bounds = bounds._union(coarseBounds);
      }
    }
    res.bounds = bounds;
    return res;
  }
  public Vector2 TestMove(QueryBounds q, QueryIn s, int amount, Vector2 dirvec){
    int dir = Math.Sign(amount);
    int i = 0;
    while(i!=amount){
      if(q.Collide(s,dirvec*(i+1))) return dirvec*i;
      i+=dir;
    }
    return dirvec*amount;
  }
  public Vector2 TestLeniency(QueryBounds q, QueryIn s, Vector2 ioffset, int maxLeniency, Vector2 leniencyVec){
    for(int i=1; i<=maxLeniency; i++){
      for(int j=-1; j<=1; j+=2){
        if(!q.Collide(s,i*j*leniencyVec+ioffset)) return i*j*leniencyVec+ioffset;
      }
    }
    return Vector2.Zero;
  }
  public bool TestMoveLeniency(QueryBounds q, QueryIn s, int amount, Vector2 dirvec, int maxLeniency, Vector2 leniencyVec, out Vector2 loc){
    if(amount == 0){
      if(q.Collide(s,Vector2.Zero)){
        loc = TestLeniency(q,s,Vector2.Zero,maxLeniency,leniencyVec);
        return loc==Vector2.Zero;
      }
      loc = Vector2.Zero;
      return false;
    }
    var tryMove = TestMove(q,s,amount,dirvec);
    if(tryMove != Vector2.Zero){
      loc = tryMove;
      return true;
    } else {
      loc = TestLeniency(q,s,dirvec*Math.Sign(amount),maxLeniency,leniencyVec);
      return loc==Vector2.Zero;
    }
  }
  public Vector2 TestMoveLeniency(QueryBounds q, QueryIn s, int amount, Vector2 dirvec, int maxLeniency, Vector2 leniencyVec){
    bool res = TestMoveLeniency(q,s,amount,dirvec,maxLeniency,leniencyVec, out var v);
    return v;
  }
  // public bool MoveBy(Vector2 amount, Vector2 liftspeed, Vector2? leniencyVec, int? maxLeniency){
  //   Vector2 leni = leniencyVec??Vector2.Zero;
  //   movementCounter+=amount;
  //   if(exactPosition.Round()!=Position){
  //     Vector2 delta = exactPosition.Round()-Position;
  //     var ml = maxLeniency??0;
  //     bool res;
  //     QueryIn s = getQself();
  //     Vector2 ex = amount.Abs()+leni.Abs()*ml;
  //     QueryBounds q = getQinfo(s.bounds._expand(ex.X+1,ex.Y+1),s.gotten);
  //     if(ml == 0){
  //       Vector2 npos = 
  //     }
  //   }
  //   return true;
  // }
  public bool MoveHCollide(QueryBounds q, QueryIn s, int amount, int leniency, Vector2 liftspeed){
    Vector2 v = leniency==0? TestMove(q,s,amount,new Vector2(1,0)) : TestMoveLeniency(q,s,amount,new Vector2(1,0),leniency,new Vector2(0,1));
    if(v!=Vector2.Zero){
      Position+=v;
      childRelposTo(virtLoc,liftspeed);
      return false;
    }
    return true;
  }
  public bool MoveVCollide(QueryBounds q, QueryIn s, int amount, int leniency, Vector2 liftspeed){
    Vector2 v = leniency==0? TestMove(q,s,amount,new Vector2(0,1)) : TestMoveLeniency(q,s,amount,new Vector2(0,1),leniency,new Vector2(1,0));
    if(v!=Vector2.Zero){
      Position+=v;
      childRelposTo(virtLoc,liftspeed);
      return false;
    }
    return true;
  }
}