

using System;
using System.Collections.Generic;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

public class auspicioushelperModuleSession : EverestModuleSession {
  public class EntityDataId{
    public EntityData data;
    public EntityID id;
    public EntityDataId(EntityData d, EntityID id){
      data = d;
      this.id = id;
    }
  }
  public Dictionary<string,int> channelData = new Dictionary<string, int>();
  public List<EntityDataId> PersistentFollowers = new List<EntityDataId>();
  public HashSet<string> collectedTrackedCassettes = new HashSet<string>();
  public HashSet<int> openedGates = new HashSet<int>();

  public void save(){
    DebugConsole.Write("Saving channel state");
    channelData = ChannelState.save();
  }
  public void load(bool initialize){
    DebugConsole.Write("Loading channel state");
    if(initialize){
      channelData.Clear();
    }
    ChannelState.load(channelData);
    if(initialize) save();
  } 
}