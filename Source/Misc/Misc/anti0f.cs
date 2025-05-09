



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/Anti0fZone")]
[Tracked]
public class Anti0fZone:Entity{
  public FloatRect bounds;
  float maxstep = 4;
  bool ctriggers = true;
  bool cplayercolliders = true;
  bool cthrowables = false;
  bool csolids = false;
  bool cposexit = false;
  bool wholeroom = false;
  public Anti0fZone(EntityData d, Vector2 offset):base(d.Position+offset){
    bounds = new FloatRect(Position.X,Position.Y,d.Width,d.Height);
    maxstep = d.Int("step",4);
    hooks.enable();
    cthrowables = d.Bool("holdables", false);
    cplayercolliders = d.Bool("player_colliders",true);
    ctriggers = d.Bool("triggers", true);
  }
  public struct ACol<T>{
    public FloatRect.FRCollision f;
    public T o;
    public int order;
    public ACol(FloatRect.FRCollision info, T col){
      f=info; o=col;
    }
  }

  public static Anti0fZone getHit(Player p){
    FloatRect r = new FloatRect(p);
    foreach(Anti0fZone a in p.Scene.Tracker.GetEntities<Anti0fZone>()){
      if(a.wholeroom || a.bounds.CollideRectSweep(r,p.Speed*Engine.DeltaTime)){
        return a;
      }
    }
    return null;
  }

  public static bool PlayerUpdateDetour(Player p){
    //DebugConsole.Write("In update hook");
    int state = p.StateMachine.state;
    if(state == 9 || state == 22) return false;
    Vector2 ospeed = p.Speed;
    Vector2 step = ospeed*Engine.DeltaTime;
    FloatRect r = new FloatRect(p);
    Anti0fZone hits = null;
    foreach(Anti0fZone a in p.Scene.Tracker.GetEntities<Anti0fZone>()){
      if(a.bounds.CollideRectSweep(r,step)){
        hits=a;
        break;
      }
    }
    if(hits==null){
      return false;
    }
    if(hits.maxstep>0 || PortalGateH.intersections.ContainsKey(p)){
      //We use L1 distance
      float length = Math.Max(Math.Abs(step.X),Math.Abs(step.Y));
      int steps = (int)Math.Ceiling(length/MathF.Max(hits.maxstep,1));
      DebugConsole.Write("Inside 0f "+length.ToString()+" "+steps.ToString());
      Vector2 substep = step/(float)steps;
      for(int i=0; i<steps; i++){
        if(p.StateMachine.state != state || p.Speed.Y!=ospeed.Y || p.Speed.X!=ospeed.X || p.Dead) return true;
        if(i!=0){
          if(hits.cthrowables&&(state == 0 || state == 2 || state == 7)){
            foreach(Holdable c in p.Scene.Tracker.GetComponents<Holdable>()){
              if(c.Check(p) && c.Pickup(p)){
                p.StateMachine.state = 8;
                return true;
              }
            }
          }

          if(state != 18 && hits.ctriggers){
            foreach(Trigger t in p.Scene.Tracker.GetEntities<Trigger>()){
              if(p.CollideCheck(t)){
                if(!t.Triggered){
                  t.Triggered = true;
                  p.triggersInside.Add(t);
                  t.OnEnter(p);
                }
              } else if(t.Triggered) {
                p.triggersInside.Remove(t);
                t.Triggered=false;
                t.OnLeave(p);
              }
            }
          }
          
          Collider g = p.Collider;
          p.Collider=p.hurtbox;
          if(!p.Dead && state!=21 && hits.cplayercolliders){
            foreach(PlayerCollider c in p.Scene.Tracker.GetComponents<PlayerCollider>()){
              if(c.Check(p) && p.Dead){
                p.Collider=g;
                return true;
              }
            }
          }
          p.Collider=g;
          
        }
        p.MoveH(substep.X,p.onCollideH,null);
        p.MoveV(substep.Y,p.onCollideV,null);
      }
    } else {
      foreach(Holdable h in p.Scene.Tracker.GetComponents<Holdable>()){
        
      }
    }
    return true;
    
  }
  abstract class LinearRaster<T>{
    public LinkedList<ACol<T>> active = new();
    List<ACol<T>> mayHit = new();
    int addIdx = 0;
    public void Fill(List<ACol<T>> l, float maxt){
      for(int idx=0; idx<l.Count; idx++){
        var col = l[idx];
        if(col.f.collides && col.f.enter<maxt){
          col.order=idx;
          mayHit.Add(col);
        }
      }
      mayHit.Sort((a,b)=>{
        var c1 = MathF.Max(a.f.enter,0)- MathF.Max(b.f.enter,0);
        if(c1 != 0) return MathF.Sign(c1);
        return a.order-b.order;
      });
    }
    public void Clear(){
      active.Clear();
      mayHit.Clear();
      addIdx=0;
    }
    public abstract void Fill(Player p, Vector2 step, float maxt);
    public abstract bool prog(Player p, float step);
    public bool prog(float step){
      var cn = active.First;
      if((cn==null) && (addIdx>=mayHit.Count || mayHit[addIdx].f.enter>step)) return false;
      while(cn!=null){
        if(cn.Value.f.exit<step) active.Remove(cn);
        cn=cn.Next;
      }
      while(addIdx<mayHit.Count && mayHit[addIdx].f.enter<=step){
        if(mayHit[addIdx].f.exit>=step) active.AddLast(mayHit[addIdx]);
        addIdx++;
      }
      return true;
    }

  }
  class HoldableRaster:LinearRaster<Holdable>{
    public override void Fill(Player p, Vector2 step, float maxt){
      FloatRect f = new FloatRect(p);
      Fill(p.Scene.Tracker.GetComponents<Holdable>().Select(
        h=>new ACol<Holdable>(f.ISweep(h.Entity.Collider,-step),(Holdable)h)
      ).ToList(),maxt);
    }
    public override bool prog(Player p, float step){
      if(!Input.GrabCheck || p.IsTired || p.Holding!=null) return false;
      switch(p.StateMachine.state){
        case 0: case 7:
          if(p.Ducking) return false; break;
        case 2:
          if(p.DashDir == Vector2.Zero || !p.CanUnDuck) return false; break;
        default: return false;
      }
      prog(step);
      foreach(var h in active){
        if(h.o.Check(p) && h.o.Pickup(p)){
          p.StateMachine.state = 8; 
          return true;
        }
      }
      return false;
    }
  }
  class ColliderRaster:LinearRaster<PlayerCollider>{
    public override void Fill(Player p, Vector2 step, float maxt){
      FloatRect f = new FloatRect(p)._expand(1,1);
      Fill(p.Scene.Tracker.GetComponents<PlayerCollider>().Select(
        h=>new ACol<PlayerCollider>(f.ISweep(h.Entity.Collider,-step),(PlayerCollider)h)
      ).ToList(),maxt);
    }
    public override bool prog(Player p, float step){
      if(p.StateMachine.state == 21) return false;
      prog(step);
      Collider old = p.Collider;
      p.Collider = p.hurtbox;
      var cn = active.First;
      while(cn!=null){
        if(cn.Value.o.Check(p)){
          active.Remove(cn);
          if(p.Dead){
            p.Collider = old;
            return true;
          }
        }
        cn=cn.Next;
      }
      p.Collider = old;
      return false;
    }
  }
  class TriggerRaster:LinearRaster<Trigger>{
    public override void Fill(Player p, Vector2 step, float maxt){
      FloatRect f = new FloatRect(p)._expand(4,4);
      Fill(p.Scene.Tracker.GetEntities<Trigger>().Select(
        h=>new ACol<Trigger>(f.ISweep(h.Collider,-step),(Trigger)h)
      ).ToList(),maxt);
    }
    public HashSet<Trigger> entered;
    public override bool prog(Player p, float step){
      if (p.StateMachine.State == 18) return false;
      prog(step);
      var cn = active.First;
      while(cn != null){
        var t = cn.Value.o;
        if (p.CollideCheck(t)){
          if (!t.Triggered){
            t.Triggered = true;
            p.triggersInside.Add(t);
            t.OnEnter(p);
          }
          t.OnStay(p);
          active.Remove(cn);
        } 
        cn = cn.Next;
      }
      return false;
    }
  }
  class SolidRaster:LinearRaster<Solid>{
    public override void Fill(Player p, Vector2 step, float maxt){
      FloatRect f = new FloatRect(p)._expand(8,8);
      Fill(p.Scene.Tracker.GetEntities<Solid>().Select(
        h=>new ACol<Solid>(f.ISweep(h.Collider,-step),(Solid)h)
      ).ToList(),maxt);
    }
    public override bool prog(Player p, float step){
      if(prog(step)) p.Scene.Tracker.Entities[typeof(Solid)] = active.Select(s=>s.o).ToList<Entity>();
      switch(p.StateMachine.state){
        case Player.StNormal:
          if (!Input.Jump.Pressed || !(TalkComponent.PlayerOver != null && Input.Talk.Pressed)) return false;
          //if(p.jumpGraceTimer>0f) p.Jump(); //always happens before we reach here
          else if(p.CanUnDuck){
            if(p.WallJumpCheck(1)){
              if (p.Facing==Facings.Right && Input.GrabCheck && !SaveData.Instance.Assists.NoGrabbing && p.Stamina>0f && 
                p.Holding==null && !ClimbBlocker.Check(p.Scene, p, p.Position + Vector2.UnitX * 3f)
              ) p.ClimbJump();
              else if (p.DashAttacking && p.SuperWallJumpAngleCheck) p.SuperWallJump(-1);
              else p.WallJump(-1);
              return true;
            } else if(p.WallJumpCheck(-1)){
              if(p.Facing==Facings.Left && Input.GrabCheck && !SaveData.Instance.Assists.NoGrabbing && p.Stamina>0f && 
                p.Holding==null && !ClimbBlocker.Check(p.Scene, p, p.Position + Vector2.UnitX * -3f)
              ) p.ClimbJump();
              else if (p.DashAttacking && p.SuperWallJumpAngleCheck) p.SuperWallJump(1);
              else p.WallJump(1);
              return true;
            }
          }
          return false;
        case Player.StDash: case Player.StRedDash:
          if(!Input.Jump.Pressed || !p.CanUnDuck) return false;
          bool wjcp = p.WallJumpCheck(1);
          bool wjcn = p.WallJumpCheck(-1);
          if(!(wjcp || wjcn)) return false;
          if(p.SuperWallJumpAngleCheck) p.SuperWallJump(wjcp?-1:1);
          else if(wjcp){
            if(p.Facing==Facings.Right && Input.GrabCheck && p.Stamina>0f && p.Holding==null && !ClimbBlocker.Check(p.Scene, p, p.Position+Vector2.UnitX*3f)){
              p.ClimbJump();
            } else p.WallJump(-1);
          } else {
            if(p.Facing==Facings.Left && Input.GrabCheck && p.Stamina>0f && p.Holding==null && !ClimbBlocker.Check(p.Scene, p, p.Position-Vector2.UnitX*3f)){
              p.ClimbJump();
            } else p.WallJump(1);
          }
          p.StateMachine.state = 0;
          return true;
      }
      return false;
    }
  }
  
  static HoldableRaster hrast = new();
  static ColliderRaster crast = new();
  static TriggerRaster trast = new();
  static void ClearRasters(){
    hrast.Clear(); crast.Clear(); trast.Clear();
  }
  static float MinStepSize = 1;
  static bool PlayerUpdateDetour2(Player p){
    int _st = p.StateMachine.state;
    Vector2 _ispeed = p.Speed;
    int _idashes = p.Dashes;
    Anti0fZone z = getHit(p);
    if(_st==9 || _st == 22 || z==null) return false;

    var dist = _ispeed*Engine.DeltaTime;
    start:  
      float length = Math.Max(Math.Abs(dist.X),Math.Abs(dist.Y));
      int steps = (int)Math.Ceiling(length/MathF.Max(z.maxstep,MinStepSize));
      Vector2 step = dist/(float)steps;
      ClearRasters();
      if(z.cthrowables) hrast.Fill(p, step, steps);
      if(z.cplayercolliders) crast.Fill(p,step,steps);
      if(z.ctriggers) trast.Fill(p,step,steps);

      List<Entity> oldSolids=null;
      if(z.csolids) oldSolids = p.Scene.Tracker.Entities[typeof(Solid)];

      bool exit()=>
        p.StateMachine.state!=_st || p.Dashes!=_idashes || 
        p.Speed!=_ispeed || p.Dead;
      bool proc(float prog){
        bool flag = false;
        if(z.cthrowables) flag |= hrast.prog(p,prog);
        if(z.cplayercolliders) flag |= crast.prog(p,prog);
        if(z.ctriggers) flag |= trast.prog(p,prog);
        return flag | exit();
      }
      for(int i=0; i<steps; i++){
        if(i!=0 && proc(i)) goto exit;
        var lpos = p.Position;
        if(p.MoveH(step.X) || p.MoveV(step.Y)) goto exit;
        if((lpos+step - p.Position).LengthSquared()>MinStepSize*MinStepSize*16){
          if(z.cposexit) goto exit;
          DebugConsole.Write("Position sharply changed while in anti0f! Attempting to reconsile");
          dist = dist-step*i;
          if(z.csolids) p.Scene.Tracker.Entities[typeof(Solid)] = oldSolids;
          goto start;
        }
      }

    exit:
      if(z.csolids) p.Scene.Tracker.Entities[typeof(Solid)] = oldSolids;
      return true;
  }

  static ILHook updateHook;
  static void ILUpdateHook(ILContext ctx){
    var c = new ILCursor(ctx);
    if(!c.TryGotoNextBestFit(MoveType.After, instr=>instr.MatchCallvirt<Player>("set_Ducking"),instr=>instr.MatchLdarg0())){
      goto bad;
    }
    ILCursor d = c.Clone();
    if(!d.TryGotoNextBestFit(MoveType.After, instr=>instr.MatchCall<Actor>("MoveV"),instr=>instr.MatchPop())){
      goto bad;
    }
    Instruction jumpTarget = d.Next;
    c.EmitDelegate(PlayerUpdateDetour2);
    c.Emit(OpCodes.Brtrue,jumpTarget);
    c.Emit(OpCodes.Ldarg_0);
    // for(int i=-10; i<30; i++){
    //   try{
    //     if(i==0) DebugConsole.Write("===========");
    //     DebugConsole.Write(c.Instrs[c.Index+i].ToString());
    //   }catch(Exception ex){
    //     DebugConsole.Write("cannot");
    //   }
    // }
    //DebugConsole.Write("Setup successfully");
    return;
    bad:
      DebugConsole.Write("Something went wrong while setting up player update hooks for anti0f");
  }
  public static HookManager hooks = new HookManager(()=>{
    MethodInfo update = typeof(Player).GetMethod(
      "orig_Update",
      BindingFlags.Public |BindingFlags.Instance
    );
    if(update == null){
      DebugConsole.Write("Could not find method");
    } else {
      updateHook = new ILHook(update, ILUpdateHook);
    }
  },void ()=>{
    updateHook.Dispose();
  },auspicioushelperModule.OnEnterMap);
}