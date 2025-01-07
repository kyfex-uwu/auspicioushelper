

using System.Collections.Generic;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

public class auspicioushelperModuleSession : EverestModuleSession {
  public Dictionary<int,int> channelData = new Dictionary<int, int>();

  public void save(){
    DebugConsole.Write("Saving channel state");
    channelData.Clear();
    foreach(KeyValuePair<int, int> p in ChannelState.channelStates){
      channelData.Add(p.Key, p.Value);
    }
  }
  public void load(bool initialize){
    DebugConsole.Write("Loading channel state");
    if(initialize){
      channelData.Clear();
    }
    ChannelState.channelStates.Clear();
    foreach(KeyValuePair<int,int> p in channelData){
      ChannelState.channelStates.Add(p.Key,p.Value);
    }
    if(initialize) save();
  } 
}