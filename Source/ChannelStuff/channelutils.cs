


using System.Collections.Generic;
using Celeste.Mod.auspicioushelper;
using Monocle;

namespace Celeste.Mods.auspicioushelper;
public static class ChannelState{
  public static Dictionary<string, int> channelStates = new Dictionary<string, int>();
  public static Dictionary<string, List<IChannelUser>> watching = new Dictionary<string, List<IChannelUser>>();
  public static int readChannel(string ch){
    int v=0;
    channelStates.TryGetValue(ch, out v);
    return v;
  }
  public static void SetChannel(string ch, int state){
    if(readChannel(ch) == state) return;
    channelStates[ch] = state;
    // foreach(ChannelBaseEntity b in Engine.Scene.Tracker.GetEntities<ChannelBaseEntity>()){
    //   if(b.channel == ch)b.setChVal(state);
    // }
    if (watching.TryGetValue(ch, out var list)) {
      foreach(IChannelUser b in list){
        b.setChVal(state);
      }
    }
  }
  public static void unwatchNow(IChannelUser b){
    if (watching.TryGetValue(b.channel, out var list)) {
      list.Remove(b);
    }
  }
  public static void watch(IChannelUser b){
    //DebugConsole.Write("watching new thing");
    if (!watching.TryGetValue(b.channel, out var list)) {
      list = new List<IChannelUser>();
      watching[b.channel] = list;
    }
    list.Add(b);
  }
  public static void unwatchAll(){
    watching.Clear();
  }
  public static void unwatchTemporary(){
    foreach(var pair in watching){
      var newlist = new List<IChannelUser>();
      foreach(IChannelUser e in pair.Value){
        if(e is Entity en && en.TagCheck(Tags.Persistent)){
          newlist.Add(e);
        }
      }
      watching[pair.Key] = newlist;
    }
  }
}