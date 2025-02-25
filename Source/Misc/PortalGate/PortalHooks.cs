


using System;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.auspicioushelper;

public struct FloatRect{
  public float x;
  public float y;
  public float w;
  public float h;
  public Vector2 tlc{
    get=>new Vector2(x,y);
  }
  public Vector2 brc{
    get=>new Vector2(x+w,y+h);
  }
  public Vector2 center {
    get=>new Vector2(x+w/2,y+h/2);
  }
  public Vector2 radius{
    get=>new Vector2(w/2,h/2);
  }
  public FloatRect(float x,float y, float w, float h){
    this.x=x; this.y=y; this.w=w; this.h=h;
  }
  public bool CollidePoint(Vector2 p){
    return p.X>=x && p.Y>=y && p.X<x+w && p.Y<y+h;
  }
  public bool CollideLine(Vector2 a, Vector2 b){
    Vector2 dir = b-a;
    Vector2 t1 = tlc/dir;
    Vector2 t2 = brc/dir;
    Vector2 maxs = Vector2.Max(t1,t2);
    Vector2 mins = Vector2.Min(t1,t2);
    float min = Math.Max(mins.X,mins.Y);
    float max = Math.Min(maxs.X,maxs.Y);
    return min<max && (min<1 || max>0);
  }
  public bool CollideCircle(Vector2 p, float r){
    p=p-center;
    Vector2 d=Vector2.Max(Vector2.Zero,new Vector2(Math.Abs(p.X)-w,Math.Abs(p.Y)-h));
    return d.X*d.X+d.Y*d.Y<r*r;
  }
  public bool CollideExRect(float ox, float oy, float ow, float oh){
    return x+w>ox && y+h>oy && x<ox+ow && y<oy+oh;
  }
  public Rectangle munane(){
    return new Rectangle((int) x, (int) y, (int) w, (int) h);
  }
  public override string ToString(){
      return "FloatRect:{"+string.Format("x:{0}, y:{1}, w:{2}, h:{3}",x,y,w,h)+"} ";
  }
}

public static class portalHooks{
  public static bool setup=false;
  /*static Hook absoluteLeftHook;
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
      //if(r.Width<=0)DebugConsole.Write(r.ToString()+" "+PortalGateH.collideLim[h.Entity]);
      return r;
    }
    return orig(self);
  }*/

  private static void HitboxRender(On.Monocle.Hitbox.orig_Render orig, Hitbox h, Camera camera, Color color){
    //DebugConsole.Write("Rendig");
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      Rectangle a=r1.munane();
      Rectangle b=r2.munane();
      Draw.HollowRect(a.X,a.Y,a.Width,a.Height, color);
      Draw.HollowRect(b.X,b.Y,b.Width,b.Height, color);
    } else {
      orig(h,camera, color);
    }
  }
  private static bool HitboxCollidePoint(On.Monocle.Hitbox.orig_Collide_Vector2 orig, Hitbox h, Vector2 p){
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return r1.CollidePoint(p) || r2.CollidePoint(p);
    } else {
      return orig(h,p);
    }
  }
  private static bool HitboxCollideLine(On.Monocle.Hitbox.orig_Collide_Vector2_Vector2 orig, Hitbox h, Vector2 a, Vector2 b){
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return r1.CollideLine(a,b) || r2.CollideLine(a,b);
    } else {
      return orig(h,a,b);
    }
  }

  private static bool HitboxCollideCircle(On.Monocle.Hitbox.orig_Collide_Circle orig, Hitbox h, Circle c){
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return r1.CollideCircle(c.Position,c.Radius) || r2.CollideCircle(c.Position,c.Radius);
    } else {
      return orig(h,c);
    }
  }
  private static bool HitboxIsect(On.Monocle.Hitbox.orig_Intersects_float_float_float_float orig, Hitbox h, float x, float y, float w, float v){
    //DebugConsole.Write("Hi ICT");
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return r1.CollideExRect(x,y,w,v) || r2.CollideExRect(x,y,w,v);
    } else {
      return orig(h,x,y,w,v); 
    }
  }
  private static bool HitboxCollideRect(On.Monocle.Hitbox.orig_Collide_Rectangle orig, Hitbox h, Rectangle r){
    //DebugConsole.Write("Hi RHB");
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return r1.CollideExRect(r.X,r.Y,r.Width,r.Height) || r2.CollideExRect(r.X,r.Y,r.Width,r.Height);
    } else {
      return orig(h,r);
    }
  }
  private static bool HitboxIsectHb(On.Monocle.Hitbox.orig_Intersects_Hitbox orig, Hitbox h, Hitbox o){
    //We still need to test o - note that unwrapping h is a must
    //DebugConsole.Write("Hi IHB");
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return o.Intersects(r1.x,r1.y,r1.w,r1.h) || o.Intersects(r2.x,r2.y,r2.w,r2.h);
    } else {
      return o.Intersects(h.AbsoluteLeft,h.AbsoluteTop,h.Width,h.Height);
    }
  }
  /*private static bool HitboxCollideHitbox(On.Monocle.Hitbox.orig_Collide_Hitbox orig, Hitbox h, Circle c){

  }*/
  private static bool HitboxCollideGrid(On.Monocle.Hitbox.orig_Collide_Grid orig, Hitbox h, Grid g){
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      //DebugConsole.Write(r1.ToString()+" "+r2.ToString());
      return g.Collide(r1.munane()) || g.Collide(r2.munane());
    } else {
      return orig(h,g);
    }
  }
  private static bool GridCollideHitbox(On.Monocle.Grid.orig_Collide_Hitbox orig, Grid g, Hitbox h){
    if(PortalGateH.intersections.TryGetValue(h.Entity, out var info)){
      info.getAbsoluteRects(h, out var r1, out var r2);
      return g.Collide(r1.munane()) || g.Collide(r2.munane());
    } else {
      return orig(g,h);
    }
  }
  public static void setupHooks(){
    setup=true;
    //On.Celeste.Actor.ctor+=ActorCtorHook;
    /*MethodInfo colliderAbsLeft = typeof(Collider).GetMethod(
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
    boundsHook = new Hook(colliderBounds, Bounds_Detour);*/

    On.Monocle.Hitbox.Render+=HitboxRender;
    On.Monocle.Hitbox.Intersects_float_float_float_float += HitboxIsect;
    On.Monocle.Hitbox.Intersects_Hitbox += HitboxIsectHb;
    On.Monocle.Hitbox.Collide_Vector2 += HitboxCollidePoint;
    On.Monocle.Hitbox.Collide_Vector2_Vector2 += HitboxCollideLine;
    On.Monocle.Hitbox.Collide_Rectangle +=HitboxCollideRect;
    On.Monocle.Hitbox.Collide_Circle += HitboxCollideCircle;
    On.Monocle.Hitbox.Collide_Grid += HitboxCollideGrid;
    On.Monocle.Grid.Collide_Hitbox += GridCollideHitbox;

    On.Celeste.Actor.MoveHExact+=PortalGateH.ActorMoveHHook;
  }
  
  public static void unsetupHooks(){
    setup=false;
    On.Monocle.Hitbox.Render -= HitboxRender;
    On.Monocle.Hitbox.Intersects_float_float_float_float -= HitboxIsect;
    On.Monocle.Hitbox.Intersects_Hitbox -= HitboxIsectHb;
    On.Monocle.Hitbox.Collide_Vector2 -= HitboxCollidePoint;
    On.Monocle.Hitbox.Collide_Vector2_Vector2 -= HitboxCollideLine;
    On.Monocle.Hitbox.Collide_Rectangle -=HitboxCollideRect;
    On.Monocle.Hitbox.Collide_Circle -= HitboxCollideCircle;
    On.Monocle.Hitbox.Collide_Grid -= HitboxCollideGrid;
    On.Monocle.Grid.Collide_Hitbox -= GridCollideHitbox;

    On.Celeste.Actor.MoveHExact-=PortalGateH.ActorMoveHHook;
    //absoluteLeftHook.Dispose();
    //absoluteRightHook.Dispose();
    //boundsHook.Dispose();
  }
}