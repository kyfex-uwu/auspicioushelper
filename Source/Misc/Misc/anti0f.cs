



using System;
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
  const float maxstep = 4;
  public Anti0fZone(EntityData d, Vector2 offset):base(d.Position+offset){
    bounds = new FloatRect(Position.X,Position.Y,d.Width,d.Height);
    hooks.enable();
  }


  public static bool PlayerUpdateDetour(Player p){
    //DebugConsole.Write("In update hook");
    int state = p.StateMachine.state;
    if(state == 9 || state == 22) return false;
    Vector2 ospeed = p.Speed;
    Vector2 step = ospeed*Engine.DeltaTime;
    FloatRect r = new FloatRect(p);
    bool flag = false;
    foreach(Anti0fZone a in p.Scene.Tracker.GetEntities<Anti0fZone>()){
      if(a.bounds.CollideRectSweep(r,step)){
        flag = true;
        break;
      }
    }
    if(!flag){
      return false;
    }
    //We use L1 distance
    float length = Math.Max(Math.Abs(step.X),Math.Abs(step.Y));
    int steps = (int)Math.Ceiling(length/maxstep);
    DebugConsole.Write("Inside 0f "+length.ToString()+" "+steps.ToString());
    Vector2 substep = step/(float)steps;
    for(int i=0; i<steps; i++){
      if(p.StateMachine.state != state || p.Speed.Y!=ospeed.Y || p.Speed.X!=ospeed.X || p.Dead) return true;
      if(i!=0){
        if(state != 18) foreach(Trigger t in p.Scene.Tracker.GetEntities<Trigger>()){
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
        Collider g = p.Collider;
        p.Collider=p.hurtbox;
        if(!p.Dead && state!=21)foreach(PlayerCollider c in p.Scene.Tracker.GetComponents<PlayerCollider>()){
          if(c.Check(p) && p.Dead){
            p.Collider=g;
            return true;
          }
        }
        p.Collider=g;
        
      }
      p.MoveH(substep.X,p.onCollideH,null);
      p.MoveV(substep.Y,p.onCollideV,null);
    }
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
    c.EmitDelegate(PlayerUpdateDetour);
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