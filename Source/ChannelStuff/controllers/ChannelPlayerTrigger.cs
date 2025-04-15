


using System;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelPlayerTrigger")]
public class ChannelPlayerTrigger:Trigger{
  string channel {get; set;}
  public enum Op{
    xor, and, or,
    set, max, min,
    add,
  }
  public Op op;
  public int value;
  bool activateOnEnter=false;
  bool activateOnleave=false;

  public ChannelPlayerTrigger(EntityData data, Vector2 offset):base(data, offset){
    
    channel = data.Attr("channel","");
    value = data.Int("value",1);
    switch(data.Attr("action")){
      case "dash":
        Add(new DashListener((Vector2 d)=>activate()));
        break;
      case "jump":
        Add(new JumpListener((int t)=>activate()));
        break;
      case "enter":
        activateOnEnter=true; break;
      case "leave":
        activateOnleave=true; break;

      default: DebugConsole.Write("Unknown action"+data.Attr("action")); break;
    }
    op = data.Attr("op","") switch {
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
    if(!PlayerIsInside) return;
    int oldval = ChannelState.readChannel(channel);
    //DebugConsole.Write(op.ToString());
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
  public override void OnEnter(Player player){
    base.OnEnter(player);
    if(activateOnEnter) activate();
  }
  public override void OnLeave(Player player){
    base.OnLeave(player);
    if(activateOnleave) activate();
  }
}