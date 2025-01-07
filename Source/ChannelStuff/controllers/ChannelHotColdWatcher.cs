


using System;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelHotColdWatcher")]
public class ChannelCoreWatcher:Entity {

  public int hotset;
  public int coldset;
  public int channel;
  public ChannelCoreWatcher(EntityData data, Vector2 offset):base(new Vector2(0,0)){
    hotset = data.Int("Hot_value",0);
    coldset = data.Int("Cold_value",1);
    channel = data.Int("channel",0);
    Add(new CoreModeListener(OnChangeMode));
  }
  public override void Added(Scene scene){
    base.Added(scene);
    try{
      OnChangeMode(SceneAs<Level>().coreMode);
    } catch(Exception ex){
      DebugConsole.Write($"Failed with {ex}");
    }
  }

  public void OnChangeMode(Session.CoreModes mode){
    ChannelState.SetChannel(channel, mode==Session.CoreModes.Cold? coldset:hotset);
  }
}