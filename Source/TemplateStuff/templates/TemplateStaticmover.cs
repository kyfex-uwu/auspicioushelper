



using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateStaticmover")]
public class TemplateStaticmover:TemplateDisappearer, IMaterialObject{
  int smearamount;
  Vector2[] pastLiftspeed;
  bool averageSmear;
  string channel="";
  bool ridingTrigger;
  
  public TemplateStaticmover(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateStaticmover(EntityData d, Vector2 offset, int depthoffset):base(d.Attr("template",""),d.Position+offset,depthoffset,d.ID){
    smearamount = d.Int("liftspeed_smear",4);
    averageSmear = d.Bool("smear_average",false);
    channel = d.Attr("channel","");
    pastLiftspeed = new Vector2[smearamount];
    ridingTrigger = d.Bool("ridingTrigger",true);
    hooks.enable();
  }
  void evalLiftspeed(bool precess = true){
    float mX=0;
    float mY=0;
    if(!averageSmear)foreach(Vector2 v in pastLiftspeed){
      if(Math.Abs(v.X)>Math.Abs(mX)) mX=v.X;
      if(Math.Abs(v.Y)>Math.Abs(mY)) mY=v.Y;
    } else foreach(Vector2 v in pastLiftspeed){
      mX+=v.X/smearamount; mY+=v.Y/smearamount;
    }
    ownLiftspeed = new Vector2(mX,mY);
    if(!precess) return; 
    for(int i=smearamount-1; i>=1; i--){
      pastLiftspeed[i]=pastLiftspeed[i-1];
    }
    pastLiftspeed[0]=Vector2.Zero;
  }
  List<Entity> todraw;
  StaticMover sm;
  HashSet<Platform> doNot = new();
  public override void addTo(Scene scene){
    base.addTo(scene);
    setVisCol(false,false);
    CassetteMaterialLayer layer = null;
    if(channel != "")CassetteMaterialLayer.layers.TryGetValue(channel,out layer);
    List<Entity> allChildren = new();
    AddAllChildren(allChildren);
    foreach(Entity e in allChildren){
      if(e is Platform p) doNot.Add(p);
    }
    Add(sm = new StaticMover(){
      OnEnable=()=>{
        childRelposTo(virtLoc,Vector2.Zero);
        setVisCol(true,true);
        if(layer != null) layer.removeTrying(this);
      },
      OnDisable=()=>{
        setVisCol(false,false);
        cachedCol =false;
        if(layer !=null) layer.removeTrying(this);
      },
      OnAttach=(Platform p)=>{
        setVisCol(true,true);
      },
      SolidChecker=(Solid s)=>{
        //DebugConsole.Write(s.ToString());
        return s.Collidable && !doNot.Contains(s) && s.CollidePoint(Position);
      },
      OnDestroy=()=>{
        setVisCol(true,false);
        if(layer!=null) layer.removeTrying(this);
      },
      OnMove=(Vector2 move)=>{
        Position+=move;
        pastLiftspeed[0]+=move/Math.Max(Engine.DeltaTime,0.005f);
        evalLiftspeed();
        bool flag = false;
        if(cachedCol != getSelfCol() && move!=Vector2.Zero){
          flag = true;
          setCollidability(cachedCol);
        }
        childRelposTo(virtLoc,gatheredLiftspeed);
        if(flag)setCollidability(!cachedCol);
      }
    });
    if(layer!=null){
      todraw = new List<Entity>();
      AddAllChildren(todraw);
    }
  }
  public override void Update(){
    base.Update();
    evalLiftspeed(true);
    if(ridingTrigger){
      if(hasRiders<Player>()) sm.TriggerPlatform();
    }
  }
  public void renderMaterial(IMaterialLayer l, SpriteBatch s, Camera c){
    SpriteBatch origsb = Draw.SpriteBatch;
    Draw.SpriteBatch = s;
    foreach(Entity e in todraw) if(e.Scene != null && e.Depth<=l.depth)e.Render();
    Draw.SpriteBatch = origsb;
  }
  bool cachedCol;

  static bool MoveHPlatHook(On.Celeste.Platform.orig_MoveHExactCollideSolids orig, Platform p, 
    int moveH, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide
  ){
    foreach(StaticMover s in p.staticMovers){
      if(s.Entity is TemplateStaticmover s_){
        s_.cachedCol = s_.getSelfCol();
        s_.setCollidability(false);
      } 
    }
    bool res = orig(p, moveH, thruDashBlocks, onCollide);
    foreach(StaticMover s in p.staticMovers){
      if(s.Entity is TemplateStaticmover s_){
        s_.setCollidability(s_.cachedCol);
      } 
    }
    return res;
  }
  static bool MoveVPlatHook(On.Celeste.Platform.orig_MoveVExactCollideSolids orig, Platform p, 
    int moveH, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide
  ){
    foreach(StaticMover s in p.staticMovers){
      if(s.Entity is TemplateStaticmover s_){
        s_.cachedCol = s_.getSelfCol();
        s_.setCollidability(false);
      } 
    }
    bool res = orig(p, moveH, thruDashBlocks, onCollide);
    foreach(StaticMover s in p.staticMovers){
      if(s.Entity is TemplateStaticmover s_){
        s_.setCollidability(s_.cachedCol);
      } 
    }
    return res;
  }
  static HookManager hooks = new HookManager(()=>{
    On.Celeste.Platform.MoveHExactCollideSolids += MoveHPlatHook;
    On.Celeste.Platform.MoveVExactCollideSolids += MoveVPlatHook;
  },()=>{
    On.Celeste.Platform.MoveHExactCollideSolids -= MoveHPlatHook;
    On.Celeste.Platform.MoveVExactCollideSolids -= MoveVPlatHook;

  },auspicioushelperModule.OnEnterMap);
}