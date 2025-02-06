
using System;
using Celeste.Mods.auspicioushelper;
using Monocle;

namespace Celeste.Mod.auspicioushelper;
[Tracked]
public class ChannelTracker : Component, IChannelUser{
  public string channel {get; set;}
  Action<int> onChannelChange;
  public ChannelTracker(string channel, Action<int> onChannelChange):base(true, false){
    this.channel=channel;
    this.onChannelChange=onChannelChange;
    ChannelState.watch(this);
  }
  public void setChVal(int val){
    onChannelChange(val);
  }
}