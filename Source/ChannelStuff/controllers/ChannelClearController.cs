using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelClearController")]
public class ChannelClearController:Entity {
  public ChannelClearController(EntityData data, Vector2 offset):base(new Vector2(0,0)){
    if(data.Bool("clear_all",false)){
      ChannelState.channelStates.Clear();
    }
    ChannelState.SetChannel(data.Int("channel",0),data.Int("value",0));
    RemoveSelf();
  }
}