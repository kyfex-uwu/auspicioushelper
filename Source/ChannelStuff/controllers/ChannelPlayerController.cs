


using System;
using Celeste;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeleste.Mods.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelPlayerWatcher")]
public class ChannelPlayerWatcher:Entity{
  string channel {get; set;}
  public enum Op{
    xor, and, or,
    set, max, min,
    add,
  }
  public Op op;
  public int value;
  public ChannelPlayerWatcher(EntityData data, Vector2 offset):base(new Vector2(0,0)){
    channel = data.Attr("channel","");
    value = data.Int("value",1);
    switch(data.Attr("action")){
      case "dash":
        Add(new DashListener((Vector2 d)=>activate()));
        break;
      case "jump":
        Add(new JumpListener(()=>activate()));
        break;
      default: break;
    }
    op = data.Attr("operation","") switch {
      "xor"=>Op.xor,
      "and"=>Op.and,
      "or"=>Op.or,
      "max"=>Op.max,
      "min"=>Op.min,
      "add"=>Op.add,
      _=>Op.set
    };
  }
  public void activate(){
    int oldval = ChannelState.readChannel(channel);
    ChannelState.SetChannel(channel, op switch {
      Op.set => value,
      Op.xor => oldval ^ value,
      Op.and => value & oldval,
      Op.or => value | oldval,
      Op.add => oldval+value,
      Op.max => Math.Max(oldval,value),
      Op.min => Math.Min(value, oldval),
      _=>oldval
    });
  }
}