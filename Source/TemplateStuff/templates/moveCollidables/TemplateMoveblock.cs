



using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;
[CustomEntity("auspicioushelper/TemplateMoveblock")]
public class TemplateMoveBlock:TemplateMoveCollidable{
  public Vector2 movedir;
  public float maxspeed;
  public float acceleration;
  bool respawning;
  float maxrespawntimer;
  float maxStuckTime;
  bool triggered;
  bool cansteer;
  int maxleniency;
  Vector2 origpos;
  public TemplateMoveBlock(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateMoveBlock(EntityData d, Vector2 offset, int depthoffset)
  :base(d,offset+d.Position,depthoffset){
    movedir = d.Attr("direction") switch{
      "down"=> Vector2.UnitY,
      "up"=>-Vector2.UnitY,
      "left"=>-Vector2.UnitX,
      "right"=>Vector2.UnitX,
      _=>Vector2.UnitX
    };
    useOwnUncollidable = d.Bool("uncollidable_blocks",false);
    maxspeed = d.Float("speed",75);
    acceleration = d.Float("acceleration", 300);
    respawning = d.Bool("respawning",true);
    maxrespawntimer = d.Float("respawn_timer",2f);
    maxStuckTime = d.Float("Max_stuck",0.15f);
    cansteer = d.Bool("cansteer", false);
    maxleniency = d.Int("max_leniency",4);
    origpos = Position;
    prop &= ~Propagation.Riding;
  }
  public override void Awake(Scene scene) {
    base.Awake(scene);
    Add(movesfx = new SoundSource());
    Add(new Coroutine(Sequence()));
  }

  SoundSource movesfx;
  IEnumerator Sequence(){
    float stuckTimer;
    float speed;
    float steerdelay;
    waiting:
      stuckTimer = maxStuckTime;
      steerdelay = 0.2f;
      while(!triggered && !hasRiders<Player>()) yield return null;
      disconnect();
      speed = 0;
      Audio.Play("event:/game/04_cliffside/arrowblock_activate", Position);
      yield return 0.2f;
      movesfx.Play("event:/game/04_cliffside/arrowblock_move");
      movesfx.Param("arrow_stop", 0f);
    moving:
      yield return null;
      speed = Calc.Approach(speed,maxspeed,acceleration*Engine.DeltaTime);
      Query qs = getq(new Vector2(speed+maxleniency,speed+maxleniency).Ceiling());
      bool collideflag = false;
      Vector2 movevec = movedir;
      Player p = Scene.Tracker.GetEntity<Player>();
      if(movedir.Y==0){
        if(cansteer && p != null && p.StateMachine.state == 0 && Input.MoveY.Value!=0 && hasRiders<Player>()){
          if(steerdelay>0){
            steerdelay-=Engine.DeltaTime;
          } else {
            movevec.Y = Input.MoveY.Value;
          }
        } else {
          steerdelay=0.1f;
        }
        movevec.Normalize();
        ownLiftspeed = movevec*speed;
        collideflag = MoveHCollide(qs,speed*movevec.X*Engine.DeltaTime,maxleniency,ownLiftspeed);
        if(collideflag) ownLiftspeed.X=0;
        if(MoveVCollide(qs,speed*movevec.Y*Engine.DeltaTime,0,ownLiftspeed)) ownLiftspeed.Y=0;
      } else {
        if(cansteer && p != null && p.StateMachine.state == 1 && Input.MoveX.Value!=0 && hasRiders<Player>()){
          if(steerdelay>0){
            steerdelay-=Engine.DeltaTime;
          } else {
            movevec.X = Input.MoveX.Value;
          }
        } else {
          steerdelay=0.1f;
        }
        movevec.Normalize();
        ownLiftspeed = movevec*speed;
        collideflag = MoveVCollide(qs,speed*movevec.Y*Engine.DeltaTime,maxleniency,ownLiftspeed);
        if(collideflag) ownLiftspeed.Y=0;
        if(MoveHCollide(qs,speed*movevec.X*Engine.DeltaTime,0,ownLiftspeed)) ownLiftspeed.X=0;
      }
      if(collideflag){
        movesfx.Param("arrow_stop", 1f);
        if(stuckTimer>0) stuckTimer-=Engine.DeltaTime;
        else goto blocked;
      } else {
        movesfx.Param("arrow_stop", 0f);
        stuckTimer = maxStuckTime;
      }
      goto moving;
    blocked:
      movesfx.Stop();
      Audio.Play("event:/game/04_cliffside/arrowblock_break", Position);
      if(!respawning){
        destroy(true);
        yield break;
      } 
      destroyChildren();
      fgt = null;
      yield return maxrespawntimer;
      Scene old = Scene;
      Scene = null;
      reconnect(origpos);
      addTo(old);
      Audio.Play("event:/game/04_cliffside/arrowblock_reappear", Position);
      goto waiting;
  }
  public override void Removed(Scene scene) {
    base.Removed(scene);
    movesfx.Stop();
  }
}