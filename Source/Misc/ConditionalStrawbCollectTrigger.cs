


using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeleste.Mods.auspicioushelper;

[CustomEntity("auspicioushelper/ConditionalStrawbCollectTrigger")]
public class ConditionalStrawbCollectTrigger:Trigger {
  public string idstr;
  public ConditionalStrawbCollectTrigger(EntityData data, Vector2 offset):base(data, offset){
    idstr=data.Attr("strawberry_id");
  }
  public override void OnStay(Player player){
    base.OnStay(player);
    foreach(ConditionalStrawb s in Scene.Tracker.GetEntities<ConditionalStrawb>()){
      if(s.idstr == idstr && s.follower.Leader==player.Leader){
        s.OnCollect();
      }
    }
  }
}