


using System;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Collections.Generic;

namespace Celeste.Mod.auspicioushelper;

public static class portalHooks{
  public static bool setup=false;
  static Hook absoluteLeftHook;
  static Hook absoluteRightHook;
  static Hook boundsHook;
  private static float AbsoluteLeft_Detour(Func<Collider, float> orig, Collider self) {
    float o = orig(self);
    if(self is Hitbox h){
      if(PortalGateH.collideLim.TryGetValue(h.Entity, out var l)){
        //DebugConsole.Write(l.X.ToString()+" "+o.ToString());
        return Math.Max(l.X,o);
      }
    }
    return o;
  }
  private static float AbsoluteRight_Detour(Func<Collider, float> orig, Collider self) {
    float o = orig(self);
    if(self is Hitbox h){
      if(PortalGateH.collideLim.TryGetValue(h.Entity, out var l)){
        //DebugConsole.Write(l.Y.ToString()+" "+o.ToString());
        return Math.Min(l.Y,o);
      }
    }
    return o;
  }
  private static Rectangle Bounds_Detour(Func<Collider, Rectangle> orig, Collider self){
    if(self is Hitbox h){
      Rectangle r = new Rectangle((int)self.AbsoluteLeft, (int)self.AbsoluteTop, 
      (int)self.AbsoluteRight-(int)self.AbsoluteLeft, (int)self.AbsoluteBottom-(int)self.AbsoluteTop);
      if(r.Width<=0)DebugConsole.Write(r.ToString()+" "+PortalGateH.collideLim[h.Entity]);
      return r;
    }
    return orig(self);
  }
  public static void setupHooks(){
    setup=true;
    //On.Celeste.Actor.ctor+=ActorCtorHook;
    MethodInfo colliderAbsLeft = typeof(Collider).GetMethod(
        "get_AbsoluteLeft",
        BindingFlags.Instance | BindingFlags.Public
    );
    absoluteLeftHook = new Hook(colliderAbsLeft,AbsoluteLeft_Detour);
    MethodInfo colliderAbsRight = typeof(Collider).GetMethod(
        "get_AbsoluteRight",
        BindingFlags.Instance | BindingFlags.Public
    );
    absoluteRightHook = new Hook(colliderAbsRight, AbsoluteRight_Detour);
    MethodInfo colliderBounds = typeof(Collider).GetMethod(
        "get_Bounds",
        BindingFlags.Instance | BindingFlags.Public
    );
    boundsHook = new Hook(colliderBounds, Bounds_Detour);


    On.Celeste.Actor.MoveHExact+=PortalGateH.ActorMoveHHook;
  }
  
  public static void unsetupHooks(){
    setup=false;
    On.Celeste.Actor.MoveHExact-=PortalGateH.ActorMoveHHook;
    absoluteLeftHook.Dispose();
    absoluteRightHook.Dispose();
    boundsHook.Dispose();
  }
}