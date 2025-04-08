


using System;
using System.Collections.Generic;
using Celeste.Mod.auspicioushelper;
using FMOD;
using Monocle;

namespace Celeste.Mods.auspicioushelper;
public static class ChannelState{
  public static Dictionary<string, int> channelStates = new Dictionary<string, int>();
  public static Dictionary<string, List<IChannelUser>> watching = new Dictionary<string, List<IChannelUser>>();
  enum Ops{
    none, not, lnot, xor, and, or, add, sub, mult, div, mrecip, mod, safemod, min, max, ge, le, gt, lt, eq, ne
  }
  struct modifier{
    int y;
    Ops op;
    public modifier(string s){

    }
    public int apply(int x){
      switch(op){
        case Ops.not: return ~x;
        case Ops.lnot: return x==0?1:0;
        case Ops.xor: return x^y;
        case Ops.and: return x&y;
        case Ops.or: return x|y;
        case Ops.add: return x+y;
        case Ops.sub: return x-y;
        case Ops.mult: return x*y;
        case Ops.div: return x/y;
        case Ops.mrecip: return y/x;
        case Ops.mod: return x%y;
        case Ops.safemod: return ((x%y)+y)%y;
        case Ops.min: return Math.Min(x,y);
        case Ops.max: return Math.Max(x,y);
        case Ops.ge: return x>=y?1:0;
        case Ops.le: return x<=y?1:0;
        case Ops.gt: return x>y?1:0;
        case Ops.lt: return x<y?1:0;
        case Ops.eq: return x==y?1:0;
        case Ops.ne: return x!=y?1:0;
        default: return x;
      }
    }
  }
  struct modifierDesc{
    string outname;
    List
  }
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