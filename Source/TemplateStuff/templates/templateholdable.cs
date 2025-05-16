


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/templateholdable")]
public class TemplateHoldable:Actor{
  TemplateDisappearer te;
  Vector2 hoffset;
  Vector2 lpos;
  Vector2 exlpos;
  Vector2 prevLiftspeed;
  Vector2 Speed;
  Holdable Hold;
  Vector2 origpos;
  float noGravityTimer=0;
  bool keepCollidableAlways = false;
  float playerfrac; float theofrac;
  string wallhitsound;
  float wallhitKeepspeed;
  float gravity;
  float friction;
  float terminalvel;
  bool dietobarrier;
  bool respawning;
  float respawndelay;
  EntityData d;
  public TemplateHoldable(EntityData d, Vector2 offset):base(d.Position+offset){
    Position+=new Vector2(d.Width/2, d.Height);
    hoffset = d.Nodes.Length>0?d.Nodes[0]-new Vector2(d.Width/2, d.Height):new Vector2(0,-d.Height/2);
    Collider = new Hitbox(d.Width,d.Height,-d.Width/2,-d.Height);
    lpos = Position;
    Add(Hold = new Holdable(d.Float("cannot_hold_timer",0.1f)));
    var ex = d.Int("Holdable_collider_expand",4);
    Hold.PickupCollider = new Hitbox(d.Width+2*ex, d.Height+2*ex, -d.Width/2-ex, -d.Height-ex);
    Hold.SlowFall = d.Bool("slowfall",false);
    Hold.SlowRun = d.Bool("slowrun",true); 
    Hold.SpeedGetter = ()=>Speed;
    Hold.SpeedSetter = (Vector2 v)=>Speed=v;
    Hold.OnPickup = OnPickup;
    Hold.OnRelease = OnRelease;
    Hold.OnHitSpring = HitSpring;
    LiftSpeedGraceTime = 0.1f;
    Tag = Tags.TransitionUpdate;
    keepCollidableAlways = d.Bool("always_collidable",false);
    origpos = Position;
    hooks.enable();
    this.d=d;

    playerfrac = d.Float("player_momentum_weight",1);
    theofrac = d.Float("holdable_momentum_weight",0);
    wallhitsound = d.Attr("wallhitsound","event:/game/05_mirror_temple/crystaltheo_hit_side");
    wallhitKeepspeed = d.Float("wallhit_speedretain",0.4f);
    gravity = d.Float("gravity",800f);
    terminalvel = d.Float("terminal_velocity",200f);
    friction = d.Float("friction",350);
    dietobarrier = d.Bool("die_to_barrier",false);
    respawning = d.Bool("respawning",false);
    respawndelay = d.Float("respawnDelay",2f);
    SquishCallback = OnSquish2;
  }
  HashSet<Platform> Mysolids;
  void make(Scene s){
    te = new TemplateDisappearer(d.Attr("template"),Position+hoffset,d.Int("depthoffset",0),Template.getOwnID(d));
    te.addTo(s);
    Mysolids = new (te.GetChildren<Solid>());
  }
  public override void Added(Scene scene){
    base.Added(scene);
    make(scene);
  }
  public override void Removed(Scene scene){
    if(te!=null){
      te.destroy(true);
      te = null;
    }
    base.Removed(scene);
  }
  public override bool IsRiding(JumpThru jumpThru){
    return Speed.Y ==0 && !Hold.IsHeld && base.IsRiding(jumpThru);
  }
  public override bool IsRiding(Solid jumpThru){
    return Speed.Y ==0 && !Hold.IsHeld && base.IsRiding(jumpThru);
  }
  bool HitSpring(Spring s){
    if(Hold.IsHeld) return false;
    if (s.Orientation == Spring.Orientations.Floor && Speed.Y >= 0f){
      Speed.X *= 0.5f;
      Speed.Y = -160f;
      noGravityTimer = 0.15f;
      return true;
    }
    if (s.Orientation == Spring.Orientations.WallLeft && Speed.X <= 0f){
      MoveTowardsY(s.CenterY + 5f, 4f);
      Speed.X = 220f;
      Speed.Y = -80f;
      noGravityTimer = 0.1f;
      return true;
    }
    if (s.Orientation == Spring.Orientations.WallRight && Speed.X >= 0f){
      MoveTowardsY(s.CenterY + 5f, 4f);
      Speed.X = -220f;
      Speed.Y = -80f;
      noGravityTimer = 0.1f;
      return true;
    }
    return false;
  }
  void OnPickup(){
    if(lastPickup!=null){
      lastPickup.Speed = lastPickup.Speed*playerfrac+Speed*theofrac;
    }
    Speed = Vector2.Zero;
    if(!keepCollidableAlways) te.setCollidability(false);
    AddTag(Tags.Persistent);
  }
  void OnRelease(Vector2 force){
    RemoveTag(Tags.Persistent);
    if (force.X != 0f && force.Y == 0f){
      force.Y = -0.4f;
    }
    if(!keepCollidableAlways) te.setCollidability(true);
    Speed = force * 200f;
    if (Speed != Vector2.Zero){
      noGravityTimer = 0.1f;
    }
    exlpos = ExactPosition;
  }
  void OnCollideH(CollisionData data){
    if (data.Hit is DashSwitch){
      (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * MathF.Sign(Speed.X));
    }

    Audio.Play(wallhitsound, Position);
    // if (Math.Abs(Speed.X) > 100f){
    //   ImpactParticles(data.Direction);
    // }
    Speed.X *= -wallhitKeepspeed;
  }
  void OnCollideV(CollisionData data){
    if (data.Hit is DashSwitch){
      (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitY * Math.Sign(Speed.Y));
    }
    Audio.Play(wallhitsound, Position);
    // if (Speed.Y > 160f){
    //   ImpactParticles(data.Direction);
    // }
    if (Speed.Y > 140f && !(data.Hit is DashSwitch)){
      Speed.Y *= -0.6f;
    }
    else{
      Speed.Y = 0f;
    }
  }
  bool resetting;
  IEnumerator resetRoutine(){
    if(resetting){
      DebugConsole.Write("Called multiple times to reset routine (bad)");
      yield break;
    }
    resetting = true;
    Collidable = false;
    Position = origpos;
    Speed = Vector2.Zero;
    te.destroy(true);
    Mysolids.Clear();
    te = null;
    if(!respawning){
      RemoveSelf();
      yield break;
    }
    yield return respawndelay;
    if(Scene!=null)make(Scene);
    Collidable = true;
    resetting = false;
  }
  public void OnSquish2(CollisionData data){
    if(inRelpos || Mysolids.Contains(data.Hit)) return;
    DebugConsole.Write($"squished: {data.Pusher} {data.Hit}");
    Add(new Coroutine(resetRoutine()));
  }
  public override void Update(){
    base.Update();
    if(resetting) return;

    evalLiftspeed(true);
    if (Hold.IsHeld){
      prevLiftspeed = Vector2.Zero;
    } else {
      //DebugConsole.Write($"out update: {Position}");
      if(OnGround()){
        float target = (!OnGround(Position + Vector2.UnitX * 3f))? 20f: (!OnGround(Position - Vector2.UnitX * 3f) ? -20f:0);
        Speed.X = Calc.Approach(Speed.X, target, 800f * Engine.DeltaTime);
        if (LiftSpeed == Vector2.Zero && prevLiftspeed != Vector2.Zero){
          Speed = prevLiftspeed;
          prevLiftspeed = Vector2.Zero;
          Speed.Y = Math.Min(Speed.Y * 0.6f, 0f);
          if (Speed.X != 0f && Speed.Y == 0f) Speed.Y=-60;
          if (Speed.Y < 0f) noGravityTimer = 0.15f;
        } else {
          prevLiftspeed = LiftSpeed;
          if (LiftSpeed.Y < 0f && Speed.Y < 0f) Speed.Y=0;
        }
      } else if(Hold.ShouldHaveGravity) {
        float num = gravity;
        if (Math.Abs(Speed.Y) <= 30f) num*=0.5f;
        float num2 = friction;
        if (Speed.Y < 0f) num2*=0.5f;

        Speed.X = Calc.Approach(Speed.X, 0f, num2 * Engine.DeltaTime);
        if (noGravityTimer > 0f)noGravityTimer-=Engine.DeltaTime;
        else Speed.Y = Calc.Approach(Speed.Y, terminalvel, num * Engine.DeltaTime);
      }
      te.setCollidability(false);
      MoveH(Speed.X * Engine.DeltaTime, OnCollideH);
      MoveV(Speed.Y * Engine.DeltaTime, OnCollideV);
      te.setCollidability(true);
    }
    if(dietobarrier) foreach (SeekerBarrier entity in base.Scene.Tracker.GetEntities<SeekerBarrier>()){
      entity.Collidable = true;
      bool res = CollideCheck(entity);
      entity.Collidable = false;
      if (res){
        if (Hold.IsHeld){
          Vector2 speed2 = Hold.Holder.Speed;
          Hold.Holder.Drop();
          Speed = speed2 * 0.333f;
          Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        }
        Audio.Play("event:/new_content/game/10_farewell/glider_engage", Position);
        Add(new Coroutine(resetRoutine()));
        break;
      }
    }

    var move = ExactPosition - exlpos;
    pastLiftspeed[0]+=move/Math.Max(Engine.DeltaTime,0.005f);
    exlpos = ExactPosition;
    if(lpos!=Position){
      lpos = Position;
      inRelpos = true; AllowPushing = false;
      te.relposTo(Position+hoffset,te.ownLiftspeed);
      inRelpos = false; AllowPushing = true;
      Position = lpos;
    }
    lpos = Position;

  }
  bool inRelpos;
  const int smearamount = 3;
  Vector2[] pastLiftspeed = new Vector2[smearamount];
  void evalLiftspeed(bool precess = true){
    float mX=0;
    float mY=0;
    foreach(Vector2 v in pastLiftspeed){
      if(MathF.Abs(v.X)>MathF.Abs(mX)) mX=v.X;
      if(MathF.Abs(v.Y)>MathF.Abs(mY)) mY=v.Y;
    } 
    if(te!=null)te.ownLiftspeed = new Vector2(mX,mY);
    else DebugConsole.Write($"te is null?");
    if(!precess) return; 
    for(int i=smearamount-1; i>=1; i--){
      pastLiftspeed[i]=pastLiftspeed[i-1];
    }
    pastLiftspeed[0]=Vector2.Zero;
  }
  static Player lastPickup;
  public static bool PickupHook(On.Celeste.Holdable.orig_Pickup orig, Holdable self, Player player){
    lastPickup = player;
    return orig(self, player);
  }
  public static bool MoveHHook(On.Celeste.Actor.orig_MoveHExact orig, Actor self, int amount, Collision cb, Solid pusher){
    if(pusher != null && self is TemplateHoldable s && s.te!=null){
      s.te.setCollidability(false);
      bool res = orig(self,amount,cb,pusher);
      s.te.setCollidability(true);
      return res;
    } else{
      return orig(self, amount, cb, pusher);
    }
  }
  public static bool MoveVHook(On.Celeste.Actor.orig_MoveVExact orig, Actor self, int amount, Collision cb, Solid pusher){
    if(pusher != null && self is TemplateHoldable s && s.te!=null){
      s.te.setCollidability(false);
      bool res = orig(self,amount,cb,pusher);
      s.te.setCollidability(true);
      return res;
    } else{
      return orig(self, amount, cb, pusher);
    }
  }
  static HookManager hooks = new HookManager(()=>{
    On.Celeste.Holdable.Pickup += PickupHook;
    On.Celeste.Actor.MoveHExact+=MoveHHook;
    On.Celeste.Actor.MoveVExact+=MoveVHook;
  },()=>{
    On.Celeste.Holdable.Pickup -= PickupHook;
    On.Celeste.Actor.MoveHExact-=MoveHHook;
    On.Celeste.Actor.MoveVExact-=MoveVHook;
  },auspicioushelperModule.OnEnterMap);
}