


using System;
using System.Collections.Generic;
using Celeleste.Mods.auspicioushelper;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/PortalGateH")]
[Tracked]
public class PortalGateH:Entity{
  private MTexture texture = GFX.Game["util/lightbeam"];
  public NoiseSamplerOS2_2DLoop ogen = new NoiseSamplerOS2_2DLoop(20, 70, 100);
  public static Dictionary<Actor, PortalIntersectInfoH> intersections = new Dictionary<Actor, PortalIntersectInfoH>();
  public static Dictionary<Entity, Vector2> collideLim = new Dictionary<Entity, Vector2>();
  public float height;
  public Vector2 npos;
  public float top1 {
    get=>Position.Y;
  }
  public float bottom1 {
    get=>Position.Y+height;
  }
  public float top2 {
    get=>npos.Y;
  }
  public float bottom2{
    get=>npos.Y+height;
  }
  public float x1{
    get=>Position.X;
  }
  public float x2{
    get=>npos.X;
  }
  public Vector2[] v1s = {Vector2.Zero, Vector2.Zero};
  public Vector2[] v2s = {Vector2.Zero, Vector2.Zero};
  public Solid s1=null;
  public Solid s2=null;
  public bool flipped;
  public List<uint> handles = new List<uint>();
  public PortalGateH(EntityData d, Vector2 offset):base(d.Position+offset){
    if(!portalHooks.setup) portalHooks.setupHooks();
    Depth=-9998;
    height = d.Height+0.999f;
    npos=d.Nodes[0]+offset;
    flipped = d.Bool("flipped",false);
    for(int i=0; i<height; i+=2){
      handles.Add(ogen.getHandle());
      handles.Add(ogen.getHandle());
    }
    if(d.Bool("attached",true)){
      Add(new StaticMover(){
        OnMove = (Vector2 amount)=>{
          Position+=amount;
          v1s[0]=amount/Math.Max(Engine.DeltaTime,0.005f);;
        },
        SolidChecker = (Solid solid)=>{
          bool c = solid.CollideRect(new Rectangle((int)Math.Floor(Position.X)-1, (int)Position.Y, 1, (int)height));
          bool d = solid.CollideRect(new Rectangle((int)Math.Floor(Position.X), (int)Position.Y, 1, (int)height));
          if(c!=d)s1=solid;
          return c!=d;
        }
      });
      Add(new StaticMover(){
        OnMove = (Vector2 amount)=>{
          npos+=amount;
          v2s[0]=amount/Math.Max(Engine.DeltaTime,0.005f);
        },
        SolidChecker = (Solid solid)=>{
          bool c = solid.CollideRect(new Rectangle((int)Math.Floor(npos.X)-1, (int)npos.Y, 1, (int)height));
          bool d = solid.CollideRect(new Rectangle((int)Math.Floor(npos.X), (int)npos.Y, 1, (int)height));
          if(c!=d)s2=solid;
          return c!=d;
        }
      });
    }
  }
  public static bool ActorMoveHHook(On.Celeste.Actor.orig_MoveHExact orig, Actor a, int moveH, Collision onCollide, Solid pusher){
    if(a is PortalOthersider m){
      //DebugConsole.Write("dummy "+moveH.ToString());
      m.info.applyDummyPush(new Vector2(moveH, 0));
      return false;
    } else {
      //DebugConsole.Write(moveH.ToString());
      float left = float.NegativeInfinity; 
      float right = float.PositiveInfinity;
      PortalGateH pleft = null;
      PortalGateH pright = null;
      bool leftNode=false;
      bool rightNode=false;
      float center = (a.Left+a.Right)/2;
      foreach(PortalGateH p in a.Scene.Tracker.GetEntities<PortalGateH>()){
        if(p.top1<=a.Top && p.bottom1>=a.Bottom){
          if(p.x1<=center && left<p.x1){
            left=p.x1; pleft = p; leftNode=false;
          }
          if(p.x1>center && right>p.x1){
            right = p.x1; pright = p; rightNode=false;
          }
        }
        if(p.top2<=a.Top && p.bottom2>=a.Bottom){
          if(p.x2<=center && left<p.x2){
            left=p.x2; pleft = p; leftNode=true;
          }
          if(p.x2>center && right>p.x2){
            right = p.x2; pright = p; rightNode=true;
          } 
        }
      }
      collideLim[a] = new Vector2(left,right);
      if(moveH==0) return false;
      PortalIntersectInfoH info=null;
      if(intersections.TryGetValue(a,out info)){

      } else if(a.Left+moveH<=left) {
        intersections[a]=(info = new PortalIntersectInfoH(leftNode, pleft,a));
        info.addOthersider();
      } else if(a.Right+moveH>=right){
        intersections[a]=(info = new PortalIntersectInfoH(rightNode, pright, a));
        info.addOthersider();
      }
      //DebugConsole.Write(collideLim[a].ToString()+" "+moveH.ToString()+ " "+intersections.Count);
      
      
      bool val = orig(a,moveH,onCollide,pusher);

      if(info != null && info.finish()) intersections.Remove(a); 
      return val;
    }
  }
  public Vector2 getpos(bool node){
    return node?npos:Position;
  }
  public Vector2 getspeed(bool node){
    Vector2[] s=node?v2s:v1s;
    return Vector2.Max(s[0],s[1]);
  }


  public override void Render(){
    base.Render();
    for(int i=4; i<height-4; i+=4){
      float alpha = ogen.sample(handles[i])/2;
      if(alpha<0) continue;
      float length = ogen.sample(handles[i+1])*10+20;
      texture.Draw(npos+new Vector2(0,i), new Vector2(0f, 0.5f), new Color(1f,1f,1f,1f)*alpha, new Vector2(1f / (float)texture.Width * length, 8), 0);
    }
    for(int i=4; i<height-4; i+=4){
      float alpha = ogen.sample(handles[i])/2;
      if(alpha<0) continue;
      float length = ogen.sample(handles[i+1])*10+20;
      texture.Draw(Position+new Vector2(0,i), new Vector2(0f, 0.5f), new Color(1f,1f,1f,1f)*alpha, new Vector2(-1f / (float)texture.Width * length, 8), 0);
    }
  }
  public override void Update(){
    base.Update();
    ogen.update(Engine.DeltaTime);
    if(Engine.DeltaTime!=0){
      for(int i=v1s.Length-1; i>0; i--){
        v1s[i]=v1s[i-1];
        v2s[i]=v2s[i-1];
      }
      v1s[0]=Vector2.Zero;
      v2s[0]=Vector2.Zero;
    }
  }
}