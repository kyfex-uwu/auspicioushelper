


using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[Tracked]
[CustomEntity("auspicioushelper/ChannelTheo")]
public class ChannelTheo:TheoCrystal, IChannelUser{
  public static bool hooked = false;
  public int channel {get; set;}
  public bool active=false;
  public bool switchThrown;
  public ChannelTheo(EntityData data, Vector2 offset):base(data, offset){
    channel = data.Int("channel",0);
    switchThrown = data.Bool("switch_thrown_momentum",false);
    if(!hooked){
      hooked=true;
      On.Celeste.Player.Throw += PlayerThrowHook;
    }
  }
  public override void Added(Scene scene){
    base.Added(scene);
    ChannelState.watch(this);
  }
  public void setChVal(int val){
    active = (val&1)==1;
  }
  public static void PlayerThrowHook(On.Celeste.Player.orig_Throw orig, Player self){
    Holdable held = self.Holding;
    orig(self);
    foreach(ChannelTheo t in self.Scene.Tracker.GetEntities<ChannelTheo>()){
      if(t.Hold == held){
        if(t.switchThrown && t.active){
          Vector2 temp = self.Speed;
          self.Speed = t.Speed;
          t.Speed = temp;
        }
        break;
      }
    }
  }
}