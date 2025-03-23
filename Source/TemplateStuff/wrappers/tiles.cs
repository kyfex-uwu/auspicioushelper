

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class BgTiles:BackgroundTiles, ITemplateChild{
  public Template.Propagation prop{get;} = Template.Propagation.None;
  public Template parent {get;set;}
  public Vector2 toffset;
  public BgTiles(templateFiller t, Vector2 posoffset, int depthoffset):base(posoffset+t.offset, t.bgt){
    toffset = t.offset;
    Depth+=depthoffset;
  }
  public void relposTo(Vector2 loc, Vector2 liftspeed){
    Position = loc+toffset;
  }
}

public class FgTiles:SolidTiles, ITemplateChild{
  public Template.Propagation prop{get;} = Template.Propagation.All;
  public Template parent {get;set;}
  public Vector2 toffset;
  public FgTiles(templateFiller t, Vector2 posoffset, int depthoffset):base(posoffset+t.offset, t.fgt){
    toffset = t.offset;
    Depth+=depthoffset;
  }
  public bool hasRiders<T>() where T:Actor{
    foreach(T a in Scene.Tracker.GetEntities<T>()){
      if(a.IsRiding(this)) return true;
    }
    return false;
  }
  public void relposTo(Vector2 loc, Vector2 liftspeed){
    MoveTo(loc+toffset, liftspeed);
  }

  public override void MoveHExact(int move){
    GetRiders();
    Player player = null;
    player = base.Scene.Tracker.GetEntity<Player>();
    //What the heck does this do?
    if (player != null && Input.MoveX.Value == Math.Sign(move) && Math.Sign(player.Speed.X) == Math.Sign(move) && 
    !riders.Contains(player) && CollideCheck(player, Position + Vector2.UnitX * move - Vector2.UnitY)){
      player.MoveV(1f);
    }

    base.X += move;
    MoveStaticMovers(Vector2.UnitX * move);
    if(!Collidable) return;
    foreach (Actor entity in base.Scene.Tracker.GetEntities<Actor>()){
      if (!entity.AllowPushing) continue;
      bool collidable = entity.Collidable;
      entity.Collidable = true;
      if (!entity.TreatNaive && CollideCheck(entity, Position)){
        Collidable = false;
        for(int i=0; i<Math.Abs(move); i++){
          if(!CollideCheck(entity, Position) || entity.MoveHExact(Math.Sign(move), entity.SquishCallback, this)) break;
        }
        entity.LiftSpeed = LiftSpeed;
        Collidable = true;
      } else if (riders.Contains(entity)) {
        Collidable = false;
        if (entity.TreatNaive) entity.NaiveMove(Vector2.UnitX * move);
        else entity.MoveHExact(move);
        entity.LiftSpeed = LiftSpeed;
        Collidable = true;
      }
      entity.Collidable = collidable;
    }  
    riders.Clear();
  }
  public override void MoveVExact(int move){
    GetRiders();
    base.Y += move;
    MoveStaticMovers(Vector2.UnitY * move);
    if(!Collidable) return;
    foreach (Actor entity in base.Scene.Tracker.GetEntities<Actor>()){
      if (!entity.AllowPushing) continue;
      bool collidable = entity.Collidable;
      entity.Collidable = true;
      if (!entity.TreatNaive && CollideCheck(entity, Position)){
          Collidable = false;
          for(int i=0; i<Math.Abs(move); i++){
            if(!CollideCheck(entity, Position) || entity.MoveVExact(Math.Sign(move), entity.SquishCallback, this)) break;
          }
          entity.LiftSpeed = LiftSpeed;
          Collidable = true;
      } else if (riders.Contains(entity)){
          Collidable = false;
          if (entity.TreatNaive) entity.NaiveMove(Vector2.UnitY * move);
          else entity.MoveVExact(move);
          entity.LiftSpeed = LiftSpeed;
          Collidable = true;
      }
      entity.Collidable = collidable;
    }
    riders.Clear();
  }
  public override void Awake(Scene scene){
    base.Awake(scene);
    foreach (StaticMover smover in scene.Tracker.GetComponents<StaticMover>()){
      if (smover.Platform == null && smover.IsRiding(this)){
        staticMovers.Add(smover);
        smover.Platform = this;
        if (smover.OnAttach != null){
          smover.OnAttach(this);
        }
      }
    }
  }
}