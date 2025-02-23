


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
  public class SurroundingInfoH {
    public PortalGateH left=null;
    public PortalGateH right=null;
    public Actor actor=null;
    public float leftl=float.NegativeInfinity;
    public float rightl=float.PositiveInfinity;
    public bool leftn;
    public bool rightn;
    public void setl(PortalGateH p, bool node){
      left = p; leftl = node?p.x2:p.x1; leftn = node;
    }
    public void setr(PortalGateH p, bool node){
      right = p; rightl = node?p.x2:p.x1; rightn = node;
    }
  }
  public static Dictionary<Entity,SurroundingInfoH> portalInfos = new Dictionary<Entity, SurroundingInfoH>();
  public static SurroundingInfoH evalEnt(Actor a){
    if(a is PortalOthersider o){
      return null;
    }
    SurroundingInfoH s = new SurroundingInfoH{actor=a};
    foreach(PortalGateH p in a.Scene.Tracker.GetEntities<PortalGateH>()){
      if(p.top1<=a.Top && p.bottom1>=a.Bottom){
        if(p.n1dir){
          if(a.Right>=p.x1 && s.leftl<p.x1) s.setl(p,false);
        } else {
          if(a.Left<=p.x1 && s.rightl>p.x1) s.setr(p,false);
        }
      }
      if(p.top2<=a.Top && p.bottom2>=a.Bottom){
        if(p.n2dir){
          if(a.Right>=p.x2 && s.leftl<p.x2) s.setl(p,true);
        } else {
          if(a.Left<=p.x2 && s.rightl>p.x2) s.setr(p,true);
        }
      }
    }
    portalInfos[a]=s;
    collideLim[a]=new Vector2(s.leftl,s.rightl);
    return s;
  }
  public static void SceneUpdateHook(On.Monocle.Scene.orig_Update orig, Scene scene){
    if(scene is Level){
      collideLim.Clear();
      portalInfos.Clear();
      foreach(Actor a in scene.Tracker.GetEntities<Actor>()){
        evalEnt(a);
      }
    }
    orig(scene);
  }
  public void movePortalPos(Vector2 amount){
    float oldx=x1;
    Position+=amount;
    v1s[0]=amount/Math.Max(Engine.DeltaTime,0.005f);

    /*foreach(Actor a in Scene.Tracker.GetEntities<Actor>()){
      if(!a.Active) continue;
      if(a is PortalOthersider o){
        continue;
      }
      SurroundingInfoH s=null;
      if(portalInfos.TryGetValue(a,out s)){
        if(((n1dir? s.left:s.right) == this) && ((n1dir?s.leftn:s.rightn)==false)) evalEnt(a);
      } else {
        s=evalEnt(a);
      }

    }*/
  }
  public void movePortalNpos(Vector2 amount){

  }

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
  public bool n1dir;
  public bool n2dir;
  public List<uint> handles = new List<uint>();
  public Color color;
  public PortalGateH(EntityData d, Vector2 offset):base(d.Position+offset){
    if(!portalHooks.setup) portalHooks.setupHooks();
    Depth=-9998;
    height = d.Height+0.999f;
    npos=d.Nodes[0]+offset;
    n1dir = d.Bool("right_facing_f0",false);
    n2dir = d.Bool("right_facing_f1",true);
    color = Util.hexToColor(d.Attr("color_hex","#FFFA"));
    DebugConsole.Write(color.ToString()+" "+x1.ToString()+" "+x2.ToString());
    flipped = n1dir==n2dir;
    
    for(int i=0; i<height; i+=2){
      handles.Add(ogen.getHandle());
      handles.Add(ogen.getHandle());
    }
    if(d.Bool("attached",false)){
      Add(new StaticMover(){
        OnMove = movePortalPos,
        SolidChecker = (Solid solid)=>{
          bool c = solid.CollideRect(new Rectangle((int)Math.Floor(Position.X)-1, (int)Position.Y, 1, (int)height));
          bool d = solid.CollideRect(new Rectangle((int)Math.Floor(Position.X), (int)Position.Y, 1, (int)height));
          if(c!=d)s1=solid;
          return c!=d;
        }
      });
      Add(new StaticMover(){
        OnMove = (Vector2 amount)=>{
          //DebugConsole.Write("V2 "+amount.ToString());
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
      if(m.Scene == null){
        DebugConsole.Write("Moving removed entity");
        return true;
      }
      //DebugConsole.Write(a.Collider.Bounds.ToString()+collideLim[m].ToString());
      m.info.applyDummyPush(new Vector2(moveH, 0));
      return orig(a,moveH,onCollide,pusher);
    } else {
      SurroundingInfoH s = evalEnt(a);
      PortalIntersectInfoH info=null;
      if(intersections.TryGetValue(a,out info)){
      } else if(a.Left+moveH<=s.leftl) {
        intersections[a]=(info = new PortalIntersectInfoH(s.leftn, s.left,a));
        PortalOthersider mn = info.addOthersider();
        collideLim[mn] = s.left.getSidedCollidelim(!s.leftn);
      } else if(a.Right+moveH>=s.rightl){
        intersections[a]=(info = new PortalIntersectInfoH(s.rightn, s.right, a));
        PortalOthersider mn = info.addOthersider();
        collideLim[mn] = s.right.getSidedCollidelim(!s.rightn);
      }
      //DebugConsole.Write(collideLim[a].ToString()+" "+moveH.ToString()+ " "+intersections.Count);
      int res = moveH;
      if(info != null){
        //DebugConsole.Write("Moving Dummy");
        res = info.m.tryMoveH(moveH,onCollide,pusher);
      }
      
      bool val = orig(a,res,onCollide,pusher);

      if(info != null && info.finish()) intersections.Remove(a); 
      return val || (res != moveH);
    }
  }
  public static bool ActorMoveVHook(On.Celeste.Actor.orig_MoveVExact orig, Actor a, int moveV, Collision onCollide, Solid pusher){
    if(a is PortalOthersider m){
      if(m.Scene == null){
        DebugConsole.Write("Moving removed entity");
        return true;
      }
      m.info.applyDummyPush(new Vector2(0, moveV));
      return orig(a,moveV,onCollide,pusher);
    } else {
      evalEnt(a);
      if(intersections.TryGetValue(a,out var info)){
        int res = info.m.tryMoveV(moveV,onCollide,pusher);
        bool val = res!=moveV;
        bool val2 = orig(a,res,onCollide,pusher);
        if(val2){
          info.m.Center = info.getOthersiderPos();
        }
        return val || val2;
      } else {
        return orig(a,moveV,onCollide,pusher);
      }
    }
  }
  public Vector2 getpos(bool node){
    return node?npos:Position;
  }
  public Vector2 getspeed(bool node){
    Vector2[] s=node?v2s:v1s;
    return Vector2.Max(s[0],s[1]);
  }
  public Vector2 getSidedCollidelim(bool node){
    Vector2 b = new Vector2(float.NegativeInfinity,float.PositiveInfinity);
    float x = node?x2:x1;
    if(node?n2dir:n1dir){
      b.X=x;
    } else {
      b.Y=x;
    }
    return b;
  }


  public override void Render(){
    base.Render();
    Vector2 offset = new Vector2(0f, 0.5f);
    float wrec1 = (n1dir?1f:-1f) / (float)texture.Width;
    float wrec2 = (n2dir?1f:-1f) / (float)texture.Width;
    for(int i=4; i<height-4; i+=4){
      float alpha = Math.Min(1,Math.Max(0,ogen.sample(handles[i]))+0.2f);
      if(alpha<0) continue;
      float length = ogen.sample(handles[i+1])*10+20;
      texture.Draw(Position+new Vector2(0,i+2), offset, color*alpha, new Vector2(wrec1 * length, 8), 0);
      texture.Draw(npos+new Vector2(0,i), offset, color*alpha, new Vector2(wrec2 * length, 8), 0);
    }
  }
  public override void Update(){
    base.Update();
    ogen.update(Engine.DeltaTime);
    //DebugConsole.Write(v1s[0].ToString());
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