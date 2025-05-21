


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class TemplateMoveCollidable:TemplateDisappearer{ 
  public override Vector2 gatheredLiftspeed => ownLiftspeed;
  Vector2 movementCounter;
  bool useOwnUncollidable;
  public TemplateMoveCollidable(EntityData data, Vector2 pos, int depthoffset, string idp):base(data,pos,depthoffset){
    movementCounter = Vector2.Zero;
    prop &= ~Propagation.Riding; 
  }
  List<Solid> solids;
  MipGridCollisionCacher mgcc = new();
  public override void addTo(Scene scene){
    base.addTo(scene);
    solids = GetChildren<Solid>();
  }

  public override void relposTo(Vector2 loc, Vector2 liftspeed) {}
  public class QueryBounds {
    public List<FloatRect> rects=new();
    public List<MipGrid> grids=new();
    public bool Test(FloatRect o, Vector2 offset){
      float nx = MathF.Round(o.x)+offset.X;
      float ny = MathF.Round(o.y)+offset.Y;
      foreach(var rect in rects){
        if(rect.CollideExRect(nx,ny,o.w,o.h)) return true;
      }
      FloatRect n = new FloatRect(nx,ny,o.w,o.h);
      foreach(var grid in grids){
        if(grid.collideFrOffset(o,offset)) return true;
      }
      return false;
    }
    public bool Test(MipGrid g, Vector2 offset){
      foreach(var grid in grids){
        if(grid.collideMipGridOffset(g,offset)) return true;
      }
      foreach(var rect in rects){
        if(g.collideFrOffset(rect, -offset)) return true;
      }
      return false;
    }
    public bool Test(Solid s, Vector2 offset){

    }
  }
  public QueryBounds getQinfo(){
    QueryBounds res  =new();
    foreach(Solid s in solids){
      if(!s.Collidable) continue;
      if(s.Collider is Hitbox h) res.hitboxSolids
    }
  }

  public bool testOffset(QueryBounds q, Vector2 offset){
    bool uou = useOwnUncollidable;
    foreach(Solid s in solids){
      if(uou||s.Collidable||q.Test(s,offset)){
        return true;
      }
    }
    return false;
  }
  public Vector2 testMove(QueryBounds q, int amount, Vector2 dirvec){
    int dir = Math.Sign(amount);
    int i = 0;
    while(i!=amount){
      if(testOffset(q,dirvec*(i+1))) return dirvec*i;
      i+=dir;
    }
    return dirvec*amount;
  }
  public Vector2 testLeniency(QueryBounds q, Vector2 ioffset, int maxLeniency, Vector2 leniencyVec){
    for(int i=1; i<=maxLeniency; i++){
      for(int j=-1; j<=1; j+=2){
        if(!testOffset(q,i*j*leniencyVec+ioffset)) return i*j*leniencyVec+ioffset;
      }
    }
  }
  public Vector2 testMoveLeniency(QueryBounds q, int amount, Vector2 dirvec, int maxLeniency, Vector2 lenicyVec){
    int dir = Math.Sign(amount);
    int i=0;
    if(amount == 0) return 
  }
}