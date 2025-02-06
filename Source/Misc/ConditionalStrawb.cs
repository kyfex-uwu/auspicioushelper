


using System;
using System.Collections;
using System.Linq;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.auspicioushelper;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeleste.Mods.auspicioushelper;

[CustomEntity("auspicioushelper/ConditionalStrawb")]
[RegisterStrawberry(true, true)]
[Tracked]
public class ConditionalStrawb:Entity, IStrawberry{
  public static ConditionalStrawb carryingDeathless = null;
  public string idstr;
  public Follower follower;
  public EntityID id;
  public bool isGhost;
  public bool wingedFollower;
  public bool winged;
  Vector2? otherpos;
  public float ispeed;
  public float maxspeed;
  public float acceleration;
  public bool golden;
  public enum Strawb {
    hidden, idling, following, flying, gone, collected
  }
  public Strawb state;
  public Sprite sprite;
  public EntityData data;
  public bool deathless;
  public ConditionalStrawb(EntityData data, Vector2 offset, EntityID id):base(data.Position+offset){
    idstr = data.Attr("strawberry_id");
    this.id=id;
    isGhost = SaveData.Instance.CheckStrawberry(id);
    base.Collider = new Hitbox(14f, 14f, -7f, -7f);
    Add(new PlayerCollider(OnPlayer));
    Add(new MirrorReflection());
    Add(follower = new Follower(id, null, OnLoseLeader));
    follower.FollowDelay = 0.3f;
    otherpos=data.Nodes.Count() == 0?null:data.Nodes[0]+offset;

    wingedFollower= data.Bool("wingedfollower",true);
    winged = data.Bool("winged",true);
    Add(new DashListener(OnDash));
    ispeed=data.Float("ispeed",1.0f);
    maxspeed=data.Float("maxspeed",240.0f);
    acceleration=data.Float("acceleration",240f);
    this.data=data;

    deathless=data.Bool("deathless",false);
  }
  public Strawb handleAppearance(EntityData e){
    if(!e.Bool("appear_on_ch",false)) return Strawb.idling;
    int v=e.Int("appear_chvalue",0);
    string ch = e.Attr("appear_channel","");
    bool c=ChannelState.readChannel(ch)==v;
    if(c || e.Bool("appear_roomenter_only",true)){
      return c?Strawb.idling:Strawb.hidden;
    }
    Add(new ChannelTracker(ch,(int val)=>{
      if(val==v && state==Strawb.hidden){
        state=Strawb.idling;
        Collidable = true;
        sprite.Visible=true;
      } 
    }));
    return Strawb.hidden;
  }
  public override void Added(Scene scene){
    base.Added(scene);
    Add(sprite = auspicioushelperGFX.spriteBank.Create("conditionalstrawb"));
    if(isGhost) sprite.Color = new Color(100,100,255,144);
    state = handleAppearance(data);
    if(state == Strawb.hidden){
      Collidable=false;
      sprite.Visible=false;
    }
    if(data.Bool("fly_on_ch",false)){
      string ch = data.Attr("fly_channel","");
      int v = data.Int("fly_value",1);
      if(ChannelState.readChannel(ch) == v) Fly();
      Add(new ChannelTracker(ch,(int val)=>{
        if(val == v) Fly();
      }));
    }
    //sprite.Play("silverflap");
  }
  public void Fly(){
    Add(new Coroutine(Fly(otherpos??new Vector2(Position.X,Position.Y-1000000))));
  }
  public void OnDash(Vector2 dir){
    if(winged && state==Strawb.idling && (otherpos is Vector2 o) &&Vector2.Distance(o,Position)>1.0f){
      Fly();
    }
    if(wingedFollower && state == Strawb.following){
      Fly();
    }
  }
  public void OnPlayer(Player p){
    state=Strawb.following;
    if(deathless && carryingDeathless == null) carryingDeathless = this;
    Collidable=false;
    Audio.Play(isGhost ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", Position);
    p.Leader.GainFollower(follower);
  }
  public void OnCollect(){
    if(state != Strawb.following) return;
    Player p = follower.Leader.Entity as Player;
    int idx = p.StrawberryCollectIndex;
    p.StrawberryCollectIndex++;
    p.StrawberryCollectResetTimer = 2.5f;
    follower.Leader.LoseFollower(follower);
    SaveData.Instance.AddStrawberry(id, golden);
    state=Strawb.collected;
    Session session = (base.Scene as Level).Session;
    session.DoNotLoad.Add(id);
    session.Strawberries.Add(id);
    Add(new Coroutine(CollectRoutine(idx)));
  }
  public IEnumerator CollectRoutine(int idx){
    Tag = Tags.TransitionUpdate;
    Audio.Play("event:/game/general/strawberry_get", Position, "colour", 1, "count", idx);
    sprite.Play("collect");
    while (sprite.Animating){
        yield return null;
    }
    Scene.Add(new StrawberryPoints(Position, isGhost, idx, false));
    RemoveSelf();
  }
  public void OnLoseLeader(){
    if(state==Strawb.collected) return;

  }
  public IEnumerator Fly(Vector2 target){
    if(state==Strawb.following){
      follower.Leader.Followers.Remove(follower);
      follower.OnLoseLeaderUtil();
      follower.Leader=null;
    }
    state = Strawb.flying;
    if(deathless && carryingDeathless==this)carryingDeathless=null;
    yield return 0.1f;
    Audio.Play("event:/game/general/strawberry_laugh", Position);
    yield return 0.2f;
    if (!follower.HasLeader){
      Audio.Play("event:/game/general/strawberry_flyaway", Position);
    }
    float speed=ispeed;
    while(true){
      Position = Calc.Approach(Position,target,speed*Engine.DeltaTime);
      if(speed<maxspeed){
        speed=Calc.Approach(speed, maxspeed, acceleration*Engine.DeltaTime);
      }
      if(Vector2.Distance(Position,target)< 0.1f){
        break;
      }
      if(Position.Y<(float)SceneAs<Level>().Bounds.Top -16){
        RemoveSelf();
        state=Strawb.gone;
        yield break;
      }
      yield return null;
    }
    state=Strawb.idling;
    Collidable = true;
  }
}