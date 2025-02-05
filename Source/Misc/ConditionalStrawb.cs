


using System.Collections;
using System.Linq;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeleste.Mods.auspicioushelper;

[CustomEntity("auspicioushelper/ConditionalStrawb")]
[RegisterStrawberry(true, true)]
[Tracked]
public class ConditionalStrawb:Entity, IStrawberry{
  public string idstr;
  public Follower follower;
  public bool collected;
  public bool ReturnWhenLost;
  public EntityID id;
  public bool isGhost;
  public bool wingedFollower;
  public bool winged;
  public bool following;
  Vector2? otherpos;
  public float ispeed;
  public float maxspeed;
  public float acceleration;
  public Sprite sprite;
  public ConditionalStrawb(EntityData data, Vector2 offset, EntityID id):base(data.Position+offset){
    idstr = data.Attr("idstr");
    this.id=id;
    isGhost = SaveData.Instance.CheckStrawberry(id);
    base.Collider = new Hitbox(14f, 14f, -7f, -7f);
    Add(new PlayerCollider(OnPlayer));
    Add(new MirrorReflection());
    Add(follower = new Follower(id, null, OnLoseLeader));
    follower.FollowDelay = 0.3f;
    otherpos=data.Nodes.Count() == 0?null:data.Nodes[0]+offset;

    wingedFollower= data.Bool("wingedFollower",false);
    winged = data.Bool("winged",false);
    Add(new DashListener(OnDash));
    ispeed=data.Float("ispeed",1.0f);
    maxspeed=data.Float("maxspeed",1.0f);
    acceleration=data.Float("acceleration",1f);
  }
  public override void Added(Scene scene){
    base.Added(scene);
    
  }
  public void OnDash(Vector2 dir){
    if(!following && winged){
      Add(new Coroutine(Fly(otherpos??new Vector2(Position.X,Position.Y-1000000))));
    }
    if(following && wingedFollower){
      follower.Leader.Followers.Remove(follower);
      follower.OnLoseLeaderUtil();
      follower.Leader=null;
      following=false;
    }
  }
  public void OnPlayer(Player p){
    if(following || collected) return;
    following=true;
    Audio.Play(isGhost ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", Position);
    p.Leader.GainFollower(follower);
  }
  public void OnCollect(){

  }
  public void OnLoseLeader(){
    if(collected || !ReturnWhenLost) return;

  }
  public IEnumerator Fly(Vector2 target){
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
        yield break;
      }
      yield return null;
    }
    following=false;
  }
}