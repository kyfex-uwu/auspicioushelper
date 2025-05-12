

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/HoldableReleaseTrigger")]
public class ReleaseHoldableTrigger:Trigger{
  bool force;
  bool once;
  public ReleaseHoldableTrigger(EntityData d, Vector2 offset):base(d,offset){
    force = d.Bool("force_throw");
    once = d.Bool("only_once");
  }
  void ex(Player p){
    if(p.Holding!=null){
      p.minHoldTimer = 0;
      if(force) p.Throw();
      if(once) RemoveSelf();
    }
  }
  public override void OnEnter(Player player){
    base.OnEnter(player);
    ex(player);
  }
  public override void OnStay(Player player){
    base.OnStay(player);
    ex(player);
  }
}